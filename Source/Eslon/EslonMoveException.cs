using System;

namespace Eslon
{
    /// <summary>
    /// The exception that is thrown upon failing to convert an instance of <see cref="EslonVolume"/>.
    /// </summary>
    public class EslonMoveException : Exception
    {
        private EslonVolume mute;
        private Type cast;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonMoveException"/>.
        /// </summary>
        /// <param name="mute">
        /// The volume that causes the exception.
        /// </param>
        /// <param name="cast">
        /// The type that failed to cast.
        /// </param>
        public EslonMoveException(EslonVolume mute, Type cast) : this(mute, cast, null, null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonMoveException"/>.
        /// </summary>
        /// <param name="mute">
        /// The volume that causes the exception.
        /// </param>
        /// <param name="cast">
        /// The type that failed to cast.
        /// </param>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        public EslonMoveException(EslonVolume mute, Type cast, string message) : this(mute, cast, message, null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonMoveException"/>.
        /// </summary>
        /// <param name="mute">
        /// The volume that causes the exception.
        /// </param>
        /// <param name="cast">
        /// The type that failed to cast.
        /// </param>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that causes the current exception.
        /// </param>
        public EslonMoveException(EslonVolume mute, Type cast, string message, Exception innerException) : base(Express(mute, cast, message), innerException)
        {
            this.mute = mute;
            this.cast = cast;
        }

        /// <summary>
        /// Gets the volume that caused the exception.
        /// </summary>
        /// <returns>
        /// The volume that caused the exception.
        /// </returns>
        public EslonVolume Mute
        {
            get { return mute; }
        }

        /// <summary>
        /// Gets the type that failed to cast.
        /// </summary>
        /// <returns>
        /// The type that failed to cast.
        /// </returns>
        public Type Cast
        {
            get { return cast; }
        }

        private static string Express(EslonVolume mute, Type cast, string message)
        {
            return COM.Express("The conversion from a volume of '%' to type '%' failed." +
                (message == null || message.Length == 0 ? "" : "\r\n%"), mute?.Symbol.ToString(), cast?.FullName, message);
        }
    }
}
