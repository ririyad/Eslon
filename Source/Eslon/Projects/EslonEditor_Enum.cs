using System;

namespace Eslon
{
    internal class EslonEditor_Enum<TEnum, TU> : EslonEditor
    {
        private EslonEditor element;

        public EslonEditor_Enum(Type logo, EslonReader reader, EslonWriter writer, EslonEditor element) : base(logo, reader, writer)
        {
            this.element = element;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            return (TEnum)element.DirectCast(volume, null);
        }

        protected override EslonVolume AutoDigest(object value)
        {
            return element.DirectDigest((TU)value);
        }
    }
}
