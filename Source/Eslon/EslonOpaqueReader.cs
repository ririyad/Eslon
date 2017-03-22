namespace Eslon
{
    internal class EslonOpaqueReader : EslonReader
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

        private static readonly int[] MapDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] ListDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] EmbedDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] EscapeHandles = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 39, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 92, 1, 1, 1, 1, 7, 8, 1,
            1, 1, 12, 1, 1, 1, 1, 1, 1, 1, 10, 1, 1, 1, 13, 1, 9, 4, 11, 1, 2
        };

        public EslonOpaqueReader() { }

        public override EslonVolume Read(string text)
        {
            API.Check(text, nameof(text));

            EslonVolume volume;

            using (PanReader reader = new PanStringReader(text))
            {
                if ((volume = Read(reader)) == null)
                {
                    text = ExtraText.Trim(text);

                    if (!ExtraRender.Scan(text, ValueDelimiters))
                    {
                        reader.Error(EslonReadExceptionCode.CorruptText, 0);
                    }

                    volume = new EslonDisk(EslonDiskMode.Express, text);
                }
            }

            return volume;
        }

        private EslonVolume Read(PanReader e)
        {
            EslonVolume volume = null;

            e.SkipBreaks();

            if ((volume = ReadElement(e, 0)) != null)
            {
                e.SkipBreaks();
                e.Dismiss();
            }

            return volume;
        }

        private EslonVolume ReadElement(PanReader e, int level)
        {
            switch (e.Peek())
            {
                case Eslon.Opaque: return ReadEmbed(e);
                case Eslon.OpenMap: return ReadMap(e, level + 1);
                case Eslon.OpenList: return ReadList(e, level + 1);
                case Eslon.Null: return ReadNull(e);
            }

            return null;
        }

        private EslonMap ReadMap(PanReader e, int level)
        {
            if (level == LevelCap)
            {
                e.Error(EslonReadExceptionCode.LevelCapExceeded, e.GetPos() + 1);
            }

            EslonMap map = new EslonMap();

            e.Advance();
            e.SkipSpace();

            if (e.Peek() == Eslon.CloseMap)
            {
                e.Advance();
            }
            else
            {
                while (true)
                {
                    string key = e.Consume(KeyDelimiters, true);

                    if (e.Peek() != Eslon.Value)
                    {
                        e.SkipSpace();

                        if (e.Peek() != Eslon.Value)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    if (map.ContainsKey(key))
                    {
                        e.Error(EslonReadExceptionCode.KeyExists, e.GetPos());
                    }

                    e.Advance();
                    e.SkipSpace();
                    map.Add(key, (ReadElement(e, level) ?? new EslonDisk(EslonDiskMode.Express, e.Consume(MapDelimiters, true))));

                    if (e.Peek() != Eslon.Next)
                    {
                        e.SkipSpace();

                        if (e.Peek() == Eslon.CloseMap)
                        {
                            e.Advance();

                            break;
                        }

                        if (e.Peek() != Eslon.Next)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    e.Advance();
                    e.SkipSpace();
                }
            }

            return map;
        }

        private EslonList ReadList(PanReader e, int level)
        {
            if (level == LevelCap)
            {
                e.Error(EslonReadExceptionCode.LevelCapExceeded, e.GetPos() + 1);
            }

            EslonList list = new EslonList();

            e.Advance();
            e.SkipSpace();

            if (e.Peek() == Eslon.CloseList)
            {
                e.Advance();
            }
            else
            {
                while (true)
                {
                    list.Add(ReadElement(e, level) ?? new EslonDisk(EslonDiskMode.Express, e.Consume(ListDelimiters, true)));

                    if (e.Peek() != Eslon.Next)
                    {
                        e.SkipSpace();

                        if (e.Peek() == Eslon.CloseList)
                        {
                            e.Advance();

                            break;
                        }

                        if (e.Peek() != Eslon.Next)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    e.Advance();
                    e.SkipSpace();
                }
            }

            return list;
        }

        private EslonDisk ReadEmbed(PanReader e)
        {
            string str = null;

            e.Advance();

            while (true)
            {
                str += e.Consume(EmbedDelimiters, false);

                if (e.Peek() == Eslon.Opaque)
                {
                    e.Advance();

                    return new EslonDisk(EslonDiskMode.Auto, str);
                }
                else if (e.Read() == Eslon.Escape)
                {
                    int n;

                    if ((n = e.Read()) < 121 && (n = EscapeHandles[n]) != 1)
                    {
                        str += (n == 2 || n == 4) ? ReadEscape(e, n) : (char)n;

                        continue;
                    }
                }

                e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos());
            }
        }

        private char ReadEscape(PanReader e, int length)
        {
            string str = new string(e.Arrange(length));

            if (ExtraCode.ExpectHex(str, 0, length - 1) != length)
            {
                e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos());
            }

            return ExtraCode.CastCharHex(str, 0, length - 1);
        }

        private EslonVoid ReadNull(PanReader e)
        {
            e.Advance();

            return new EslonVoid();
        }
    }
}
