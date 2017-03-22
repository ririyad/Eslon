using System;

namespace Eslon
{
    internal class EslonEditor_Nullable : EslonEditor
    {
        private EslonEnginePrime element;

        public EslonEditor_Nullable(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element) : base(logo, reader, writer)
        {
            this.element = element;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            return element.Cast(volume, null);
        }

        protected override EslonVolume AutoDigest(object value)
        {
            return element.Digest(value);
        }
    }
}
