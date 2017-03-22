using System;
using System.Collections;
using System.Collections.Generic;

namespace Eslon
{
    /// <summary>
    /// Presents a comment to associate with batch values.
    /// </summary>
    /// <seealso cref="EslonBatch"/>
    public sealed class EslonComment : IEnumerable<string>
    {
        private List<string> code;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonComment"/>.
        /// </summary>
        /// <param name="text">
        /// The string to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="System.FormatException">
        /// <paramref name="text"/> contains an invalid character.
        /// </exception>
        public EslonComment(string text) : this()
        {
            API.Check(text, nameof(text));

            Load(text);
        }

        internal EslonComment()
        {
            this.code = new List<string>(1);
        }

        internal List<string> Code
        {
            get { return code; }
        }

        /// <summary>
        /// Acquires an enumerator for the comment.
        /// </summary>
        /// <returns>
        /// An enumerator for the comment.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return code.GetEnumerator();
        }

        /// <summary>
        /// Copies the comment to a new array.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public string[] ToArray()
        {
            return code.ToArray();
        }

        /// <summary>
        /// Acquires a string instance of the comment.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        public override string ToString()
        {
            return (code.Count == 1) ? code[0] : String.Join(Environment.NewLine, code);
        }

        private void Load(string str)
        {
            int offset = 0;
            int end = str.Length - 1;

            for (int i = 0; i <= end; i++)
            {
                char c = str[i];

                if (c == '\n')
                {
                    LoadPart(str.Substring(offset, i - offset));
                    offset = i + 1;
                }
                else if (c == '\r' && i < end && str[i + 1] == '\n')
                {
                    LoadPart(str.Substring(offset, i - offset));
                    offset = i + 2;
                    i++;
                }
                else if (ExtraText.IsControl(c))
                {
                    API.ThrowFormatException("The text contains an invalid character.");
                }
            }

            LoadPart(str.Substring(offset, str.Length - offset));
        }

        private void LoadPart(string part)
        {
            code.Add(ExtraText.TrimRight(part));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return code.GetEnumerator();
        }
    }
}
