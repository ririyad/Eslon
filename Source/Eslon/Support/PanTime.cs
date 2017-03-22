using System;

namespace Eslon
{
    internal static class PanTime
    {
        private const int MaxMonth = 12;
        private const int MaxHour = 23;
        private const int MaxMinute = 59;
        private const int MaxSecond = 59;
        private const int MaxUtcOffsetHour = 14;

        private static readonly Parser[] Parsers = new Parser[]
        {
            new Parser(ParseYear),
            new Parser(ParseMonth),
            new Parser(ParseDay),
            new Parser(ParseHour),
            new Parser(ParseMinute),
            new Parser(ParseSecond),
            new Parser(ParseMillisecond),
            new Parser(ParseUtcOffsetHour),
            new Parser(ParseUtcOffsetMinute)
        };

        private static readonly string Template = "0000-00-00T00:00:00.000+00:00";
        private static readonly int[] Primes = new int[] { 4, 7, 10, 13, 16, 19, 23, 26, 29 };

        public static string Serialize(DateTime value)
        {
            return Serialize(value.Kind == DateTimeKind.Utc ? new DateTimeOffset(value) : RoundDateTime(value));
        }

        public static string Serialize(DateTimeOffset value)
        {
            char[] cx = Template.ToCharArray();

            for (int i = 0; i < 9; i++)
            {
                int n = 0;

                switch (i)
                {
                    case 0: n = value.Year; break;
                    case 1: n = value.Month; break;
                    case 2: n = value.Day; break;
                    case 3: n = value.Hour; break;
                    case 4: n = value.Minute; break;
                    case 5: n = value.Second; break;
                    case 6: n = value.Millisecond; break;
                    case 7: n = (value.Offset < TimeSpan.Zero) ? -value.Offset.Hours : value.Offset.Hours; break;
                    case 8: n = value.Offset.Minutes; break;
                }

                string s = ExtraCode.EmitDecimalString(n);

                s.CopyTo(0, cx, Primes[i] - s.Length, s.Length);
            }

            return new string(cx);
        }

        public static bool Parse(string text, out DateTime result)
        {
            API.Check(text, nameof(text));

            Frame frame = new Frame();

            if (!Parse(text, ref frame))
            {
                result = default(DateTime);

                return false;
            }

            TimeSpan utcOffset = frame.GetUtcOffset();

            if (utcOffset == TimeZoneInfo.Utc.BaseUtcOffset)
            {
                result = frame.ToDateTime(DateTimeKind.Utc);
            }
            else if (utcOffset == TimeZoneInfo.Local.BaseUtcOffset)
            {
                result = frame.ToDateTime(DateTimeKind.Local);
            }
            else
            {
                result = frame.ToDateTime(DateTimeKind.Utc).Subtract(utcOffset);
            }

            return true;
        }

        public static bool Parse(string text, out DateTimeOffset result)
        {
            API.Check(text, nameof(text));

            Frame frame = new Frame();

            if (!Parse(text, ref frame))
            {
                result = default(DateTimeOffset);

                return false;
            }

            result = frame.ToDateTimeOffset();

            return true;
        }

        private static DateTimeOffset RoundDateTime(DateTime value)
        {
            TimeSpan utcOffset = TimeZoneInfo.Local.BaseUtcOffset;
            DateTime minDateTime = new DateTime(1, 1, 1, utcOffset.Hours, utcOffset.Minutes, utcOffset.Seconds, utcOffset.Milliseconds, value.Kind);

            return new DateTimeOffset(value < minDateTime ? minDateTime : value);
        }

        private static bool Parse(string text, ref Frame frame)
        {
            text = ExtraText.Trim(text);

            int level;

            switch (text.Length)
            {
                case 4: level = 1; break;
                case 7: level = 2; break;
                case 10: level = 3; break;
                case 13: level = 4; break;
                case 16: level = 5; break;
                case 19: level = 6; break;
                case 23: level = 7; break;
                case 24: level = 8; break;
                case 26: level = 8; break;
                case 29: level = 9; break;

                default:
                    return false;
            }

            for (int i = 0; i < level; i++)
            {
                if (!Parsers[i].Invoke(text, ref frame))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ParseYear(string text, ref Frame frame)
        {
            int n = ParseNumber(text, 0, 4);

            if (n <= 0)
            {
                return false;
            }

            frame.Year = n;

            return true;
        }

        private static bool ParseMonth(string text, ref Frame frame)
        {
            if (text[4] != '-')
            {
                return false;
            }

            int n = ParseNumber(text, 5, 2);

            if (n <= 0 || n > MaxMonth)
            {
                return false;
            }

            frame.Month = n;

            return true;
        }

        private static bool ParseDay(string text, ref Frame frame)
        {
            if (text[7] != '-')
            {
                return false;
            }

            int n = ParseNumber(text, 8, 2);

            if (n <= 0 || n > DateTime.DaysInMonth(frame.Year, frame.Month))
            {
                return false;
            }

            frame.Day = n;

            return true;
        }

        private static bool ParseHour(string text, ref Frame frame)
        {
            if (text[10] != 'T')
            {
                return false;
            }

            int n = ParseNumber(text, 11, 2);

            if (n == -1 || n > MaxHour)
            {
                return false;
            }

            frame.Hour = n;

            return true;
        }

        private static bool ParseMinute(string text, ref Frame frame)
        {
            if (text[13] != ':')
            {
                return false;
            }

            int n = ParseNumber(text, 14, 2);

            if (n == -1 || n > MaxMinute)
            {
                return false;
            }

            frame.Minute = n;

            return true;
        }

        private static bool ParseSecond(string text, ref Frame frame)
        {
            if (text[16] != ':')
            {
                return false;
            }

            int n = ParseNumber(text, 17, 2);

            if (n == -1 || n > MaxSecond)
            {
                return false;
            }

            frame.Second = n;

            return true;
        }

        private static bool ParseMillisecond(string text, ref Frame frame)
        {
            if (text[19] != '.')
            {
                return false;
            }

            int n = ParseNumber(text, 20, 3);

            if (n == -1)
            {
                return false;
            }

            frame.Millisecond = n;

            return true;
        }

        private static bool ParseUtcOffsetHour(string text, ref Frame frame)
        {
            char c = text[23];

            if (text.Length == 24)
            {
                return (c == 'Z');
            }

            if (c != '+' && c != '-')
            {
                return false;
            }

            int n = ParseNumber(text, 24, 2);

            if (n == -1 || n > MaxUtcOffsetHour || (c == '+' && frame.Leading && frame.Hour < n))
            {
                return false;
            }

            frame.UtcOffsetHour = (c == '+') ? n : -n;

            return true;
        }

        private static bool ParseUtcOffsetMinute(string text, ref Frame frame)
        {
            if (text[26] != ':')
            {
                return false;
            }

            int n = ParseNumber(text, 27, 2);

            if (n == -1 || n > MaxMinute || (frame.UtcOffsetHour == MaxUtcOffsetHour && n != 0) || (frame.Leading && frame.UtcOffsetHour == frame.Hour && frame.Minute < n))
            {
                return false;
            }

            frame.UtcOffsetMinute = n;

            return true;
        }

        private static int ParseNumber(string text, int index, int length)
        {
            if (ExtraText.ExpectDigits(text, index, (index + length) - 1) != length)
            {
                return -1;
            }

            switch (length)
            {
                case 1: return text[index] - 48;
                case 2: return ((text[index] - 48) * 10) + (text[index + 1] - 48);
                case 3: return ((text[index] - 48) * 100) + ((text[index + 1] - 48) * 10) + (text[index + 2] - 48);
                case 4: return ((text[index] - 48) * 1000) + ((text[index + 1] - 48) * 100) + ((text[index + 2] - 48) * 10) + (text[index + 3] - 48);
            }

            return 0;
        }

        private delegate bool Parser(string text, ref Frame frame);

        private struct Frame
        {
            public int Year;
            public int Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;
            public int Millisecond;
            public int UtcOffsetHour;
            public int UtcOffsetMinute;

            public bool Leading
            {
                get
                {
                    return (Year == 1 && Month == 1 && Day == 1);
                }
            }

            public DateTime ToDateTime(DateTimeKind kind)
            {
                return new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond, kind);
            }

            public DateTimeOffset ToDateTimeOffset()
            {
                return new DateTimeOffset(Year, Month, Day, Hour, Minute, Second, Millisecond, GetUtcOffset());
            }

            public TimeSpan GetUtcOffset()
            {
                return new TimeSpan(UtcOffsetHour, UtcOffsetMinute, 0);
            }
        }
    }
}
