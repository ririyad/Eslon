using System;
using System.IO;
using System.Text;

namespace Eslon
{
    /// <summary>
    /// Presents a thread-safe converter of periodic objects.
    /// </summary>
    public abstract class EslonEditor
    {
        private Type logo;
        private EslonReader reader;
        private EslonWriter writer;
        private PanInstanceChecker checker;
        private bool isNullEnabled;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonEditor"/>.
        /// </summary>
        /// <param name="logo">
        /// The type to assign.
        /// </param>
        /// <param name="reader">
        /// The reader to employ.
        /// </param>
        /// <param name="writer">
        /// The writer to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="logo"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="reader"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="logo"/> is not an eligible type.
        /// </exception>
        protected EslonEditor(Type logo, EslonReader reader, EslonWriter writer)
        {
            API.Check(logo, nameof(logo));
            API.Check(reader, nameof(reader));
            API.Check(writer, nameof(writer));

            if (logo.IsAbstract || logo.ContainsGenericParameters)
            {
                API.ThrowArgumentException("The type is not eligible.", nameof(logo));
            }

            this.logo = logo;
            this.reader = reader;
            this.writer = writer;
            this.checker = PanDynamic.CreateInstanceChecker(logo);
            this.isNullEnabled = BCL.IsNullableType(logo);
        }

        /// <summary>
        /// Gets the assigned type.
        /// </summary>
        /// <returns>
        /// The assigned type.
        /// </returns>
        public Type Logo
        {
            get { return logo; }
        }

        /// <summary>
        /// Gets the employed reader.
        /// </summary>
        /// <returns>
        /// The employed reader.
        /// </returns>
        public EslonReader Reader
        {
            get { return reader; }
        }

        /// <summary>
        /// Gets the employed writer.
        /// </summary>
        /// <returns>
        /// The employed writer.
        /// </returns>
        public EslonWriter Writer
        {
            get { return writer; }
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Object"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="Cast(EslonVolume, System.Object)"/>
        public object Read(string text)
        {
            return Read(text, null);
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Object"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="Cast(EslonVolume, System.Object)"/>
        public object Read(string text, object parent)
        {
            API.Check(text, nameof(text));

            return Cast(reader.Read(text), parent);
        }

        /// <summary>
        /// Writes the specified object to a new string.
        /// </summary>
        /// <param name="value">
        /// The object to write.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        /// <seealso cref="Digest(System.Object)"/>
        public string Write(object value)
        {
            return writer.Append(Digest(value), new StringBuilder(128)).ToString();
        }

        /// <summary>
        /// Writes the specified object.
        /// </summary>
        /// <param name="value">
        /// The object to write.
        /// </param>
        /// <param name="textWriter">
        /// The text writer to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="textWriter"/> is null.
        /// </exception>
        /// <seealso cref="Digest(System.Object)"/>
        public void Write(object value, TextWriter textWriter)
        {
            API.Check(textWriter, nameof(textWriter));

            writer.Write(Digest(value), textWriter, 0);
        }

        /// <summary>
        /// Casts an object from the specified volume.
        /// </summary>
        /// <param name="volume">
        /// The volume to convert.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Object"/>, or null if the specified volume is a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="volume"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="parent"/> is invalid.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        public object Cast(EslonVolume volume, object parent)
        {
            API.Check(volume, nameof(volume));

            if (parent != null && parent.GetType() != logo && !checker.Invoke(parent))
            {
                API.ThrowArgumentException("The parent is invalid.", nameof(parent));
            }

            if (volume.Symbol == EslonSymbol.Null)
            {
                if (!isNullEnabled)
                {
                    throw new EslonMoveException(volume, logo);
                }

                return null;
            }

            return AutoCast(volume, parent);
        }

        /// <summary>
        /// Converts the specified object to a volume.
        /// </summary>
        /// <param name="value">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonVolume"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <p>
        /// <paramref name="value"/> is null while the editor does not entail null values.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="value"/> is not compatible.
        /// </p>
        /// </exception>
        public EslonVolume Digest(object value)
        {
            if (value == null)
            {
                if (!isNullEnabled)
                {
                    API.ThrowArgumentException("The editor does not entail null values.", nameof(value));
                }

                return new EslonVoid();
            }

            if (value.GetType() != logo && !checker.Invoke(value))
            {
                API.ThrowArgumentException("The value is not compatible.", nameof(value));
            }

            return AutoDigest(value);
        }

        /// <summary>
        /// Casts an object from the specified volume.
        /// </summary>
        /// <param name="volume">
        /// The volume to convert.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Object"/>.
        /// </returns>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        protected abstract object AutoCast(EslonVolume volume, object parent);
        /// <summary>
        /// Converts the specified object to a volume.
        /// </summary>
        /// <param name="value">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonVolume"/>.
        /// </returns>
        protected abstract EslonVolume AutoDigest(object value);

        internal object DirectCast(EslonVolume volume, object parent)
        {
            return AutoCast(volume, parent);
        }

        internal EslonVolume DirectDigest(object value)
        {
            return AutoDigest(value);
        }
    }
}
