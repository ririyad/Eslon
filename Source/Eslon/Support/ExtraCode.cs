using System.Runtime.CompilerServices;

namespace Eslon
{
    internal static class ExtraCode
    {
        private static readonly char[] HexFieldCharset = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        private static readonly char[] HexCharset = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        private static readonly int[] HexDecodingTable = new int[]
        {
            16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
            16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 16, 16, 16, 16, 16, 16, 16, 10, 11, 12, 13, 14, 15, 16,
            16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
            16, 10, 11, 12, 13, 14, 15
        };

        private static readonly int[] Charmap = new int[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 4, 4, 4, 4, 4, 4
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperHex(char c)
        {
            return (c < 103 && (Charmap[c] & 3) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerHex(char c)
        {
            return (c < 103 && (Charmap[c] & 5) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHex(char c)
        {
            return (c < 103 && (Charmap[c] & 7) != 0);
        }

        public static int ExpectUpperHex(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsUpperHex(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectLowerHex(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsLowerHex(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectHex(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsHex(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static string EmitDecimalString(int n)
        {
            char[] cx = new char[10];

            int index = 10;
            int rem;

            do
            {
                cx[--index] = (char)(48 + (n - ((rem = n / 10) * 10)));
            }
            while ((n = rem) != 0);

            return new string(cx, index, 10 - index);
        }

        public static string EmitDecimalString(uint n)
        {
            char[] cx = new char[10];

            int index = 10;
            uint rem;

            do
            {
                cx[--index] = (char)(48 + (n - ((rem = n / 10) * 10)));
            }
            while ((n = rem) != 0);

            return new string(cx, index, 10 - index);
        }

        public static string EmitDecimalString(long n)
        {
            char[] cx = new char[19];

            int index = 19;
            long rem;

            do
            {
                cx[--index] = (char)(48 + (n - ((rem = n / 10) * 10)));
            }
            while ((n = rem) != 0);

            return new string(cx, index, 19 - index);
        }

        public static string EmitDecimalString(ulong n)
        {
            char[] cx = new char[20];

            int index = 20;
            ulong rem;

            do
            {
                cx[--index] = (char)(48 + (n - ((rem = n / 10) * 10)));
            }
            while ((n = rem) != 0);

            return new string(cx, index, 20 - index);
        }

        public static string EmitHexString(byte[] bytes)
        {
            char[] cx = new char[bytes.Length * 2];

            int length = bytes.Length;

            for (int i = 0; i < length; i++)
            {
                byte b = bytes[i];

                cx[i * 2] = HexCharset[b >> 4];
                cx[(i * 2) + 1] = HexCharset[b & 0x0F];
            }

            return new string(cx);
        }

        public static string EmitHexString(byte b)
        {
            return new string(new char[] { HexFieldCharset[b >> 4], HexFieldCharset[b & 0x0F] });
        }

        public static string EmitHexString(char c)
        {
            return EmitHexString((short)c);
        }

        public static string EmitHexString(ushort n)
        {
            return EmitHexString((short)n);
        }

        public static string EmitHexString(short n)
        {
            char[] cx = new char[4];

            for (int i = 3; i >= 0; i--)
            {
                cx[i] = HexFieldCharset[n & 0x0F];
                n >>= 4;
            }

            return new string(cx);
        }

        public static string EmitHexString(uint n)
        {
            return EmitHexString((int)n);
        }

        public static string EmitHexString(int n)
        {
            char[] cx = new char[8];

            for (int i = 7; i >= 0; i--)
            {
                cx[i] = HexFieldCharset[n & 0x0F];
                n >>= 4;
            }

            return new string(cx);
        }

        public static string EmitHexString(ulong n)
        {
            return EmitHexString((long)n);
        }

        public static string EmitHexString(long n)
        {
            char[] cx = new char[16];

            for (int i = 15; i >= 0; i--)
            {
                cx[i] = HexFieldCharset[n & 0x0F];
                n >>= 4;
            }

            return new string(cx);
        }

        public static int CastInt32Decimal(string str, int begin, int end)
        {
            int n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n * 10) + (str[i] - 48);
            }

            return n;
        }

        public static uint CastUInt32Decimal(string str, int begin, int end)
        {
            uint n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n * 10) + (uint)(str[i] - 48);
            }

            return n;
        }

        public static long CastInt64Decimal(string str, int begin, int end)
        {
            long n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n * 10) + (str[i] - 48);
            }

            return n;
        }

        public static ulong CastUInt64Decimal(string str, int begin, int end)
        {
            ulong n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n * 10) + (ulong)(str[i] - 48);
            }

            return n;
        }

        public static byte[] CastBytes(string str)
        {
            int count = str.Length / 2;
            bool even = ((str.Length & 1) == 0);

            byte[] bx = new byte[even ? count : count + 1];

            for (int i = 0; i < count; i++)
            {
                bx[i] = (byte)((HexDecodingTable[str[i * 2]] << 4) | HexDecodingTable[str[(i * 2) + 1]]);
            }

            if (!even)
            {
                bx[count] = (byte)HexDecodingTable[str[str.Length - 1]];
            }

            return bx;
        }

        public static char CastCharHex(string str, int begin, int end)
        {
            return (char)CastInt32Hex(str, begin, end);
        }

        public static ushort CastUInt16Hex(string str, int begin, int end)
        {
            return (ushort)CastInt32Hex(str, begin, end);
        }

        public static short CastInt16Hex(string str, int begin, int end)
        {
            return (short)CastInt32Hex(str, begin, end);
        }

        public static uint CastUInt32Hex(string str, int begin, int end)
        {
            return (uint)CastInt32Hex(str, begin, end);
        }

        public static int CastInt32Hex(string str, int begin, int end)
        {
            int n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n << 4) | HexDecodingTable[str[i]];
            }

            return n;
        }

        public static ulong CastUInt64Hex(string str, int begin, int end)
        {
            return (ulong)CastInt64Hex(str, begin, end);
        }

        public static long CastInt64Hex(string str, int begin, int end)
        {
            long n = 0;

            for (int i = begin; i <= end; i++)
            {
                n = (n << 4) | (long)HexDecodingTable[str[i]];
            }

            return n;
        }
    }
}
