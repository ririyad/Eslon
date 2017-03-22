using System;
using System.Collections.Generic;

namespace Eslon
{
    internal class EslonEditor_Collection<T> : EslonEditor
    {
        private EslonEnginePrime element;
        private PanActivator activator;

        public EslonEditor_Collection(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element, PanActivator activator) : base(logo, reader, writer)
        {
            this.element = element;
            this.activator = activator;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            ICollection<T> collection;

            if (parent == null)
            {
                collection = (ICollection<T>)activator.Invoke();
            }
            else
            {
                collection = (ICollection<T>)parent;
                collection.Clear();
            }

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T value = (T)element.Cast(list.Code[i], null);

                try
                {
                    collection.Add(value);
                }
                catch (ArgumentException exception)
                {
                    throw new EslonMoveException(volume, Logo, COM.Express("An argument exception was thrown at position %.", i + 1), exception);
                }
            }

            return collection;
        }

        protected override EslonVolume AutoDigest(object value)
        {
            ICollection<T> collection = (ICollection<T>)value;

            EslonList list = new EslonList(collection.Count);

            foreach (T item in collection)
            {
                list.Add(element.Digest(item));
            }

            return list;
        }
    }
}
