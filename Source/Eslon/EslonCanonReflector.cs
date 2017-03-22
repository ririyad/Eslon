using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Eslon
{
    internal class EslonCanonReflector : EslonReflector
    {
        private static readonly Type[] Classes = new Type[]
        {
            typeof(Dictionary<,>),
            typeof(List<>),
            typeof(KeyValuePair<,>),
            typeof(Nullable<>),
            typeof(Enum),
            typeof(Array)
        };

        private static readonly Type[] Interfaces = new Type[]
        {
            typeof(IDictionary<,>),
            typeof(IList<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>)
        };

        private EslonComposer composer;

        public EslonCanonReflector() { }

        public static Delegate CreateEnumerableCommitter(Type type, Type relay, Type elementType)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                ParameterInfo[] parameters;

                if (method.Name == "Add" && !method.IsGenericMethod && method.ReturnType == typeof(void) &&
                    (parameters = method.GetParameters()).Length == 1 && parameters[0].ParameterType == elementType && !parameters[0].IsOut)
                {
                    Type[] actionParameterTypes = new Type[] { relay, elementType };
                    DynamicMethod dynamicMethod = new DynamicMethod(String.Empty, null, actionParameterTypes, typeof(EslonCanonReflector), true);
                    ILGenerator gen = dynamicMethod.GetILGenerator();

                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Callvirt, method);
                    gen.Emit(OpCodes.Ret);

                    return dynamicMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(actionParameterTypes));
                }
            }

            throw new EslonEngineException(COM.Express("The enumerable of '%' does not specify an eligible method.", type.FullName));
        }

        protected override EslonEditor Elect(Type type)
        {
            if (composer == null)
            {
                composer = new EslonComposer(Engine);
            }

            PanActivator activator;
            Type[] gta;
            Type relay;
            int index;

            if ((index = ExtraReflection.RelateClass(type, Classes, out relay)) != -1 && (relay.IsAbstract || relay == type))
            {
                gta = relay.GetGenericArguments();

                switch (index)
                {
                    case 0: return CreateEditor(typeof(EslonEditor_Dictionary<,>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(typeof(KeyValuePair<,>).MakeGenericType(gta)), null);
                    case 1: return CreateEditor(typeof(EslonEditor_List<>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]), null);
                    case 2: return CreateEditor(typeof(EslonEditor_KeyValuePair<,>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]), Engine.Collect(gta[1]));
                    case 3: return new EslonEditor_Nullable(type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]));
                    case 4: return CreateEditor(typeof(EslonEditor_Enum<,>), new Type[] { type, Enum.GetUnderlyingType(type) }, GetEnumEditorArguments(type));
                    case 5: return CreateArrayEditor(type);
                }
            }

            if (type.IsClass)
            {
                if ((activator = PanDynamic.CreateActivator(type)) == null)
                {
                    throw new EslonEngineException(COM.Express("Unable to create an activator because class '%' does not specify an eligible constructor.", type.FullName));
                }

                if ((index = ExtraReflection.RelateInterface(type, Interfaces, out relay)) != -1)
                {
                    gta = relay.GetGenericArguments();

                    switch (index)
                    {
                        case 0: return CreateEditor(typeof(EslonEditor_Dictionary<,>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(typeof(KeyValuePair<,>).MakeGenericType(gta)), activator);
                        case 1: return CreateEditor(typeof(EslonEditor_List<>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]), activator);
                        case 2: return CreateEditor(typeof(EslonEditor_Collection<>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]), activator);
                        case 3: return CreateEditor(typeof(EslonEditor_Enumerable<>), gta, type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(gta[0]), activator, CreateEnumerableCommitter(type, relay, gta[0]));
                    }
                }

                return new EslonDevice(type, EslonCanon.Reader, EslonCanon.Writer, composer.Compose(type), activator);
            }

            return null;
        }

        private static object[] GetEnumEditorArguments(Type type)
        {
            return new object[] { type, EslonCanon.Reader, EslonCanon.Writer, (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0 ?
                EslonCanon.GetEditor(Enum.GetUnderlyingType(type)) : EslonCanon.GetHexEditor(Enum.GetUnderlyingType(type))) };
        }

        private static EslonEditor CreateEditor(Type def, Type[] gta, params object[] args)
        {
            return (EslonEditor)Activator.CreateInstance(def.MakeGenericType(gta), args);
        }

        private EslonEditor CreateArrayEditor(Type type)
        {
            return CreateEditor((type.GetArrayRank() == 1 ? typeof(EslonEditor_Array<>) : typeof(EslonEditor_MultiArray<>)), new Type[] { type.GetElementType() },
                type, EslonCanon.Reader, EslonCanon.Writer, Engine.Collect(type.GetElementType()));
        }
    }
}
