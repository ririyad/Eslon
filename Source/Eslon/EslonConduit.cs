using System;

namespace Eslon
{
    /// <summary>
    /// Presents an engine module to modify volumes prior to casting.
    /// </summary>
    /// <seealso cref="EslonEngine"/>
    public abstract class EslonConduit : EslonEngineModule
    {
        private Type logo;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonConduit"/>.
        /// </summary>
        /// <param name="logo">
        /// The type to assign.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="logo"/> is null.
        /// </exception>
        protected EslonConduit(Type logo)
        {
            API.Check(logo, nameof(logo));

            this.logo = logo;
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
        /// Modifies the specified volume.
        /// </summary>
        /// <param name="editor">
        /// The editor that attends the specified volume.
        /// </param>
        /// <param name="volume">
        /// Contains the volume to modify.
        /// </param>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        protected abstract void Elect(EslonEditor editor, ref EslonVolume volume);

        internal void Generate(EslonEditor editor, ref EslonVolume volume)
        {
            EslonVolume process = volume;

            Elect(editor, ref process);

            if (process != null)
            {
                volume = process;
            }
        }
    }
}
