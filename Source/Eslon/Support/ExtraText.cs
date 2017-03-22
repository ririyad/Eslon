using System.Runtime.CompilerServices;

namespace Eslon
{
    internal static class ExtraText
    {
        private static readonly char[] UpperTable = new char[]
        {
            '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0A', '\x0B',
            '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17',
            '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F', '\x20', '\x21', '\x22', '\x23',
            '\x24', '\x25', '\x26', '\x27', '\x28', '\x29', '\x2A', '\x2B', '\x2C', '\x2D', '\x2E', '\x2F',
            '\x30', '\x31', '\x32', '\x33', '\x34', '\x35', '\x36', '\x37', '\x38', '\x39', '\x3A', '\x3B',
            '\x3C', '\x3D', '\x3E', '\x3F', '\x40', '\x41', '\x42', '\x43', '\x44', '\x45', '\x46', '\x47',
            '\x48', '\x49', '\x4A', '\x4B', '\x4C', '\x4D', '\x4E', '\x4F', '\x50', '\x51', '\x52', '\x53',
            '\x54', '\x55', '\x56', '\x57', '\x58', '\x59', '\x5A', '\x5B', '\x5C', '\x5D', '\x5E', '\x5F',
            '\x60', '\x41', '\x42', '\x43', '\x44', '\x45', '\x46', '\x47', '\x48', '\x49', '\x4A', '\x4B',
            '\x4C', '\x4D', '\x4E', '\x4F', '\x50', '\x51', '\x52', '\x53', '\x54', '\x55', '\x56', '\x57',
            '\x58', '\x59', '\x5A'
        };

        private static readonly char[] LowerTable = new char[]
        {
            '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0A', '\x0B',
            '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17',
            '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F', '\x20', '\x21', '\x22', '\x23',
            '\x24', '\x25', '\x26', '\x27', '\x28', '\x29', '\x2A', '\x2B', '\x2C', '\x2D', '\x2E', '\x2F',
            '\x30', '\x31', '\x32', '\x33', '\x34', '\x35', '\x36', '\x37', '\x38', '\x39', '\x3A', '\x3B',
            '\x3C', '\x3D', '\x3E', '\x3F', '\x40', '\x61', '\x62', '\x63', '\x64', '\x65', '\x66', '\x67',
            '\x68', '\x69', '\x6A', '\x6B', '\x6C', '\x6D', '\x6E', '\x6F', '\x70', '\x71', '\x72', '\x73',
            '\x74', '\x75', '\x76', '\x77', '\x78', '\x79', '\x7A', '\x5B', '\x5C', '\x5D', '\x5E', '\x5F',
            '\x60', '\x61', '\x62', '\x63', '\x64', '\x65', '\x66', '\x67', '\x68', '\x69', '\x6A', '\x6B',
            '\x6C', '\x6D', '\x6E', '\x6F', '\x70', '\x71', '\x72', '\x73', '\x74', '\x75', '\x76', '\x77',
            '\x78', '\x79', '\x7A'
        };

        private static readonly int[] Charmap = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 1, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            72, 72, 72, 72, 72, 72, 72, 72, 72, 72, 0, 0, 0, 0, 0, 0, 0, 80, 80, 80, 80, 80, 80, 80,
            80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 0, 0, 0, 0, 64,
            0, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96, 96,
            96, 96, 96, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsControl(char c)
        {
            return (c < 161 && (Charmap[c] & 1) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpace(char c)
        {
            return (c < 161 && (Charmap[c] & 2) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLinebreak(char c)
        {
            return (c < 161 && (Charmap[c] & 4) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBreak(char c)
        {
            return (c < 161 && (Charmap[c] & 6) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(char c)
        {
            return (c < 161 && (Charmap[c] & 8) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpper(char c)
        {
            return (c < 161 && (Charmap[c] & 16) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLower(char c)
        {
            return (c < 161 && (Charmap[c] & 32) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlpha(char c)
        {
            return (c < 161 && (Charmap[c] & 48) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWord(char c)
        {
            return (c < 161 && (Charmap[c] & 64) != 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlphaWord(char c)
        {
            return (c < 161 && (Charmap[c] & 72) == 64);
        }

        public static int Compare(string a, string b, int begin, int end)
        {
            int length = (end - begin) + 1;

            if (length > b.Length)
            {
                return 0;
            }

            if (length == b.Length)
            {
                for (int i = begin; i <= end; i++)
                {
                    if (a[i] != b[i - begin])
                    {
                        return (a[i] > b[i - begin]) ? i + 1 : -3;
                    }
                }

                return -1;
            }

            return -2;
        }

        public static int Expect(char chr, string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (str[pos] != chr)
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectSpaces(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsSpace(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectBreaks(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsBreak(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectDigits(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos <= end; pos++)
            {
                if (!IsDigit(str[pos]))
                {
                    break;
                }
            }

            return pos - begin;
        }

        public static int ExpectRight(char chr, string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos >= end; pos--)
            {
                if (str[pos] != chr)
                {
                    break;
                }
            }

            return begin - pos;
        }

        public static int ExpectSpacesRight(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos >= end; pos--)
            {
                if (!IsSpace(str[pos]))
                {
                    break;
                }
            }

            return begin - pos;
        }

        public static int ExpectBreaksRight(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos >= end; pos--)
            {
                if (!IsBreak(str[pos]))
                {
                    break;
                }
            }

            return begin - pos;
        }

        public static int ExpectDigitsRight(string str, int begin, int end)
        {
            int pos;

            for (pos = begin; pos >= end; pos--)
            {
                if (!IsDigit(str[pos]))
                {
                    break;
                }
            }

            return begin - pos;
        }

        public static string ToUpper(string str, int begin, int end)
        {
            if (begin > end)
            {
                return str;
            }

            char[] cx = new char[str.Length];

            if (begin != 0)
            {
                str.CopyTo(0, cx, 0, begin);
            }

            for (int i = begin; i <= end; i++)
            {
                int n = str[i];

                cx[i] = (n < 123) ? UpperTable[n] : (char)n;
            }

            if (end != str.Length - 1)
            {
                str.CopyTo(end + 1, cx, end + 1, (str.Length - end) - 1);
            }

            return new string(cx);
        }

        public static string ToLower(string str, int begin, int end)
        {
            if (begin > end)
            {
                return str;
            }

            char[] cx = new char[str.Length];

            if (begin != 0)
            {
                str.CopyTo(0, cx, 0, begin);
            }

            for (int i = begin; i <= end; i++)
            {
                int n = str[i];

                cx[i] = (n < 123) ? LowerTable[n] : (char)n;
            }

            if (end != str.Length - 1)
            {
                str.CopyTo(end + 1, cx, end + 1, (str.Length - end) - 1);
            }

            return new string(cx);
        }

        public static string Trim(string str)
        {
            int x = ExpectBreaks(str, 0, str.Length - 1);
            int y = ExpectBreaksRight(str, str.Length - 1, x);

            return (x == 0 && y == 0) ? str : str.Substring(x, (str.Length - x) - y);
        }

        public static string TrimLeft(string str)
        {
            int x = ExpectBreaks(str, 0, str.Length - 1);

            return (x == 0) ? str : str.Substring(x, str.Length - x);
        }

        public static string TrimRight(string str)
        {
            int x = ExpectBreaksRight(str, str.Length - 1, 0);

            return (x == 0) ? str : str.Substring(0, str.Length - x);
        }
    }
}
