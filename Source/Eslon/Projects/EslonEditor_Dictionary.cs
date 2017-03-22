using System;
using System.Collections.Generic;

namespace Eslon
{
    internal class EslonEditor_Dictionary<TKey, TValue> : EslonEditor
    {
        private EslonEnginePrime element;
        private PanActivator activator;

        public EslonEditor_Dictionary(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element, PanActivator activator) : base(logo, reader, writer)
        {
            this.element = element;
            this.activator = activator;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            IDictionary<TKey, TValue> collection;

            if (activator == null)
            {
                collection = new Dictionary<TKey, TValue>(list.Count);
            }
            else
            {
                if (parent == null)
                {
                    collection = (IDictionary<TKey, TValue>)activator.Invoke();
                }
                else
                {
                    collection = (IDictionary<TKey, TValue>)parent;
                    collection.Clear();
                }
            }

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                KeyValuePair<TKey, TValue> value = (KeyValuePair<TKey, TValue>)element.Cast(list.Code[i], null);

                try
                {
                    collection.Add(value.Key, value.Value);
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
            IDictionary<TKey, TValue> collection = (IDictionary<TKey, TValue>)value;

            EslonList list = new EslonList(collection.Count);

            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                list.Add(element.Digest(item));
            }

            return list;
        }
    }
}
