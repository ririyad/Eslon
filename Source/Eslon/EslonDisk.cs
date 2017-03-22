using System.ComponentModel;

namespace Eslon
{
    /// <summary>
    /// Defines modes for disk writing.
    /// </summary>
    public enum EslonDiskMode
    {
        /// <summary>
        /// Instructs the writer to employ all escape functions.
        /// </summary>
        Auto,
        /// <summary>
        /// Instructs the writer to dismiss the value.
        /// </summary>
        Absolute,
        /// <summary>
        /// Instructs the writer to embed the value.
        /// </summary>
        Cosmetic,
        /// <summary>
        /// Instructs the writer to escape the value by trial.
        /// </summary>
        Express
    }

    /// <summary>
    /// Presents a periodic string.
    /// </summary>
    public sealed class EslonDisk : EslonVolume
    {
        private EslonDiskMode mode;
        private string value;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonDisk"/>.
        /// </summary>
        /// <param name="value">
        /// The value to assign.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public EslonDisk(string value)
        {
            API.Check(value, nameof(value));

            this.mode = EslonDiskMode.Auto;
            this.value = value;
        }

        internal EslonDisk(EslonDiskMode mode, string value)
        {
            this.mode = mode;
            this.value = value;
        }

        /// <summary>
        /// Gets the assigned mode.
        /// </summary>
        /// <returns>
        /// The assigned mode.
        /// </returns>
        public EslonDiskMode Mode
        {
            get { return mode; }
        }

        /// <summary>
        /// Gets the assigned value.
        /// </summary>
        /// <returns>
        /// The assigned value.
        /// </returns>
        public string Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the assigned symbol.
        /// </summary>
        /// <returns>
        /// The assigned symbol.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override EslonSymbol Symbol
        {
            get
            {
                return EslonSymbol.String;
            }
        }

        /// <summary>
        /// Creates a new disk of mode <see cref="EslonDiskMode.Absolute"/>.
        /// </summary>
        /// <param name="value">
        /// The value to assign.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonDisk"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static EslonDisk CreateAbsoluteDisk(string value)
        {
            API.Check(value, nameof(value));

            return new EslonDisk(EslonDiskMode.Absolute, value);
        }

        /// <summary>
        /// Creates a new disk of mode <see cref="EslonDiskMode.Cosmetic"/>.
        /// </summary>
        /// <param name="value">
        /// The value to assign.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonDisk"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static EslonDisk CreateCosmeticDisk(string value)
        {
            API.Check(value, nameof(value));

            return new EslonDisk(EslonDiskMode.Cosmetic, value);
        }

        /// <summary>
        /// Creates a new disk of mode <see cref="EslonDiskMode.Express"/>.
        /// </summary>
        /// <param name="value">
        /// The value to assign.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonDisk"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static EslonDisk CreateExpressDisk(string value)
        {
            API.Check(value, nameof(value));

            return new EslonDisk(EslonDiskMode.Express, value);
        }

        /// <summary>
        /// Converts the volume to an instance of <typeparamref name="TVolume"/>.
        /// </summary>
        /// <typeparam name="TVolume">
        /// The type to cast.
        /// </typeparam>
        /// <returns>
        /// An instance of <typeparamref name="TVolume"/>.
        /// </returns>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override TVolume Move<TVolume>()
        {
            return this as TVolume ?? base.Move<TVolume>();
        }
    }
}
