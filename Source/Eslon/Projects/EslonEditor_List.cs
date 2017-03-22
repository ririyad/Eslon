using System;
using System.Collections.Generic;

namespace Eslon
{
    internal class EslonEditor_List<T> : EslonEditor
    {
        private EslonEnginePrime element;
        private PanActivator activator;

        public EslonEditor_List(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element, PanActivator activator) : base(logo, reader, writer)
        {
            this.element = element;
            this.activator = activator;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            IList<T> collection;

            if (activator == null)
            {
                collection = new List<T>(list.Count);
            }
            else
            {
                if (parent == null)
                {
                    collection = (IList<T>)activator.Invoke();
                }
                else
                {
                    collection = (IList<T>)parent;
                    collection.Clear();
                }
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
            IList<T> collection = (IList<T>)value;

            EslonList list = new EslonList(collection.Count);

            int count = collection.Count;

            for (int i = 0; i < count; i++)
            {
                list.Add(element.Digest(collection[i]));
            }

            return list;
        }
    }
}
