using System;
using System.Globalization;

namespace Eslon
{
    internal static class Extra
    {
        public static T[] CreateArray<T>(int length, T example)
        {
            T[] array = new T[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = example;
            }

            return array;
        }

        public static T[] DupeArray<T>(T[] array)
        {
            return DupeArray(array, 0, array.Length);
        }

        public static T[] DupeArray<T>(T[] array, int index)
        {
            return DupeArray(array, index, array.Length - index);
        }

        public static T[] DupeArray<T>(T[] array, int index, int length)
        {
            T[] counter = new T[length];

            Array.Copy(array, index, counter, 0, length);

            return counter;
        }

        public static string ToStringInvariant(this IFormattable value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this IFormattable value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        public static bool DetectNull<T>(this T[] array) where T : class
        {
            return (Array.IndexOf(array, null) != -1);
        }
    }
}
