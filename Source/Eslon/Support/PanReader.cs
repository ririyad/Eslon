using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Eslon
{
    internal abstract class PanReader : IDisposable
    {
        public char[] buffer;
        public int lineTicket;
        public int bufferSize;
        public int bufferBound;
        public int bufferPos;
        public long token;
        public long linePos;

        protected long total;

        public PanReader()
        {
            this.lineTicket = 1;
        }

        public bool Depleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (bufferPos == bufferBound && !Shift());
            }
        }

        public virtual void Dispose()
        {
            buffer = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Admit()
        {
            token = GetPos();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Test()
        {
            if (token == GetPos())
            {
                Abort(1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Impend()
        {
            if (Depleted)
            {
                Error(EslonReadExceptionCode.Depletion, GetPos());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dismiss()
        {
            if (!Depleted)
            {
                Error(EslonReadExceptionCode.SyntaxError, GetPos() + 1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance()
        {
            bufferPos++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Read()
        {
            if (Depleted)
            {
                Error(EslonReadExceptionCode.Depletion, GetPos());
            }

            return buffer[bufferPos++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char Peek()
        {
            if (Depleted)
            {
                Error(EslonReadExceptionCode.Depletion, GetPos());
            }

            return buffer[bufferPos];
        }

        public char[] Arrange(int count)
        {
            if (!CheckCapacity(count))
            {
                Error(EslonReadExceptionCode.Depletion, GetPos());
            }

            char[] cx = new char[count];

            Array.Copy(buffer, bufferPos, cx, 0, count);
            bufferPos += count;

            return cx;
        }

        public bool Match(string text)
        {
            int length = text.Length;

            if (!CheckCapacity(length))
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (text[i] != buffer[bufferPos + i])
                {
                    return false;
                }
            }

            bufferPos += length;

            return true;
        }

        public string Consume(int[] map, bool demand)
        {
            string str = String.Empty;

            if (!Depleted)
            {
                int offset = bufferPos;

                while (true)
                {
                    int n = buffer[bufferPos];

                    if (n < map.Length && map[n] > 0)
                    {
                        str += new string(buffer, offset, bufferPos - offset);

                        break;
                    }

                    if (++bufferPos == bufferBound)
                    {
                        str += new string(buffer, offset, bufferPos - offset);

                        if (!Shift())
                        {
                            break;
                        }

                        offset = 0;
                    }
                }
            }

            if (demand && str.Length == 0)
            {
                Abort(1);
            }

            return str;
        }

        public void Skip(int[] map)
        {
            if (Depleted)
            {
                return;
            }

            while (true)
            {
                int n = buffer[bufferPos];

                if ((n < map.Length && map[n] > 0) || (++bufferPos == bufferBound && !Shift()))
                {
                    break;
                }
            }
        }

        public void SkipBreaks()
        {
            while (!Depleted)
            {
                char c = buffer[bufferPos];

                if (ExtraText.IsSpace(c))
                {
                    bufferPos++;

                    continue;
                }

                if (ExtraText.IsLinebreak(c))
                {
                    bufferPos++;

                    if (c == '\r' && !Depleted && buffer[bufferPos] == '\n')
                    {
                        bufferPos++;
                    }

                    AdmitNewline();

                    continue;
                }

                return;
            }
        }

        public void SkipSpace()
        {
            while (!Depleted && ExtraText.IsSpace(buffer[bufferPos]))
            {
                bufferPos++;
            }
        }

        public void AssumeNewline()
        {
            if (!Depleted)
            {
                if (buffer[bufferPos++] == '\r' && !Depleted && buffer[bufferPos] == '\n')
                {
                    bufferPos++;
                }

                AdmitNewline();
            }
        }

        public void AdmitNewline()
        {
            linePos = GetPos();
            lineTicket++;
        }

        public bool CheckCapacity(int count)
        {
            do
            {
                if (count <= bufferBound - bufferPos)
                {
                    return true;
                }
            }
            while (Shift());

            return false;
        }

        public bool Shift()
        {
            int remainder = bufferBound - bufferPos;
            int margin = bufferSize - remainder;

            if (margin != 0 || bufferSize == 0)
            {
                if (remainder != 0 && bufferPos != 0)
                {
                    Array.Copy(buffer, bufferPos, buffer, 0, remainder);
                }

                bufferBound = remainder + Add(remainder, margin);
                total += bufferPos;
                bufferPos = 0;

                if (bufferBound != remainder)
                {
                    return true;
                }
            }

            return false;
        }

        public long GetPos()
        {
            return total + bufferPos;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Abort(int count)
        {
            if (Depleted)
            {
                Error(EslonReadExceptionCode.Depletion, GetPos());
            }
            else
            {
                Error(EslonReadExceptionCode.SyntaxError, GetPos() + count);
            }
        }

        [DebuggerHidden]
        public void Error(Enum code, long position)
        {
            throw new EslonReadException(code, position, lineTicket, (position - linePos) + 1);
        }

        protected abstract int Add(int offset, int length);
    }
}
