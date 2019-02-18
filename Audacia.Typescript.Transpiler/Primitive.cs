using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler
{
    public abstract class Primitive
    {
        public static Primitive String { get; } = new StringPrimitive();
        public static Primitive Number { get; } = new NumberPrimitive();
        public static Primitive Boolean { get; } = new BooleanPrimitive();
        public static Primitive Array { get; } = new ArrayPrimitive();
        public static Primitive Date { get; } = new DatePrimitive();
        public static Primitive Map { get; } = new MapPrimitive();

        protected virtual Action<TypescriptBuilder> GenericArguments(Type source) => builder => builder
            .Append('<')
            .Join(source.GetGenericArguments().Select(a => a.TypescriptName()), ", ")
            .Append('>');

        public static ICollection<Primitive> Defaults { get; } = typeof(Primitive)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Primitive))
            .Select(p => (Primitive)p.GetValue(null))
            .ToList();

        public abstract void Literal(TypescriptBuilder builder, object value);

        public abstract void Identifier(TypescriptBuilder builder, Type source);

        public static string Literal(object value)
        {
            if (value == null) return "null";
            var type = value.GetType();
            type = type = Nullable.GetUnderlyingType(type) ?? type;

            var builder = new TypescriptBuilder();
            var match = Defaults.FirstOrDefault(primitive => primitive.CanWrite(value.GetType()));
            if (match == null) return null;

            match.Literal(builder, value);
            return builder.ToString();
        }

        public static string Identifier(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            var builder = new TypescriptBuilder();
            var match = Defaults.FirstOrDefault(primitive => primitive.CanWrite(type));
            if (match == null) return null;

            match.Identifier(builder, type);
            return builder.ToString();
        }

        public abstract bool CanWrite(Type value);

        private class ArrayPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value)
            {
                var array = ((IEnumerable) value).Cast<object>().ToList();
                var literals = array.Select(Primitive.Literal);
                if (!array.Any()) builder.Append("[]");

                builder.Append('[')
                    .Indent()
                    .NewLine()
                    .Join(literals, b => b.Append(',').NewLine())
                    .Unindent()
                    .NewLine()
                    .Append(']');
            }

            public override void Identifier(TypescriptBuilder builder, Type source) => builder
                .Append("Array")
                .Append(GenericArguments(source));

            public override bool CanWrite(Type type) => !(type == typeof(IDictionary))
                && (type.IsArray || type == typeof(IEnumerable))
                && (type.Namespace?.StartsWith(nameof(System)) ?? false);
        }

        private class MapPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder
                .Append("new Map")
                .Append(GenericArguments(value.GetType()));

            public override void Identifier(TypescriptBuilder builder, Type source)
            {
                builder.Append("Map");

                if (source.IsGenericType) builder.Append(GenericArguments(source));
                else builder.Append("<any, any>");
            }

            public override bool CanWrite(Type type) => type == typeof(IDictionary)
                && (type.Namespace?.StartsWith(nameof(System)) ?? false);
        }

        private class StringPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder
                .Append('"')
                .Append(value.ToString().Replace("\"", "\\\""))
                .Append('"');

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("string");

            public override bool CanWrite(Type type) =>
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

            public override bool CanWrite(Type type) => type == typeof(bool);
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
            public override bool CanWrite(Type type) => type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

        private class NumberPrimitive : Primitive
        {
            public override void Literal(TypescriptBuilder builder, object value) => builder.Append(value);

            public override void Identifier(TypescriptBuilder builder, Type source) => builder.Append("number");

            public override bool CanWrite(Type type) => new[]
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
    }
}