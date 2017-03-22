using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Eslon
{
    internal class EslonBatchWriter
    {
        private static readonly int[] CollisionTable = new int[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1
        };

        private static readonly EslonComment End = new EslonComment("End");

        private EslonBatch batch;
        private EslonMap sectors;
        private Dictionary<EslonVolume, EslonComment> comments;
        private EslonWriter inlineWriter;

        public EslonBatchWriter(EslonBatch batch)
        {
            this.batch = batch;
            this.sectors = batch.Sectors.Core;
            this.comments = batch.Comments.Core;
            this.inlineWriter = new EslonWriter();
        }

        public void Write(StringBuilder builder)
        {
            using (StringWriter writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                Write(writer);
            }
        }

        public void Write(Stream stream, Encoding encoding, int bufferSize)
        {
            using (StreamWriter writer = new StreamWriter(stream, (encoding ?? Encoding.UTF8), (bufferSize == 0 ? 1024 : bufferSize)))
            {
                Write(writer);
            }
        }

        private static string GetAssemblyName()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                AssemblyName name = assembly.GetName();
                string companyName = ExtraReflection.GetAssemblyCompanyName(assembly);

                return ExtraRender.Detonate("% (%)" + (companyName == null || companyName.Length == 0 ? "" : ", %"), name.Name, name.Version.ToString(), companyName);
            }

            return null;
        }

        private static bool Collide(char c)
        {
            return (c < 65 && CollisionTable[c] == 1);
        }

        private void Write(TextWriter e)
        {
            bool flag = false;

            if (batch.Descriptive)
            {
                WriteDescriptor(e);
                flag = true;
            }

            for (EslonMapNode node = sectors.Begin; node != null; node = node.Next)
            {
                if (flag)
                {
                    Enter(e);
                    Enter(e);
                }

                EslonComment comment;

                if (comments.TryGetValue(node.Value, out comment))
                {
                    WriteComment(e, comment);
                    Enter(e);
                }

                if (node.Value.Symbol == EslonSymbol.Map)
                {
                    e.Write('@');
                    e.Write('\x20');
                    e.Write(node.Key);
                    WriteMap(e, (EslonMap)node.Value);
                }
                else
                {
                    e.Write('+');
                    e.Write('\x20');
                    e.Write(node.Key);
                    WriteList(e, (EslonList)node.Value);
                }

                flag = true;
            }

            if (flag)
            {
                Enter(e);
                Enter(e);
            }

            WriteComment(e, (batch.Poster ?? End));
        }

        private void WriteMap(TextWriter e, EslonMap map)
        {
            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                EslonComment comment;

                if (comments.TryGetValue(node.Value, out comment))
                {
                    Enter(e);
                    WriteComment(e, comment);
                }

                Enter(e);
                e.Write('\x20');
                e.Write('\x20');
                e.Write(node.Key);
                e.Write(" = ");
                inlineWriter.Write(node.Value, e, 0);
            }
        }

        private void WriteList(TextWriter e, EslonList list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                EslonVolume item = list.Code[i];
                EslonDisk disk = (item as EslonDisk);
                EslonComment comment;

                if (comments.TryGetValue(item, out comment))
                {
                    Enter(e);
                    WriteComment(e, comment);
                }

                Enter(e);
                e.Write('\x20');
                e.Write('\x20');

                if (disk != null && disk.Value.Length != 0 && Collide(disk.Value[0]))
                {
                    inlineWriter.WriteString(new EslonDisk(EslonDiskMode.Auto, disk.Value), e);
                }
                else
                {
                    inlineWriter.Write(item, e, 0);
                }
            }
        }

        private void WriteDescriptor(TextWriter e)
        {
            e.Write("^ Eslon Batch");
            Enter(e);
            Enter(e);
            e.Write("Assembly: ");
            e.Write(GetAssemblyName() ?? "<none>");

            if (sectors.Begin != null)
            {
                Enter(e);

                for (EslonMapNode node = sectors.Begin; node != null; node = node.Next)
                {
                    Enter(e);
                    e.Write(node.Value.Symbol == EslonSymbol.Map ? '@' : '+');
                    e.Write('\x20');
                    e.Write(node.Key);
                }
            }

            if (batch.Header != null)
            {
                WriteHeader(e, batch.Header);
            }

            Enter(e);
            e.Write('^');
        }

        private void WriteHeader(TextWriter e, EslonComment comment)
        {
            int count = comment.Code.Count;
            bool flag = true;

            for (int i = 0; i < count; i++)
            {
                string item = comment.Code[i];

                if (item.Length == 0)
                {
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        Enter(e);
                        flag = false;
                    }

                    Enter(e);
                    e.Write('#');
                    e.Write('\x20');
                    e.Write(item);
                }
            }
        }

        private void WriteComment(TextWriter e, EslonComment comment)
        {
            int count = comment.Code.Count;

            for (int i = 0; i < count; i++)
            {
                string item = comment.Code[i];

                if (i != 0)
                {
                    Enter(e);
                }

                e.Write('#');

                if (item.Length != 0)
                {
                    e.Write('\x20');
                    e.Write(item);
                }
            }
        }

        private void Enter(TextWriter e)
        {
            e.Write('\r');
            e.Write('\n');
        }
    }
}
