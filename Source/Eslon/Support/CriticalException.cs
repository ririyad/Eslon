using System;

namespace Eslon
{
    internal class CriticalException : Exception
    {
        public CriticalException() { }

        public CriticalException(string message) : base(message) { }

        public CriticalException(string message, Exception innerException) : base(message, innerException) { }
    }
}
