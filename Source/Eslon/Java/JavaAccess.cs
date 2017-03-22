using System.IO;

namespace Eslon.Java
{
    /// <summary>
    /// Presents a surrogate to <see cref="EslonEngine"/> using <see cref="JavaCanon"/>.
    /// </summary>
    public static class JavaAccess
    {
        private static object stub;
        private static EslonEngine _engine;

        static JavaAccess()
        {
            stub = new object();
        }

        /// <summary>
        /// Gets the employed engine.
        /// </summary>
        /// <returns>
        /// The employed engine.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value"/> contains an invalid reader.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The surrogate already employs an engine.
        /// </exception>
        public static EslonEngine Engine
        {
            get
            {
                if (_engine == null)
                {
                    Load();
                }

                return _engine;
            }

            set
            {
                if (value.Reader.GetType() != JavaCanon.Reader.GetType())
                {
                    API.ThrowArgumentException("The specified engine contains an invalid reader.", nameof(value));
                }

                lock (stub)
                {
                    if (_engine != null)
                    {
                        API.ThrowInvalidOperationException("The surrogate already employs an engine.");
                    }

                    _engine = value;
                }
            }
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEngine.Read{T}(System.String)"/>
        public static T Read<T>(string text)
        {
            return Engine.Read<T>(text);
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEngine.Read{T}(System.String, System.Object)"/>
        public static T Read<T>(string text, object parent)
        {
            return Engine.Read<T>(text, parent);
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
        /// <seealso cref="EslonEngine.Write(System.Object)"/>
        public static string Write(object value)
        {
            return Engine.Write(value);
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
        /// <seealso cref="EslonEngine.Write(System.Object, System.IO.TextWriter)"/>
        public static void Write(object value, TextWriter textWriter)
        {
            Engine.Write(value, textWriter);
        }

        /// <summary>
        /// Casts an object from the specified volume.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="volume">
        /// The volume to convert.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified volume is a null object.
        /// </returns>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEngine.Cast(EslonVolume, System.Object)"/>
        public static T Cast<T>(EslonVolume volume, object parent)
        {
            return Engine.Cast<T>(volume, parent);
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
        /// <seealso cref="EslonEngine.Digest(System.Object)"/>
        public static EslonVolume Digest(object value)
        {
            return Engine.Digest(value);
        }

        private static void Load()
        {
            lock (stub)
            {
                if (_engine == null)
                {
                    _engine = CreateEngine();
                }
            }
        }

        private static EslonEngine CreateEngine()
        {
            return new EslonEngine(JavaCanon.Reader, JavaCanon.Writer, JavaCanon.AugmentEditors(null), JavaCanon.AugmentReflectors(null), JavaCanon.AugmentConduits(null));
        }
    }
}
