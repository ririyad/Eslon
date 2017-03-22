using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eslon
{
    internal class EslonBatchReader
    {
        private static readonly int[] ValueDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] KeyDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] LineDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private EslonBatch batch;
        private EslonMap sectors;
        private Dictionary<EslonVolume, EslonComment> comments;
        private HashSet<string> propertyRegistry;
        private HashSet<string> sectorRegistry;
        private EslonReader inlineReader;

        public EslonBatchReader(EslonBatch batch)
        {
            this.batch = batch;
            this.sectors = batch.Sectors.Core;
            this.comments = batch.Comments.Core;
            this.propertyRegistry = new HashSet<string>(StringComparer.Ordinal);
            this.sectorRegistry = new HashSet<string>(StringComparer.Ordinal);
            this.inlineReader = new EslonReader();
        }

        public void Read(string text)
        {
            using (PanStringReader reader = new PanStringReader(text))
            {
                Read(reader);
            }
        }

        public void Read(Stream stream, Encoding encoding, bool detectEncoding, int bufferSize)
        {
            using (PanStreamReader reader = new PanStreamReader(stream, (encoding ?? Encoding.UTF8), detectEncoding, (bufferSize == 0 ? 4096 : bufferSize)))
            {
                Read(reader);
            }
        }

        private void Read(PanReader reader)
        {
            if (batch.Strict)
            {
                comments.Clear();
            }

            try
            {
                ReadBatch(reader);
            }
            finally
            {
                propertyRegistry.Clear();
                sectorRegistry.Clear();
            }
        }

        private void ReadBatch(PanReader e)
        {
            EslonComment comment = null;
            EslonVolume sector = null;
            EslonMap map = null;
            bool flag = false;

            if (!e.Depleted && e.Peek() == '^')
            {
                ReadDescriptor(e);
                flag = true;
            }
            else
            {
                e.SkipBreaks();
            }

            while (!e.Depleted)
            {
                if (flag)
                {
                    e.Admit();
                    e.SkipBreaks();
                    e.Test();

                    if (e.Depleted)
                    {
                        break;
                    }
                }

                flag = true;

                switch (e.Peek())
                {
                    case '@':
                        e.Advance();
                        e.SkipSpace();
                        EnterSector(e.Consume(ValueDelimiters, true), true, e, ref comment, ref sector);
                        map = (EslonMap)sector;
                        e.SkipSpace();

                        continue;

                    case '+':
                        e.Advance();
                        e.SkipSpace();
                        EnterSector(e.Consume(ValueDelimiters, true), false, e, ref comment, ref sector);
                        e.SkipSpace();

                        continue;

                    case '#':
                        e.Advance();

                        if (!e.Depleted && e.Peek() == '\x20')
                        {
                            e.Advance();
                        }

                        (comment ?? (comment = new EslonComment())).Code.Add(e.Consume(LineDelimiters, false));

                        continue;
                }

                if (sector == null)
                {
                    e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                }

                EslonVolume volume;

                if (sector.Symbol == EslonSymbol.List)
                {
                    volume = ReadValue(e);
                    ((EslonList)sector).Add(volume);
                }
                else
                {
                    string key = e.Consume(KeyDelimiters, true);

                    if (e.Peek() != '=')
                    {
                        e.SkipSpace();

                        if (e.Peek() != '=')
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    e.Advance();

                    if (map.TryGetValue(key, out volume))
                    {
                        if (!propertyRegistry.Add(key))
                        {
                            e.Error(ExceptionCode.RepetitiveEntry, e.GetPos());
                        }

                        if (!batch.Strict)
                        {
                            comments.Remove(volume);
                        }

                        e.SkipSpace();
                        volume = ReadValue(e);
                        map[key] = volume;
                    }
                    else
                    {
                        if (batch.Strict)
                        {
                            e.Error(ExceptionCode.UnauthorizedEntry, e.GetPos());
                        }

                        e.SkipSpace();
                        volume = ReadValue(e);
                        map.Add(key, volume);
                        propertyRegistry.Add(key);
                    }
                }

                if (comment != null)
                {
                    comments.Add(volume, comment);
                    comment = null;
                }
            }

            if (sector != null && sector.Symbol == EslonSymbol.Map)
            {
                Decline(map, e);
            }

            if (batch.Strict && sectorRegistry.Count != sectors.Count)
            {
                e.Error(ExceptionCode.BatchAbortion, e.GetPos());
            }

            batch.Poster = comment;
        }

        private EslonVolume ReadValue(PanReader e)
        {
            EslonVolume volume = inlineReader.Read(e, true);

            if (volume == null)
            {
                volume = new EslonDisk(EslonDiskMode.Express, e.Consume(ValueDelimiters, false));
                e.SkipSpace();
            }

            return volume;
        }

        private void ReadDescriptor(PanReader e)
        {
            while (true)
            {
                e.Skip(LineDelimiters);
                e.Admit();
                e.AssumeNewline();
                e.Test();

                if (e.Peek() == '^')
                {
                    e.Advance();
                    e.SkipSpace();

                    break;
                }
            }
        }

        private void EnterSector(string key, bool flag, PanReader reader, ref EslonComment comment, ref EslonVolume sector)
        {
            if (sector != null && sector.Symbol == EslonSymbol.Map)
            {
                Decline((EslonMap)sector, reader);
            }

            if (sectors.TryGetValue(key, out sector))
            {
                if (sector.Symbol == EslonSymbol.List ? flag : !flag)
                {
                    reader.Error(ExceptionCode.VolumeMismatch, reader.GetPos());
                }

                if (!sectorRegistry.Add(key))
                {
                    reader.Error(ExceptionCode.RepetitiveEntry, reader.GetPos());
                }

                if (!flag)
                {
                    Incline((EslonList)sector);
                }

                if (!batch.Strict)
                {
                    comments.Remove(sector);
                }
            }
            else
            {
                if (batch.Strict)
                {
                    reader.Error(ExceptionCode.UnauthorizedEntry, reader.GetPos());
                }

                sector = (flag ? (EslonVolume)new EslonMap() : new EslonList());
                sectors.Add(key, sector);
                sectorRegistry.Add(key);
            }

            if (comment != null)
            {
                comments.Add(sector, comment);
                comment = null;
            }
        }

        private void Decline(EslonMap map, PanReader reader)
        {
            if (batch.Strict && propertyRegistry.Count != map.Count)
            {
                reader.Error(ExceptionCode.MapAbortion, reader.GetPos());
            }

            propertyRegistry.Clear();
        }

        private void Incline(EslonList list)
        {
            if (!batch.Strict)
            {
                DeleteComments(list);
            }

            list.Clear();
        }

        private void DeleteComments(EslonList list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                comments.Remove(list.Code[i]);
            }
        }

        private enum ExceptionCode
        {
            BatchAbortion,
            MapAbortion,
            RepetitiveEntry,
            UnauthorizedEntry,
            VolumeMismatch
        }
    }
}
