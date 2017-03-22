using System;
using System.Text;

namespace Eslon
{
    internal static class ExtraRender
    {
        private const string Int16MaxValueHex = "7FFF";
        private const string Int16MaxValueString = "32767";
        private const string Int16MinValueHex = "8000";
        private const string Int16MinValueString = "32768";
        private const string UInt16MaxValueHex = "FFFF";
        private const string UInt16MaxValueString = "65535";
        private const string Int32MaxValueHex = "7FFFFFFF";
        private const string Int32MaxValueString = "2147483647";
        private const string Int32MinValueHex = "80000000";
        private const string Int32MinValueString = "2147483648";
        private const string UInt32MaxValueHex = "FFFFFFFF";
        private const string UInt32MaxValueString = "4294967295";
        private const string Int64MaxValueHex = "7FFFFFFFFFFFFFFF";
        private const string Int64MaxValueString = "9223372036854775807";
        private const string Int64MinValueHex = "8000000000000000";
        private const string Int64MinValueString = "9223372036854775808";
        private const string UInt64MaxValueHex = "FFFFFFFFFFFFFFFF";
        private const string UInt64MaxValueString = "18446744073709551615";

        public static readonly Func<object, string> Converter = new Func<object, string>(ToString);

        public static bool Scan(string text, int[] map)
        {
            int length = text.Length;

            for (int i = 0; i < length; i++)
            {
                int n = text[i];

                if (n < map.Length && map[n] > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static string ToDecimalString(byte value)
        {
            return ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(sbyte value)
        {
            return (value < 0) ? "-" + ExtraCode.EmitDecimalString(-value) : ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(short value)
        {
            return (value < 0) ? "-" + ExtraCode.EmitDecimalString(-value) : ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(ushort value)
        {
            return ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(int value)
        {
            return (value < 0) ? "-" + (value == Int32.MinValue ? Int32MinValueString : ExtraCode.EmitDecimalString(-value)) : ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(uint value)
        {
            return ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(long value)
        {
            return (value < 0) ? "-" + (value == Int64.MinValue ? Int64MinValueString : ExtraCode.EmitDecimalString(-value)) : ExtraCode.EmitDecimalString(value);
        }

        public static string ToDecimalString(ulong value)
        {
            return ExtraCode.EmitDecimalString(value);
        }

        public static string ToHex(byte value)
        {
            return "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(sbyte value)
        {
            return (value < 0) ? "-0x" + ExtraCode.EmitHexString((byte)-value) : "0x" + ExtraCode.EmitHexString((byte)value);
        }

        public static string ToHex(short value)
        {
            return (value < 0) ? "-0x" + ExtraCode.EmitHexString((short)-value) : "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(ushort value)
        {
            return "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(int value)
        {
            return (value < 0) ? "-0x" + ExtraCode.EmitHexString(-value) : "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(uint value)
        {
            return "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(long value)
        {
            return (value < 0) ? "-0x" + ExtraCode.EmitHexString(-value) : "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToHex(ulong value)
        {
            return "0x" + ExtraCode.EmitHexString(value);
        }

        public static string ToString(float value)
        {
            string s = value.ToStringInvariant("R");

            return ((s.Length == 1 && ExtraText.IsDigit(s[0])) || ExtraText.ExpectDigits(s, 0, s.Length - 1) == s.Length) ? s + ".0" : s;
        }

        public static string ToString(double value)
        {
            string s = value.ToStringInvariant("R");

            return ((s.Length == 1 && ExtraText.IsDigit(s[0])) || ExtraText.ExpectDigits(s, 0, s.Length - 1) == s.Length) ? s + ".0" : s;
        }

        public static string ToString(decimal value)
        {
            string s = value.ToStringInvariant();

            return ((s.Length == 1 && ExtraText.IsDigit(s[0])) || ExtraText.ExpectDigits(s, 0, s.Length - 1) == s.Length) ? s + ".0" : s;
        }

        public static string ToString(object value)
        {
            IFormattable cast = (value as IFormattable);

            return (cast != null) ? cast.ToStringInvariant() : (value == null ? String.Empty : value.ToString());
        }

        public static bool ParseInt16(string str, out short result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & Number.Valid) != 0)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? ((number & Number.Negative) != 0 ? Int16MinValueHex : Int16MaxValueHex) :
                    ((number & Number.Negative) != 0 ? Int16MinValueString : Int16MaxValueString)), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastInt16Hex(str, x, y) : (short)ExtraCode.CastInt32Decimal(str, x, y);

                    if ((number & Number.Negative) != 0)
                    {
                        result = (short)-result;
                    }

                    return true;
                }
            }

            result = default(short);

            return false;
        }

        public static bool ParseInt32(string str, out int result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & Number.Valid) != 0)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? ((number & Number.Negative) != 0 ? Int32MinValueHex : Int32MaxValueHex) :
                    ((number & Number.Negative) != 0 ? Int32MinValueString : Int32MaxValueString)), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastInt32Hex(str, x, y) : ExtraCode.CastInt32Decimal(str, x, y);

                    if ((number & Number.Negative) != 0)
                    {
                        result = -result;
                    }

                    return true;
                }
            }

            result = default(int);

            return false;
        }

        public static bool ParseInt64(string str, out long result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & Number.Valid) != 0)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? ((number & Number.Negative) != 0 ? Int64MinValueHex : Int64MaxValueHex) :
                    ((number & Number.Negative) != 0 ? Int64MinValueString : Int64MaxValueString)), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastInt64Hex(str, x, y) : ExtraCode.CastInt64Decimal(str, x, y);

                    if ((number & Number.Negative) != 0)
                    {
                        result = -result;
                    }

                    return true;
                }
            }

            result = default(long);

            return false;
        }

        public static bool ParseUInt16(string str, out ushort result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & (Number.Valid | Number.Negative)) == Number.Valid)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? UInt16MaxValueHex : UInt16MaxValueString), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastUInt16Hex(str, x, y) : (ushort)ExtraCode.CastInt32Decimal(str, x, y);

                    return true;
                }
            }

            result = default(ushort);

            return false;
        }

        public static bool ParseUInt32(string str, out uint result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & (Number.Valid | Number.Negative)) == Number.Valid)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? UInt32MaxValueHex : UInt32MaxValueString), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastUInt32Hex(str, x, y) : ExtraCode.CastUInt32Decimal(str, x, y);

                    return true;
                }
            }

            result = default(uint);

            return false;
        }

        public static bool ParseUInt64(string str, out ulong result)
        {
            int x = 0;
            int y = str.Length - 1;
            Number number = ParseNumber(str, ref x, ref y);

            if ((number & (Number.Valid | Number.Negative)) == Number.Valid)
            {
                if ((number & Number.MixedHex) != 0)
                {
                    str = ExtraText.ToUpper(str, x, y);
                }

                if (ExtraText.Compare(str, ((number & Number.Hex) != 0 ? UInt64MaxValueHex : UInt64MaxValueString), x, y) < 0)
                {
                    result = ((number & Number.Hex) != 0) ? ExtraCode.CastUInt64Hex(str, x, y) : ExtraCode.CastUInt64Decimal(str, x, y);

                    return true;
                }
            }

            result = default(ulong);

            return false;
        }

        public static string Detonate(string format, params object[] args)
        {
            return Detonate(format, args, '%', Converter);
        }

        public static string Detonate(string format, object[] args, char sign, Func<object, string> converter)
        {
            if (converter == null)
            {
                converter = Converter;
            }

            StringBuilder sb = new StringBuilder(format.Length * 3);

            int current = 0;
            int offset = 0;
            int length = format.Length;

            for (int i = 0; i < length; i++)
            {
                if (format[i] == sign)
                {
                    if (current == args.Length)
                    {
                        API.ThrowFormatException();
                    }

                    sb.Append(format, offset, i - offset);
                    sb.Append(converter.Invoke(args[current++]));
                    offset = i + 1;
                }
            }

            sb.Append(format, offset, length - offset);

            return sb.ToString();
        }

        private static Number ParseNumber(string str, ref int x, ref int y)
        {
            Number number = Number.Valid;

            if (y != -1)
            {
                if (ExtraText.IsBreak(str[0]) || ExtraText.IsBreak(str[y]))
                {
                    x += ExtraText.ExpectBreaks(str, 0, y);
                    y -= ExtraText.ExpectBreaksRight(str, y, x);
                }

                if (x <= y)
                {
                    if (str[x] == '-')
                    {
                        number |= Number.Negative;
                        x++;
                    }
                    else if (str[x] == '+')
                    {
                        x++;
                    }

                    if (x < y && str[x] == '0')
                    {
                        if (str[x + 1] == 'x' || str[x + 1] == 'X')
                        {
                            number |= Number.Hex;
                            x += 2;
                        }

                        x += ExtraText.Expect('0', str, x, y - 1);
                    }

                    if (x <= y)
                    {
                        if ((number & Number.Hex) != 0)
                        {
                            number ^= (ExtraCode.ExpectUpperHex(str, x, y) > y - x) ? Number.MixedHex :
                                (ExtraCode.ExpectHex(str, x, y) > y - x ? Number.UpperHex : Number.Hex);

                            if ((number & Number.Hex) != 0)
                            {
                                return number;
                            }
                        }
                        else
                        {
                            if (ExtraText.ExpectDigits(str, x, y) > y - x)
                            {
                                return number;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        [Flags]
        private enum Number
        {
            Valid = 1,
            Negative = 2,
            UpperHex = 4,
            MixedHex = 8,
            Hex = 12
        }
    }
}
