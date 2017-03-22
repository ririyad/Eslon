using System;

namespace Eslon
{
    internal class EslonEditor_Array<T> : EslonEditor
    {
        private EslonEnginePrime element;

        public EslonEditor_Array(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element) : base(logo, reader, writer)
        {
            this.element = element;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            T[] array = new T[list.Count];

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                array[i] = (T)element.Cast(list.Code[i], null);
            }

            return array;
        }

        protected override EslonVolume AutoDigest(object value)
        {
            T[] array = (T[])value;

            EslonList list = new EslonList(array.Length);

            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                list.Add(element.Digest(array[i]));
            }

            return list;
        }
    }
}
