using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class Literal : Element
    {
        public Literal(object value)
        {
            Value = value;
        }

        public object Value { get; }

        private bool _raw;

        public static Literal Raw(string value) => new Literal(value) { _raw = true };

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (_raw) return builder.Append(Value);
            if (Value == null) return builder.Append("null");

            if (IsNumeric(Value.GetType()))
                return builder.Append(Value);

            switch (Value)
            {
                case string @string: return String(builder, @string);
                case bool @bool: return builder.Append(@bool.ToString().ToLower());
                case Guid guid: return String(builder, guid);
                case DateTime dateTime: return String(builder, dateTime);
                case DateTimeOffset dateTime: return String(builder, dateTime); // TODO: Date()
            }

            if (Value.GetType().IsArray)
                return Array(builder, ((IEnumerable)Value).Cast<object>().ToList());

            if (Value is IEnumerable enumerable && !(Value is IDictionary) && Value.GetType().Namespace.StartsWith("System."))
                return Array(builder, enumerable.Cast<object>().ToList());

            throw new NotSupportedException("Provided type is not supported as a typescript literal.");
        }

        public static bool TryCreate(object value, out Literal literal)
        {
            try
            {
                var result = new Literal(value);
                result.Build(new TypescriptBuilder(), null);
                literal = result;
                return true;
            }
            catch (NotSupportedException) { }

            literal = null;
            return false;
        }

        private TypescriptBuilder String(TypescriptBuilder builder, object value) => builder
            .Append('"')
            .Append(value.ToString().Replace("\"", "\\\""))
            .Append('"');

        private TypescriptBuilder Array(TypescriptBuilder builder, ICollection<object> value)
        {
            var literals = value.Select(x => new Literal(x));
            if (!value.Any()) return builder.Append("[]");

            return builder.Append('[')
                .Indent()
                .NewLine()
                .Join(literals, b => b.Append(',').NewLine())
                .Unindent()
                .NewLine()
                .Append(']');
        }

        private static readonly List<Type> StringTypes = new List<Type>
        {
            typeof(string),
            typeof(Guid)
        };

        private static readonly List<Type> NumberTypes = new List<Type>
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
        };

        private static bool IsNumeric(Type t)
        {
            var underlyingType = Nullable.GetUnderlyingType(@t) ?? t;
            return NumberTypes.Contains(underlyingType);
        }
    }
}