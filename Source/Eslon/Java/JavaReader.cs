namespace Eslon.Java
{
    internal class JavaReader : EslonReader
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
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] MapDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] ListDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] EmbedDelimiters = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        private static readonly int[] EscapeHandles = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 34, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 47, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 92, 1, 1, 1, 1, 1, 8, 1,
            1, 1, 12, 1, 1, 1, 1, 1, 1, 1, 10, 1, 1, 1, 13, 1, 9, 4
        };

        public JavaReader() { }

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
                case Java.Embed: return new EslonDisk(EslonDiskMode.Auto, ReadEmbedString(e));
                case Java.OpenMap: return ReadMap(e, level + 1);
                case Java.OpenList: return ReadList(e, level + 1);
            }

            return e.Match(Java.Null) ? new EslonVoid() : null;
        }

        private EslonMap ReadMap(PanReader e, int level)
        {
            if (level == LevelCap)
            {
                e.Error(EslonReadExceptionCode.LevelCapExceeded, e.GetPos() + 1);
            }

            EslonMap map = new EslonMap();

            e.Advance();
            e.SkipBreaks();

            if (e.Peek() == Java.CloseMap)
            {
                e.Advance();
            }
            else
            {
                while (true)
                {
                    string key = (e.Peek() == Java.Embed) ? ReadEmbedString(e) : e.Consume(KeyDelimiters, true);

                    if (e.Peek() != Java.Value)
                    {
                        e.SkipBreaks();

                        if (e.Peek() != Java.Value)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    if (map.ContainsKey(key))
                    {
                        e.Error(EslonReadExceptionCode.KeyExists, e.GetPos());
                    }

                    e.Advance();
                    e.SkipBreaks();
                    map.Add(key, (ReadElement(e, level) ?? new EslonDisk(EslonDiskMode.Express, e.Consume(MapDelimiters, true))));

                    if (e.Peek() != Java.Next)
                    {
                        e.SkipBreaks();

                        if (e.Peek() == Java.CloseMap)
                        {
                            e.Advance();

                            break;
                        }

                        if (e.Peek() != Java.Next)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    e.Advance();
                    e.SkipBreaks();
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
            e.SkipBreaks();

            if (e.Peek() == Java.CloseList)
            {
                e.Advance();
            }
            else
            {
                while (true)
                {
                    list.Add(ReadElement(e, level) ?? new EslonDisk(EslonDiskMode.Express, e.Consume(ListDelimiters, true)));

                    if (e.Peek() != Java.Next)
                    {
                        e.SkipBreaks();

                        if (e.Peek() == Java.CloseList)
                        {
                            e.Advance();

                            break;
                        }

                        if (e.Peek() != Java.Next)
                        {
                            e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos() + 1);
                        }
                    }

                    e.Advance();
                    e.SkipBreaks();
                }
            }

            return list;
        }

        private string ReadEmbedString(PanReader e)
        {
            string str = null;

            e.Advance();

            while (true)
            {
                str += e.Consume(EmbedDelimiters, false);

                if (e.Peek() == Java.Embed)
                {
                    e.Advance();

                    return str;
                }
                else if (e.Read() == Java.Escape)
                {
                    int n;

                    if ((n = e.Read()) < 118 && (n = EscapeHandles[n]) != 1)
                    {
                        str += (n == 4) ? ReadEscape(e) : (char)n;

                        continue;
                    }
                }

                e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos());
            }
        }

        private char ReadEscape(PanReader e)
        {
            string str = new string(e.Arrange(4));

            if (ExtraCode.ExpectHex(str, 0, 3) != 4)
            {
                e.Error(EslonReadExceptionCode.SyntaxError, e.GetPos());
            }

            return ExtraCode.CastCharHex(str, 0, 3);
        }
    }
}
