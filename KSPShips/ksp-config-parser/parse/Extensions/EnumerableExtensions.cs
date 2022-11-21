using System;

namespace KSPCfgParser.Extensions
{
    public static class EnumerableExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnumOrDefault<T>(this string value) where T : struct
        {
            return Enum.TryParse<T>(value, true, out T result) ? result : default(T);
        }
    }
}