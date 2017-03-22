using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eslon
{
    internal class EslonComposer
    {
        private EslonEngine engine;

        public EslonComposer(EslonEngine engine)
        {
            this.engine = engine;
        }

        public EslonDeviceNode[] Compose(Type source)
        {
            MemberInfo[] selection = Select(source);

            if (selection.Length == 0)
            {
                throw new EslonEngineException(COM.Express("Unable to create a device because class '%' does not specify any eligible members.", source.FullName));
            }

            return CreateNodes(selection);
        }

        private static MemberInfo[] Select(Type source)
        {
            List<MemberInfo> list = new List<MemberInfo>();
            HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);

            foreach (Type type in ExtraReflection.EnumerateClass(source))
            {
                foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if ((member.MemberType & (MemberTypes.Field | MemberTypes.Property)) == 0 || names.Contains(member.Name))
                    {
                        continue;
                    }

                    FieldInfo field = (member as FieldInfo);

                    if (field != null)
                    {
                        if (field.IsInitOnly)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        PropertyInfo property = (PropertyInfo)member;

                        if (property.GetGetMethod(false) == null || property.GetSetMethod(false) == null || property.GetIndexParameters().Length != 0)
                        {
                            continue;
                        }
                    }

                    list.Add(member);
                    names.Add(member.Name);
                }
            }

            return list.ToArray();
        }

        private EslonDeviceNode[] CreateNodes(MemberInfo[] selection)
        {
            EslonDeviceNode[] nodes = new EslonDeviceNode[selection.Length];

            for (int i = 0; i < selection.Length; i++)
            {
                MemberInfo member = selection[i];
                FieldInfo field = (member as FieldInfo);

                if (field != null)
                {
                    nodes[i] = EslonDeviceNode.Generate(field.Name, engine.Collect(field.FieldType), field);
                }
                else
                {
                    PropertyInfo property = (PropertyInfo)member;

                    nodes[i] = EslonDeviceNode.Generate(property.Name, engine.Collect(property.PropertyType), property);
                }
            }

            return nodes;
        }
    }
}
