using System;
using System.Collections.Generic;

namespace Eslon.Java
{
    internal class JavaCanonReflector : EslonReflector
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

        public JavaCanonReflector() { }

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
                    case 0: return CreateEditor(typeof(EslonEditor_Dictionary<,>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(typeof(KeyValuePair<,>).MakeGenericType(gta)), null);
                    case 1: return CreateEditor(typeof(EslonEditor_List<>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]), null);
                    case 2: return CreateEditor(typeof(EslonEditor_KeyValuePair<,>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]), Engine.Collect(gta[1]));
                    case 3: return new EslonEditor_Nullable(type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]));
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
                        case 0: return CreateEditor(typeof(EslonEditor_Dictionary<,>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(typeof(KeyValuePair<,>).MakeGenericType(gta)), activator);
                        case 1: return CreateEditor(typeof(EslonEditor_List<>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]), activator);
                        case 2: return CreateEditor(typeof(EslonEditor_Collection<>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]), activator);
                        case 3: return CreateEditor(typeof(EslonEditor_Enumerable<>), gta, type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(gta[0]), activator, EslonCanonReflector.CreateEnumerableCommitter(type, relay, gta[0]));
                    }
                }

                return new EslonDevice(type, JavaCanon.Reader, JavaCanon.Writer, composer.Compose(type), activator);
            }

            return null;
        }

        private static object[] GetEnumEditorArguments(Type type)
        {
            return new object[] { type, JavaCanon.Reader, JavaCanon.Writer, JavaCanon.GetEditor(Enum.GetUnderlyingType(type)) };
        }

        private static EslonEditor CreateEditor(Type def, Type[] gta, params object[] args)
        {
            return (EslonEditor)Activator.CreateInstance(def.MakeGenericType(gta), args);
        }

        private EslonEditor CreateArrayEditor(Type type)
        {
            return CreateEditor((type.GetArrayRank() == 1 ? typeof(EslonEditor_Array<>) : typeof(EslonEditor_MultiArray<>)), new Type[] { type.GetElementType() },
                type, JavaCanon.Reader, JavaCanon.Writer, Engine.Collect(type.GetElementType()));
        }
    }
}
