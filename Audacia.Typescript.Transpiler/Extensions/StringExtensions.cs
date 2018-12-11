using System.Linq;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length < 2) return s.ToLowerInvariant();
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        public static string SanitizeTypeName(this string s) => s.Split('`').First();
    }
}