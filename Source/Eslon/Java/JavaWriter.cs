using System.IO;

namespace Eslon.Java
{
    internal class JavaWriter : EslonWriter
    {
        private static readonly int[] EscapeHandles = new int[]
        {
            0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x62, 0x74, 0x6E, 0x75, 0x66, 0x72, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75,
            0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x01, 0x01, 0x22, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x00,
            0x00, 0x2F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
            0x5C, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75,
            0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75
        };

        public JavaWriter() { }

        public override void WriteMap(EslonMap map, TextWriter writer, int level)
        {
            API.Check(map, nameof(map));
            API.Check(writer, nameof(writer));

            writer.Write(Java.OpenMap);

            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                if (node != map.Begin)
                {
                    writer.Write(Java.Next);
                }

                Escape(node.Key, writer, true);
                writer.Write(Java.Value);
                Write(node.Value, writer, level);
            }

            writer.Write(Java.CloseMap);
        }

        public override void WriteList(EslonList list, TextWriter writer, int level)
        {
            API.Check(list, nameof(list));
            API.Check(writer, nameof(writer));

            writer.Write(Java.OpenList);

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    writer.Write(Java.Next);
                }

                Write(list.Code[i], writer, level);
            }

            writer.Write(Java.CloseList);
        }

        public override void WriteString(EslonDisk disk, TextWriter writer)
        {
            API.Check(disk, nameof(disk));
            API.Check(writer, nameof(writer));

            switch (disk.Mode)
            {
                case EslonDiskMode.Absolute:
                    writer.Write(disk.Value);
                    break;

                case EslonDiskMode.Cosmetic:
                    writer.Write(Java.Embed);
                    writer.Write(disk.Value);
                    writer.Write(Java.Embed);

                    break;

                default:
                    if (disk.Value.Length != 0)
                    {
                        Escape(disk.Value, writer, (disk.Mode == EslonDiskMode.Auto));
                    }
                    else
                    {
                        writer.Write(Java.Embed);
                        writer.Write(Java.Embed);
                    }

                    break;
            }
        }

        public override void WriteNull(TextWriter writer)
        {
            API.Check(writer, nameof(writer));

            writer.Write(Java.Null);
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
                            writer.Write(Java.Embed);
                        }

                        writer.Write(str.Substring(offset, i - offset));
                        writer.Write(Java.Escape);
                        writer.Write((char)n);

                        if (n == 117)
                        {
                            writer.Write(ExtraCode.EmitHexString(str[i]));
                        }

                        offset = i + 1;
                    }
                }
            }

            if (embed && offset == 0)
            {
                writer.Write(Java.Embed);
            }

            writer.Write(str.Substring(offset, length - offset));

            if (embed)
            {
                writer.Write(Java.Embed);
            }
        }
    }
}
