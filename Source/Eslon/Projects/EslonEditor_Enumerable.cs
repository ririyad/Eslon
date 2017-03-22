using System;
using System.Collections.Generic;

namespace Eslon
{
    internal class EslonEditor_Enumerable<T> : EslonEditor
    {
        private EslonEnginePrime element;
        private PanActivator activator;
        private Action<IEnumerable<T>, T> committer;

        public EslonEditor_Enumerable(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element, PanActivator activator, Action<IEnumerable<T>, T> committer) : base(logo, reader, writer)
        {
            this.element = element;
            this.activator = activator;
            this.committer = committer;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            IEnumerable<T> collection = (IEnumerable<T>)activator.Invoke();

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                T value = (T)element.Cast(list.Code[i], null);

                try
                {
                    committer.Invoke(collection, value);
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
            IEnumerable<T> collection = (IEnumerable<T>)value;

            EslonList list = new EslonList();

            foreach (T item in collection)
            {
                list.Add(element.Digest(item));
            }

            return list;
        }
    }
}
