using System.Globalization;
using System.IO;
using System.Text;

namespace Eslon
{
    /// <summary>
    /// Presents a thread-safe writer of periodic values.
    /// </summary>
    public class EslonWriter
    {
        private static readonly int[] EscapeHandles = new int[]
        {
            0x30, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x61, 0x62, 0x74, 0x6E, 0x76, 0x66, 0x72, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78,
            0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x01, 0x01, 0x22, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x00,
            0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
            0x5C, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78,
            0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78, 0x78
        };

        /// <summary>
        /// Initializes a new instance of <see cref="EslonWriter"/>.
        /// </summary>
        public EslonWriter() { }

        /// <summary>
        /// Writes the specified volume using a string builder.
        /// </summary>
        /// <param name="volume">
        /// The volume to write.
        /// </param>
        /// <param name="builder">
        /// The string builder to employ.
        /// </param>
        /// <returns>
        /// The specified string builder.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="volume"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="builder"/> is null.
        /// </p>
        /// </exception>
        public StringBuilder Append(EslonVolume volume, StringBuilder builder)
        {
            API.Check(volume, nameof(volume));
            API.Check(builder, nameof(builder));

            using (StringWriter writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                Write(volume, writer, 0);
            }

            return builder;
        }

        /// <summary>
        /// Writes the specified volume.
        /// </summary>
        /// <param name="volume">
        /// The volume to write.
        /// </param>
        /// <param name="writer">
        /// The text writer to employ.
        /// </param>
        /// <param name="level">
        /// The level of the writing operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="volume"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        public void Write(EslonVolume volume, TextWriter writer, int level)
        {
            API.Check(volume, nameof(volume));
            API.Check(writer, nameof(writer));

            switch (volume.Symbol)
            {
                case EslonSymbol.Map:
                    WriteMap((EslonMap)volume, writer, level + 1);
                    break;

                case EslonSymbol.List:
                    WriteList((EslonList)volume, writer, level + 1);
                    break;

                case EslonSymbol.String:
                    WriteString((EslonDisk)volume, writer);
                    break;

                default:
                    WriteNull(writer);
                    break;
            }
        }

        /// <summary>
        /// Writes the specified map.
        /// </summary>
        /// <param name="map">
        /// The map to write.
        /// </param>
        /// <param name="writer">
        /// The text writer to employ.
        /// </param>
        /// <param name="level">
        /// The level of the writing operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="map"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        public virtual void WriteMap(EslonMap map, TextWriter writer, int level)
        {
            API.Check(map, nameof(map));
            API.Check(writer, nameof(writer));

            writer.Write(Eslon.OpenMap);

            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                if (node != map.Begin)
                {
                    writer.Write(Eslon.Next);
                }

                writer.Write(node.Key);
                writer.Write(Eslon.Value);
                Write(node.Value, writer, level);
            }

            writer.Write(Eslon.CloseMap);
        }

        /// <summary>
        /// Writes the specified list.
        /// </summary>
        /// <param name="list">
        /// The list to write.
        /// </param>
        /// <param name="writer">
        /// The text writer to employ.
        /// </param>
        /// <param name="level">
        /// The level of the writing operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="list"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        public virtual void WriteList(EslonList list, TextWriter writer, int level)
        {
            API.Check(list, nameof(list));
            API.Check(writer, nameof(writer));

            writer.Write(Eslon.OpenList);

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    writer.Write(Eslon.Next);
                }

                Write(list.Code[i], writer, level);
            }

            writer.Write(Eslon.CloseList);
        }

        /// <summary>
        /// Writes the specified disk.
        /// </summary>
        /// <param name="disk">
        /// The disk to write.
        /// </param>
        /// <param name="writer">
        /// The text writer to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="disk"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        public virtual void WriteString(EslonDisk disk, TextWriter writer)
        {
            API.Check(disk, nameof(disk));
            API.Check(writer, nameof(writer));

            switch (disk.Mode)
            {
                case EslonDiskMode.Absolute:
                    writer.Write(disk.Value);
                    break;

                case EslonDiskMode.Cosmetic:
                    writer.Write(Eslon.Embed);
                    writer.Write(disk.Value);
                    writer.Write(Eslon.Embed);

                    break;

                default:
                    if (disk.Value.Length != 0)
                    {
                        Escape(disk.Value, writer, (disk.Mode == EslonDiskMode.Auto));
                    }
                    else
                    {
                        writer.Write(Eslon.Embed);
                        writer.Write(Eslon.Embed);
                    }

                    break;
            }
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        /// <param name="writer">
        /// The text writer to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="writer"/> is null.
        /// </exception>
        public virtual void WriteNull(TextWriter writer)
        {
            API.Check(writer, nameof(writer));

            writer.Write(Eslon.Null);
        }

        private static void Escape(string str, TextWriter writer, bool embed)
        {
            int offset = 0;
            int length = str.Length;

            for (int i = 0; i < length; i++)
            {
                int n;

                if ((n = str[i]) < 161 && (n = EscapeHandles[n]) != 0)
                {
                    embed = true;

                    if (n != 1)
                    {
                        if (offset == 0)
                        {
                            writer.Write(Eslon.Embed);
                        }

                        writer.Write(str.Substring(offset, i - offset));
                        writer.Write(Eslon.Escape);
                        writer.Write((char)n);

                        if (n == 120)
                        {
                            writer.Write(ExtraCode.EmitHexString((byte)str[i]));
                        }

                        offset = i + 1;
                    }
                }
            }

            if (embed && offset == 0)
            {
                writer.Write(Eslon.Embed);
            }

            writer.Write(str.Substring(offset, length - offset));

            if (embed)
            {
                writer.Write(Eslon.Embed);
            }
        }
    }
}
