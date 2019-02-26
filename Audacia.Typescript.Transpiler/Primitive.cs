using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler
{
    public class ObjectLiteral : Element
    {
        private int MaxDepth { get; } = 2;
        
        public ObjectLiteral(object value) => Value = value;

        public object Value { get; set; }

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent) => Build(builder, Value, 0);

        private TypescriptBuilder Build(TypescriptBuilder builder, object value, int depth)
        {
            if (value == null) return builder.Append("null");

            if (Primitive.CanWrite(value.GetType()))
                return builder.Append(Primitive.Literal(value));

            if (value.GetType().IsEnum)
                return builder
                    .Append(value.GetType().Name)
                    .Append('.')
                    .Append(System.Enum.GetName(value.GetType(), value).CamelCase());

            var properties = value.GetType()
                .GetProperties(BindingFlags.NonPublic
                               | BindingFlags.Public
                               | BindingFlags.Instance
                               | BindingFlags.DeclaredOnly)
                .Where(p => !p.GetIndexParameters().Any())
                .Where(p => !p.GetMethod.IsPrivate)
                .ToList();

            if (!properties.Any() || depth >= MaxDepth) return builder.Append("{}");

            builder.Append('{');

            if(properties.Count != 1)
                builder.Indent().NewLine();
 
            foreach (var property in properties)
            {
                builder.Append(property.Name.CamelCase())
                    .Append(": ");

                try
                {
                    Build(builder, property.GetValue(value), depth + 1);
                }
                catch (Exception e)
                {
                    // TODO: Log this somewhere
                    Build(builder, null, depth + 1); 
                }

                if (properties.Count == 1)
                    return builder.Append('}');

                if (property == properties.Last())
                    builder.Unindent().NewLine();
                else builder.Append(',').NewLine();
            }

            return builder.Append('}');
        }
    }

    public abstract class Primitive
    {
        public static Primitive Any { get; } = new AnyPrimitive();

        public static Primitive String { get; } = new StringPrimitive();

        public static Primitive Number { get; } = new NumberPrimitive();

        public static Primitive Boolean { get; } = new BooleanPrimitive();

        public static Primitive Array { get; } = new ArrayPrimitive();

        public static Primitive Date { get; } = new DatePrimitive();

        public static Primitive Map { get; } = new MapPrimitive();

        protected virtual Action<TypescriptBuilder> GenericArguments(Type source) => builder =>
        {
            if (!source.IsGenericType) return;

            builder
                .Append('<')
                .Join(source.GetGenericArguments().Select(a => a.TypescriptName()), ", ")
                .Append('>');
        };

        public static ICollection<Primitive> Defaults { get; } = typeof(Primitive)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Primitive))
            .Select(p => (Primitive)p.GetValue(null))
            .ToList();

        public abstract void Literal(TypescriptBuilder builder, object value);

        public abstract void Identifier(TypescriptBuilder builder, Type source);

        public static bool CanWrite(Type type) => Defaults.Any(primitive => primitive.CanWriteValue(type));

        public static string Literal(object value)
        {
            if (value == null) return "null";
            var type = value.GetType();
            type = type = Nullable.GetUnderlyingType(type) ?? type;

            var builder = new TypescriptBuilder();
            var match = Defaults.FirstOrDefault(primitive => primitive.CanWriteValue(value.GetType()));
            if (match == null) return null;

            match.Literal(builder, value);
            return builder.ToString();
        }

        public static string Identifier(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            var builder = new TypescriptBuilder();
            var match = Defaults.FirstOrDefault(primitive => primitive.CanWriteValue(type));
            if (match == null) return null;

            match.Identifier(builder, type);
            return builder.ToString();
        }

        public abstract bool CanWriteValue(Type value);

        private class ArrayPrimitive : Primitive
        {
            public static List<string> MaskToList(Type type, System.Enum mask)
            {
                if (type.IsSubclassOf(typeof(System.Enum)) == false)
                    throw new ArgumentException();

                if (type.GetCustomAttribute<FlagsAttribute>() == null)
                    throw new ArgumentException();

                return System.Enum.GetValues(type)
                    .Cast<System.Enum>()
                    .Where(mask.HasFlag)
                    .Select(x => type.Name + "." + x.ToString().CamelCase())
                    .ToList();
            }

            public override void Literal(TypescriptBuilder builder, object value)
            {
                IEnumerable<string> literals;
                if (value is IEnumerable enumerable)
                {
                    var array = enumerable.Cast<object>().ToList();

                    if (!array.Any())
                    {
                        builder.Append("[]");
                        return;
                    }

                    literals = array.Select(Primitive.Literal);
                }
                else if (value.GetType().IsEnum)
                {
                    literals = MaskToList(value.GetType(), (System.Enum)value);

                    if (!literals.Any())
                    {
                        builder.Append("[]");
                        return;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                builder.Append("[ ")
//                    .Indent()
//                    .NewLine()
                    .Join(literals, b => b.Append(", "))//.NewLine())
//                    .Unindent()
//                    .NewLine()
                    .Append(" ]");
            }

            public override void Identifier(TypescriptBuilder builder, Type source)
            {
                if (source.IsArray)
                    builder.Append("Array")
                        .Append('<')
                        .Append(source.GetElementType().TypescriptName())
                        .Append('>');
                else if (source.IsEnum)
                    builder.Append("Array")
                        .Append('<')
                        .Append(source.Name)
                        .Append('>');
                else
                    builder
                        .Append("Array")
                        .Append(GenericArguments(source));
            }

            public override bool CanWriteValue(Type type)
            {
                if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null) return true;

                if (typeof(IDictionary).IsAssignableFrom(type)) return false;
                if (type.IsArray) return true;

                return typeof(IEnumerable).IsAssignableFrom(type)
                    && (type != typeof(string))
                    && (type.Namespace?.StartsWith(nameof(System)) ?? false)
                    && !type.Name.Contains("Dictionary");
            }
        }

        private class MapPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder
                .Append("new Map")
                .Append(GenericArguments(value.GetType()))
                .Append("()");

            public override void Identifier(TypescriptBuilder builder, Type source)
            {
                builder.Append("Map");

                if (source.IsGenericType) builder.Append(GenericArguments(source));
                else builder.Append("<any, any>");
            }

            public override bool CanWriteValue(Type type) => // TODO: Dodgy
                (type.Namespace?.StartsWith(nameof(System)) ?? false)
                && type.Name.Contains("Dictionary");
        }

        private class StringPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder
                .Append('"')
                .Append(value.ToString().Replace("\"", "\\\""))
                .Append('"');

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("string");

            public override bool CanWriteValue(Type type) =>
                type == typeof(string)
                || type == typeof(char)
                || type == typeof(Guid)
                || type == typeof(TimeSpan);
        }

        private class BooleanPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder
                .Append(value.ToString().ToLower());

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("boolean");

            public override bool CanWriteValue(Type type) => type == typeof(bool);
        }

        private class DatePrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value)
            {
                switch (value)
                {
                    case DateTime dateTime:
                        builder
                            .Append("new Date(\"")
                            .Append(dateTime)
                            .Append("\")");
                        break;

                    case DateTimeOffset dateTimeOffset:
                        builder
                            .Append("new Date(\"")
                            .Append(dateTimeOffset.DateTime)
                            .Append("\")");
                        break;
                }
            }

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("Date" +
                                                                                                      "");
            public override bool CanWriteValue(Type type) => type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

        private class NumberPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder.Append(value);

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("number");

            public override bool CanWriteValue(Type type) => new[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(double),
                typeof(float)
            }.Contains(type);
        }

        private class AnyPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder.Append("{ }");

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("any");

            public override bool CanWriteValue(Type type) => new[]
            {
                typeof(object),
                typeof(DynamicObject),
                typeof(ExpandoObject),
            }.Contains(type);
        }
    }
}