using System.Diagnostics;
using System.Text;

namespace Eslon
{
    /// <summary>
    /// Presents the base class of periodic objects.
    /// </summary>
    /// <seealso cref="EslonMap"/>
    /// <seealso cref="EslonList"/>
    /// <seealso cref="EslonDisk"/>
    /// <seealso cref="EslonVoid"/>
    public abstract class EslonVolume
    {
        internal EslonVolume() { }

        /// <summary>
        /// Gets the assigned symbol.
        /// </summary>
        /// <returns>
        /// The assigned symbol.
        /// </returns>
        public abstract EslonSymbol Symbol { get; }

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
        [DebuggerHidden]
        public virtual TVolume Move<TVolume>() where TVolume : EslonVolume
        {
            throw new EslonMoveException(this, typeof(TVolume));
        }

        /// <summary>
        /// Acquires a string instance of the current volume.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        public override string ToString()
        {
            return new EslonWriter().Append(this, new StringBuilder(128)).ToString();
        }
    }
}
