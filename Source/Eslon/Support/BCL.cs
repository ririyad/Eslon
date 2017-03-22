using System;

namespace Eslon
{
    internal static class BCL
    {
        public const int MaxArrayLength = 0x7FEFFFFF;

        public static bool IsNullableType(Type type)
        {
            return (!type.IsValueType || IsTypeOfNullable(type));
        }

        public static bool IsTypeOfNullable(Type type)
        {
            return (type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static T[] Empty<T>()
        {
            return EmptyArray<T>.Value;
        }

        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
    }
}
