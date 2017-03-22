using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Eslon
{
    internal delegate object PanAccessor(object obj);
    internal delegate void PanMutator(object obj, object value);
    internal delegate bool PanInstanceChecker(object obj);
    internal delegate object PanActivator();

    internal static class PanDynamic
    {
        private static readonly Type[] AccessorParameterTypes = new Type[] { typeof(object) };
        private static readonly Type[] MutatorParameterTypes = new Type[] { typeof(object), typeof(object) };
        private static readonly Type[] InstanceCheckerParameterTypes = new Type[] { typeof(object) };

        public static PanAccessor CreateAccessor(FieldInfo field)
        {
            API.Check(field, nameof(field));

            DynamicMethod method = new DynamicMethod(String.Empty, typeof(object), AccessorParameterTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType)
            {
                gen.Emit(OpCodes.Box, field.FieldType);
            }

            gen.Emit(OpCodes.Ret);

            return (PanAccessor)method.CreateDelegate(typeof(PanAccessor));
        }

        public static PanAccessor CreateAccessor(PropertyInfo property)
        {
            API.Check(property, nameof(property));

            DynamicMethod method = new DynamicMethod(String.Empty, typeof(object), AccessorParameterTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt, property.GetGetMethod(false));

            if (property.PropertyType.IsValueType)
            {
                gen.Emit(OpCodes.Box, property.PropertyType);
            }

            gen.Emit(OpCodes.Ret);

            return (PanAccessor)method.CreateDelegate(typeof(PanAccessor));
        }

        public static PanMutator CreateMutator(FieldInfo field)
        {
            API.Check(field, nameof(field));

            DynamicMethod method = new DynamicMethod(String.Empty, null, MutatorParameterTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit((field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass), field.FieldType);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);

            return (PanMutator)method.CreateDelegate(typeof(PanMutator));
        }

        public static PanMutator CreateMutator(PropertyInfo property)
        {
            API.Check(property, nameof(property));

            DynamicMethod method = new DynamicMethod(String.Empty, null, MutatorParameterTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit((property.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass), property.PropertyType);
            gen.Emit(OpCodes.Callvirt, property.GetSetMethod(false));
            gen.Emit(OpCodes.Ret);

            return (PanMutator)method.CreateDelegate(typeof(PanMutator));
        }

        public static PanActivator CreateActivator(Type type)
        {
            API.Check(type, nameof(type));

            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);

            if (constructor == null)
            {
                return null;
            }

            DynamicMethod method = new DynamicMethod(String.Empty, typeof(object), Type.EmptyTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Ret);

            return (PanActivator)method.CreateDelegate(typeof(PanActivator));
        }

        public static PanInstanceChecker CreateInstanceChecker(Type type)
        {
            API.Check(type, nameof(type));

            DynamicMethod method = new DynamicMethod(String.Empty, typeof(bool), InstanceCheckerParameterTypes, typeof(PanDynamic), true);
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Isinst, type);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Cgt_Un);
            gen.Emit(OpCodes.Ret);

            return (PanInstanceChecker)method.CreateDelegate(typeof(PanInstanceChecker));
        }
    }
}
