using System;

namespace Eslon
{
    /// <summary>
    /// The exception that is thrown upon failing to read a periodic value.
    /// </summary>
    public class EslonReadException : Exception
    {
        private Enum code;
        private long position;
        private int line;
        private long column;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonReadException"/>.
        /// </summary>
        /// <param name="code">
        /// The code that identifies the exception.
        /// </param>
        /// <param name="position">
        /// The position where reading failed.
        /// </param>
        /// <param name="line">
        /// The line where reading failed.
        /// </param>
        /// <param name="column">
        /// The column where reading failed.
        /// </param>
        public EslonReadException(Enum code, long position, int line, long column)
            : base(COM.Express("Code: %\r\nPosition: %\r\nLine: %\r\nColumn: %", (code == null ? "<null>" : code.ToString()), position, line, column))
        {
            this.code = code;
            this.position = position;
            this.line = line;
            this.column = column;
        }

        /// <summary>
        /// Gets the code that identifies the exception.
        /// </summary>
        /// <returns>
        /// The code that identifies the exception.
        /// </returns>
        public Enum Code
        {
            get { return code; }
        }

        /// <summary>
        /// Gets the position where reading failed.
        /// </summary>
        /// <returns>
        /// The position where reading failed.
        /// </returns>
        public long Position
        {
            get { return position; }
        }

        /// <summary>
        /// Gets the line where reading failed.
        /// </summary>
        /// <returns>
        /// The line where reading failed.
        /// </returns>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets the column where reading failed.
        /// </summary>
        /// <returns>
        /// The column where reading failed.
        /// </returns>
        public long Column
        {
            get { return column; }
        }
    }
}
