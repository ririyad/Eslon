using System.ComponentModel;

namespace Eslon
{
    /// <summary>
    /// Presents a periodic null object.
    /// </summary>
    public sealed class EslonVoid : EslonVolume
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EslonVoid"/>.
        /// </summary>
        public EslonVoid() { }

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
                return EslonSymbol.Null;
            }
        }

        /// <summary>
        /// Acquires a string instance of the null object.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        public override string ToString()
        {
            return Eslon.Null.ToString();
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
            return base.Move<TVolume>();
        }
    }
}
