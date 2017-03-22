using System.IO;
using System.Text;

namespace Eslon
{
    internal class PanStreamReader : PanReader
    {
        private const int MinByteBufferSize = 128;

        private bool encodingDetectionEnabled;
        private byte[] byteBuffer;
        private Stream stream;
        private Encoding encoding;
        private Decoder decoder;
        private bool streamDepleted;
        private int byteBufferSize;
        private int byteBufferBound;
        private int byteBufferPos;

        public PanStreamReader(Stream stream, Encoding encoding, bool enableEncodingDetection, int byteBufferSize)
        {
            if (byteBufferSize < MinByteBufferSize)
            {
                byteBufferSize = MinByteBufferSize;
            }

            this.encodingDetectionEnabled = enableEncodingDetection;
            this.byteBuffer = new byte[byteBufferSize];
            this.stream = stream;
            this.encoding = encoding;
            this.byteBufferSize = byteBufferSize;
        }

        public override void Dispose()
        {
            if (stream == null)
            {
                return;
            }

            stream.Dispose();
            byteBuffer = null;
            stream = null;
            encoding = null;
            decoder = null;

            base.Dispose();
        }

        protected override int Add(int offset, int length)
        {
            if (stream.Position == 0 && !streamDepleted)
            {
                ShiftBytes();

                if (encodingDetectionEnabled)
                {
                    DetectEncoding();
                    Commit();
                }
            }

            if (length == 0)
            {
                if (decoder == null)
                {
                    Commit();
                }

                length = bufferSize;
            }

            bool final = false;
            int count = 0;

            while (count != length && !final)
            {
                if (byteBufferPos == byteBufferBound)
                {
                    if (!streamDepleted)
                    {
                        ShiftBytes();
                    }

                    final = streamDepleted;
                }

                int byteCount;
                int charCount;
                bool completed;

                decoder.Convert(byteBuffer, byteBufferPos, byteBufferBound - byteBufferPos, buffer, offset + count, length - count, final, out byteCount, out charCount, out completed);
                byteBufferPos += byteCount;
                count += charCount;
            }

            return count;
        }

        private void DetectEncoding()
        {
            int bom1 = ((byteBufferBound < 1 ? 0x11 : byteBuffer[0]) << 8) | (byteBufferBound < 2 ? 0x11 : byteBuffer[1]);
            int bom2 = ((byteBufferBound < 3 ? 0x11 : byteBuffer[2]) << 8) | (byteBufferBound < 4 ? 0x11 : byteBuffer[3]);

            switch (bom1)
            {
                case 0xEFBB:
                    if ((bom2 & 0xBF00) == 0xBF00)
                    {
                        encoding = Encoding.UTF8;
                        byteBufferPos = 3;
                    }

                    return;

                case 0xFEFF:
                    encoding = new UnicodeEncoding(true, true);
                    byteBufferPos = 2;

                    return;

                case 0xFFFE:
                    if (bom2 == 0x0000)
                    {
                        encoding = new UTF32Encoding(false, true);
                        byteBufferPos = 4;
                    }
                    else
                    {
                        encoding = new UnicodeEncoding(false, true);
                        byteBufferPos = 2;
                    }

                    return;

                case 0x0000:
                    if (bom2 == 0xFEFF)
                    {
                        encoding = new UTF32Encoding(true, true);
                        byteBufferPos = 4;
                    }

                    return;
            }
        }

        private void ShiftBytes()
        {
            int offset = 0;

            do
            {
                int count = stream.Read(byteBuffer, offset, byteBufferSize - offset);

                if (count == 0)
                {
                    streamDepleted = true;

                    break;
                }

                offset += count;
            }
            while (offset != byteBufferSize);

            byteBufferBound = offset;
            byteBufferPos = 0;
        }

        private void Commit()
        {
            decoder = encoding.GetDecoder();
            InitBuffer();
        }

        private void InitBuffer()
        {
            int size;

            if (streamDepleted)
            {
                size = encoding.GetMaxCharCount(byteBufferBound);
            }
            else
            {
                size = encoding.GetMaxCharCount(byteBufferSize);

                if (size < 16)
                {
                    throw new CriticalException("Unable to devise a character buffer.");
                }
            }

            buffer = new char[size];
            bufferSize = size;
        }
    }
}
