namespace Eslon
{
    internal class PanStringReader : PanReader
    {
        private const int MinBufferSize = 16;

        private string str;
        private int current;

        public PanStringReader(string str) : this(str, 1024) { }

        public PanStringReader(string str, int bufferSize)
        {
            if (bufferSize < MinBufferSize)
            {
                bufferSize = MinBufferSize;
            }

            if (bufferSize > str.Length)
            {
                bufferSize = str.Length;
            }

            this.str = str;
            this.bufferSize = bufferSize;
            this.buffer = new char[bufferSize];
        }

        public override void Dispose()
        {
            str = null;

            base.Dispose();
        }

        protected override int Add(int offset, int length)
        {
            int remainder = str.Length - current;

            if (remainder != 0)
            {
                int count = (remainder > length) ? length : remainder;

                str.CopyTo(current, buffer, offset, count);
                current += count;

                return count;
            }

            return 0;
        }
    }
}
