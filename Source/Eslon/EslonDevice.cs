using System;
using System.Reflection;

namespace Eslon
{
    internal class EslonDevice : EslonEditor
    {
        private string[] keys;
        private EslonDeviceNode[] nodes;
        private PanActivator activator;

        public EslonDevice(Type logo, EslonReader reader, EslonWriter writer, EslonDeviceNode[] nodes, PanActivator activator) : base(logo, reader, writer)
        {
            this.keys = CollectKeys(nodes);
            this.nodes = nodes;
            this.activator = activator;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonMap map = volume.Move<EslonMap>();
            EslonVolume[] values;

            if (!map.Collect(keys, out values))
            {
                throw new EslonMoveException(volume, Logo);
            }

            if (parent == null)
            {
                parent = activator.Invoke();
            }

            int length = nodes.Length;

            for (int i = 0; i < length; i++)
            {
                EslonDeviceNode node = nodes[i];
                object value = node.Prime.Cast(values[i], (node.Adapt ? node.Accessor.Invoke(parent) : null));

                try
                {
                    node.Mutator.Invoke(parent, value);
                }
                catch (ArgumentException exception)
                {
                    throw new EslonMoveException(volume, Logo, COM.Express("An argument exception was thrown upon setting the member '%.%'.", Logo.Name, node.Name), exception);
                }
            }

            return parent;
        }

        protected override EslonVolume AutoDigest(object value)
        {
            EslonMap map = new EslonMap(nodes.Length);

            int length = nodes.Length;

            for (int i = 0; i < length; i++)
            {
                EslonDeviceNode node = nodes[i];

                map.Add(node.Name, node.Prime.Digest(node.Accessor.Invoke(value)));
            }

            return map;
        }

        private static string[] CollectKeys(EslonDeviceNode[] nodes)
        {
            string[] array = new string[nodes.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                array[i] = nodes[i].Name;
            }

            return array;
        }
    }

    internal class EslonDeviceNode
    {
        public string Name;
        public EslonEnginePrime Prime;
        public PanAccessor Accessor;
        public PanMutator Mutator;
        public bool Adapt;

        public EslonDeviceNode() { }

        public static EslonDeviceNode Generate(string name, EslonEnginePrime prime, FieldInfo field)
        {
            return new EslonDeviceNode()
            {
                Name = name,
                Prime = prime,
                Accessor = PanDynamic.CreateAccessor(field),
                Mutator = PanDynamic.CreateMutator(field),
                Adapt = field.FieldType.IsClass
            };
        }

        public static EslonDeviceNode Generate(string name, EslonEnginePrime prime, PropertyInfo property)
        {
            return new EslonDeviceNode()
            {
                Name = name,
                Prime = prime,
                Accessor = PanDynamic.CreateAccessor(property),
                Mutator = PanDynamic.CreateMutator(property),
                Adapt = property.PropertyType.IsClass
            };
        }
    }
}
