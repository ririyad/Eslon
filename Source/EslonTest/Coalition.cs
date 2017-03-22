using System;
using System.Collections.Generic;

namespace EslonTest
{
    class Coalition
    {
        public Coalition_Main Main = new Coalition_Main();
        public Coalition_Aux Aux = new Coalition_Aux();
        public Coalition_Enums Enums = new Coalition_Enums();
        public Coalition_Collections Collections = new Coalition_Collections();
    }

    class Coalition_Main
    {
        public string @String = "\0\a\b\t\n\v\f\n\"\\\xA0\uFFFD";
        public bool @Boolean = false;
        public char @Char = '\0';
        public byte @Byte = Byte.MaxValue;
        public sbyte @SByte = SByte.MaxValue;
        public short @Int16 = Int16.MaxValue;
        public ushort @UInt16 = UInt16.MaxValue;
        public int @Int32 = Int32.MaxValue;
        public uint @UInt32 = UInt32.MaxValue;
        public long @Int64 = Int64.MaxValue;
        public ulong @UInt64 = UInt64.MaxValue;
        public float @Single = Single.MaxValue;
        public double @Double = Double.MaxValue;
        public decimal @Decimal = Decimal.MaxValue;
        public DateTime @DateTime = DateTime.MaxValue;
        public DateTimeOffset @DateTimeOffset = DateTimeOffset.MaxValue;
        public TimeSpan @TimeSpan = TimeSpan.MaxValue;
        public Guid @Guid = Guid.NewGuid();
        public Uri @Uri = new Uri("http://www.microsoft.com/");
        public byte[] @ByteArray = new byte[] { 0x00, 0xFF, 0x00 };
    }

    class Coalition_Aux
    {
        public ulong? @Nullable = null;
        public double @NaN = Double.NaN;
        public double @Inf = Double.PositiveInfinity;
        public double @NInf = Double.NegativeInfinity;
    }

    class Coalition_Enums
    {
        public Enum1 @Enum1 = (Enum1)UInt64.MaxValue;
        public Enum2 @Enum2 = (Enum2)Int16.MaxValue;
        public Enum3 @Enum3 = Enum3.A | Enum3.B | Enum3.C | Enum3.D;
    }

    class Coalition_Collections
    {
        public int[] @Array = new int[] { 1, 2, 3 };
        public int[,] @MultiArray = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };

        public Dictionary<string, int> @Dictionary = new Dictionary<string, int>()
        {
            { "A", 1 },
            { "B", 2 },
            { "C", 3 }
        };

        public List<int> @List = new List<int>()
        {
            1, 2, 3
        };

        public Collection<int> @Collection = new Collection<int>()
        {
            1, 2, 3
        };

        public Enumerable<int> @Enumerable = new Enumerable<int>()
        {
            1, 2, 3
        };
    }

    enum Enum1 : ulong
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3
    }

    enum Enum2 : short
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3
    }

    [Flags]
    enum Enum3 : uint
    {
        A = 1,
        B = 2,
        C = 4,
        D = 8
    }
}
