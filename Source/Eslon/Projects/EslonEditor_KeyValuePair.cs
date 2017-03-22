using System;
using System.Collections.Generic;

namespace Eslon
{
    internal class EslonEditor_KeyValuePair<TKey, TValue> : EslonEditor
    {
        private EslonEnginePrime keyElement;
        private EslonEnginePrime valueElement;

        public EslonEditor_KeyValuePair(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime keyElement, EslonEnginePrime valueElement) : base(logo, reader, writer)
        {
            this.keyElement = keyElement;
            this.valueElement = valueElement;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            if (list.Count != 2)
            {
                throw new EslonMoveException(volume, Logo);
            }

            return new KeyValuePair<TKey, TValue>((TKey)keyElement.Cast(list.Code[0], null), (TValue)valueElement.Cast(list.Code[1], null));
        }

        protected override EslonVolume AutoDigest(object value)
        {
            KeyValuePair<TKey, TValue> pair = (KeyValuePair<TKey, TValue>)value;

            return new EslonList(2) { keyElement.Digest(pair.Key), valueElement.Digest(pair.Value) };
        }
    }
}
