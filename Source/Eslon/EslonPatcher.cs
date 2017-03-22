using System;
using System.Collections.Generic;

namespace Eslon
{
    /// <summary>
    /// Presents an implementation of <see cref="EslonConduit"/> to append missing map values.
    /// </summary>
    public sealed class EslonPatcher : EslonConduit
    {
        private KeyValuePair<string, EslonVolume>[] selection;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonPatcher"/>.
        /// </summary>
        /// <param name="logo">
        /// The type to assign.
        /// </param>
        /// <param name="opaqueString">
        /// The opaque string to read.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="opaqueString"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        public EslonPatcher(Type logo, string opaqueString) : base(logo)
        {
            API.Check(opaqueString, nameof(opaqueString));

            this.selection = new EslonOpaqueReader().Read(opaqueString).Move<EslonMap>().ToArray();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonPatcher"/>.
        /// </summary>
        /// <param name="logo">
        /// The type to assign.
        /// </param>
        /// <param name="map">
        /// The map to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="map"/> is null.
        /// </exception>
        public EslonPatcher(Type logo, EslonMap map) : base(logo)
        {
            API.Check(map, nameof(map));

            this.selection = map.ToArray();
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
        protected override void Elect(EslonEditor editor, ref EslonVolume volume)
        {
            EslonMap map = volume.Move<EslonMap>();

            foreach (KeyValuePair<string, EslonVolume> item in selection)
            {
                if (!map.ContainsKey(item.Key))
                {
                    map.Add(item.Key, item.Value);
                }
            }
        }
    }
}
