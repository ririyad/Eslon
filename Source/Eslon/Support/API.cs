using System;
using System.Diagnostics;

namespace Eslon
{
    internal static class API
    {
        [DebuggerHidden]
        public static void Check(object value, string name)
        {
            if (value == null)
            {
                ThrowArgumentNullException(name);
            }
        }

        [DebuggerHidden]
        public static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        [DebuggerHidden]
        public static void ThrowArgumentOutOfRangeException(string name)
        {
            throw new ArgumentOutOfRangeException(name);
        }

        [DebuggerHidden]
        public static void ThrowArgumentOutOfRangeException(string name, string message)
        {
            throw new ArgumentOutOfRangeException(name, message);
        }

        [DebuggerHidden]
        public static void ThrowArgumentNullException(string name)
        {
            throw new ArgumentNullException(name);
        }

        [DebuggerHidden]
        public static void ThrowArgumentNullException(string name, string message)
        {
            throw new ArgumentNullException(name, message);
        }

        [DebuggerHidden]
        public static void ThrowArgumentException(string message)
        {
            throw new ArgumentException(message);
        }

        [DebuggerHidden]
        public static void ThrowArgumentException(string message, string name)
        {
            throw new ArgumentException(message, name);
        }

        [DebuggerHidden]
        public static void ThrowFormatException()
        {
            throw new FormatException();
        }

        [DebuggerHidden]
        public static void ThrowFormatException(string message)
        {
            throw new FormatException(message);
        }
    }
}
