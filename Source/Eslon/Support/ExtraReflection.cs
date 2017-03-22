using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eslon
{
    internal static class ExtraReflection
    {
        public static string GetAssemblyCompanyName(Assembly assembly)
        {
            object[] array = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            return (array.Length == 0) ? null : ((AssemblyCompanyAttribute)array[0]).Company;
        }

        public static int RelateClass(Type type, Type[] signs, out Type relay)
        {
            return RelateTypeLog(EnumerateClass(type), signs, out relay);
        }

        public static int RelateInterface(Type type, Type[] signs, out Type relay)
        {
            return RelateTypeLog(EnumerateInterface(type), signs, out relay);
        }

        public static int RelateTypeLog(Type[] log, Type[] signs, out Type relay)
        {
            if (log.Length != 0)
            {
                for (int i = 0; i < signs.Length; i++)
                {
                    foreach (Type item in log)
                    {
                        if (EquateType(item, signs[i]))
                        {
                            relay = item;

                            return i;
                        }
                    }
                }
            }

            relay = null;

            return -1;
        }

        public static bool RelateTypeLog(Type[] log, Type sign, out Type relay)
        {
            foreach (Type item in log)
            {
                if (EquateType(item, sign))
                {
                    relay = item;

                    return true;
                }
            }

            relay = null;

            return false;
        }

        public static bool EquateType(Type alpha, Type beta)
        {
            return (alpha == beta || (beta.IsGenericParameter && !alpha.IsGenericParameter) ||
                (alpha.IsGenericType && beta.IsGenericType && alpha.GetGenericTypeDefinition() == beta.GetGenericTypeDefinition() && EquateGenericType(alpha, beta)));
        }

        public static bool EquateGenericType(Type alpha, Type beta)
        {
            Type[] argsA = alpha.GetGenericArguments();
            Type[] argsB = beta.GetGenericArguments();

            for (int i = 0; i < argsA.Length; i++)
            {
                if (!EquateType(argsA[i], argsB[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Type NormalizeType(Type type)
        {
            return type.IsGenericType ? NormalizeGenericType(type) : type;
        }

        public static Type NormalizeGenericType(Type type)
        {
            Type def = type.GetGenericTypeDefinition();
            Type[] defArgs = def.GetGenericArguments();
            Type[] args = type.GetGenericArguments();

            bool success = false;

            for (int i = 0; i < args.Length; i++)
            {
                Type arg = args[i];

                if ((arg = (arg.IsGenericParameter ? defArgs[i] : NormalizeType(arg))) != args[i])
                {
                    args[i] = arg;
                    success = true;
                }
            }

            return success ? def.MakeGenericType(args) : type;
        }

        public static Type[] EnumerateClass(Type type)
        {
            if (type.IsInterface)
            {
                return Type.EmptyTypes;
            }

            List<Type> log = new List<Type>();

            do
            {
                log.Add(NormalizeType(type));
            }
            while ((type = type.BaseType) != null);

            return log.ToArray();
        }

        public static Type[] EnumerateInterface(Type type)
        {
            Type[] array = GetInterfaces(type);

            if (type.IsInterface)
            {
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = NormalizeType(type);
            }

            return array;
        }

        public static Type[] GetInterfaces(Type type)
        {
            Type[] array = type.GetInterfaces();

            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                array[i] = NormalizeType(array[i]);
            }

            return array;
        }
    }
}
