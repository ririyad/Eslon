using System;
using System.Globalization;

namespace Eslon.Java
{
    /// <summary>
    /// Presents an implementation of JavaScript Object Notation.
    /// </summary>
    public static class JavaCanon
    {
        /// <summary>
        /// Gets a JSON reader.
        /// </summary>
        /// <returns>
        /// A JSON reader.
        /// </returns>
        public static EslonReader Reader { get; } = new JavaReader();

        /// <summary>
        /// Gets a JSON writer.
        /// </summary>
        /// <returns>
        /// A JSON writer.
        /// </returns>
        public static EslonWriter Writer { get; } = new JavaWriter();

        private static EslonEditor[] Editors { get; } = new EslonEditor[]
        {
            new Editor_String(),
            new Editor_Boolean(),
            new Editor_Char(),
            new Editor_Byte(),
            new Editor_SByte(),
            new Editor_Int16(),
            new Editor_UInt16(),
            new Editor_Int32(),
            new Editor_UInt32(),
            new Editor_Int64(),
            new Editor_UInt64(),
            new Editor_Single(),
            new Editor_Double(),
            new Editor_Decimal(),
            new Editor_DateTime(),
            new Editor_DateTimeOffset(),
            new Editor_TimeSpan(),
            new Editor_Guid(),
            new Editor_Uri(),
            new Editor_ByteArray()
        };

        /// <summary>
        /// Creates a new array that includes a collection of JSON editors.
        /// </summary>
        /// <param name="array">
        /// The array to copy.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public static EslonEditor[] AugmentEditors(EslonEditor[] array)
        {
            if (array == null)
            {
                array = BCL.Empty<EslonEditor>();
            }

            Array.Resize(ref array, array.Length + Editors.Length);
            Array.Copy(array, 0, array, Editors.Length, array.Length - Editors.Length);
            Editors.CopyTo(array, 0);

            return array;
        }

        /// <summary>
        /// Creates a new array that includes a JSON reflector.
        /// </summary>
        /// <param name="array">
        /// The array to copy.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public static EslonReflector[] AugmentReflectors(EslonReflector[] array)
        {
            if (array == null)
            {
                array = BCL.Empty<EslonReflector>();
            }

            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = new JavaCanonReflector();

            return array;
        }

        /// <summary>
        /// Creates a new array that includes a collection of JSON conduits.
        /// </summary>
        /// <param name="array">
        /// The array to copy.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public static EslonConduit[] AugmentConduits(EslonConduit[] array)
        {
            return (array == null) ? BCL.Empty<EslonConduit>() : Extra.DupeArray(array);
        }

        /// <summary>
        /// Acquires an editor for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonEditor"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="type"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="type"/> does not yield an editor.
        /// </exception>
        public static EslonEditor GetEditor(Type type)
        {
            API.Check(type, nameof(type));

            foreach (EslonEditor item in Editors)
            {
                if (item.Logo == type)
                {
                    return item;
                }
            }

            throw new ArgumentException("An editor is not available for the specified type.", nameof(type));
        }

        private class Editor_String : EslonEditor
        {
            public Editor_String() : base(typeof(string), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                return volume.Move<EslonDisk>().Value;
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Auto, value.ToString());
            }
        }

        private class Editor_Boolean : EslonEditor
        {
            public Editor_Boolean() : base(typeof(bool), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                string str = volume.Move<EslonDisk>().Value;

                if (str == "true")
                {
                    return true;
                }

                if (str == "false")
                {
                    return false;
                }

                throw new EslonMoveException(volume, typeof(bool));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ((bool)value ? "true" : "false"));
            }
        }

        private class Editor_Char : EslonEditor
        {
            public Editor_Char() : base(typeof(char), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                string str = volume.Move<EslonDisk>().Value;

                if (str.Length == 1)
                {
                    return str[0];
                }

                throw new EslonMoveException(volume, typeof(char));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Auto, value.ToString());
            }
        }

        private class Editor_Byte : EslonEditor
        {
            public Editor_Byte() : base(typeof(byte), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                int value;

                if (ExtraRender.ParseInt32(volume.Move<EslonDisk>().Value, out value) && value >= 0 && value <= 255)
                {
                    return (byte)value;
                }

                throw new EslonMoveException(volume, typeof(byte));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((byte)value));
            }
        }

        private class Editor_SByte : EslonEditor
        {
            public Editor_SByte() : base(typeof(sbyte), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                int value;

                if (ExtraRender.ParseInt32(volume.Move<EslonDisk>().Value, out value) && value >= -128 && value <= 127)
                {
                    return (sbyte)value;
                }

                throw new EslonMoveException(volume, typeof(sbyte));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((sbyte)value));
            }
        }

        private class Editor_Int16 : EslonEditor
        {
            public Editor_Int16() : base(typeof(short), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                short value;

                if (ExtraRender.ParseInt16(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(short));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((short)value));
            }
        }

        private class Editor_UInt16 : EslonEditor
        {
            public Editor_UInt16() : base(typeof(ushort), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                ushort value;

                if (ExtraRender.ParseUInt16(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(ushort));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((ushort)value));
            }
        }

        private class Editor_Int32 : EslonEditor
        {
            public Editor_Int32() : base(typeof(int), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                int value;

                if (ExtraRender.ParseInt32(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(int));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((int)value));
            }
        }

        private class Editor_UInt32 : EslonEditor
        {
            public Editor_UInt32() : base(typeof(uint), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                uint value;

                if (ExtraRender.ParseUInt32(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(uint));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((uint)value));
            }
        }

        private class Editor_Int64 : EslonEditor
        {
            public Editor_Int64() : base(typeof(long), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                long value;

                if (ExtraRender.ParseInt64(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(long));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((long)value));
            }
        }

        private class Editor_UInt64 : EslonEditor
        {
            public Editor_UInt64() : base(typeof(ulong), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                ulong value;

                if (ExtraRender.ParseUInt64(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(ulong));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Absolute, ExtraRender.ToDecimalString((ulong)value));
            }
        }

        private class Editor_Single : EslonEditor
        {
            public Editor_Single() : base(typeof(float), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                float value;

                if (Single.TryParse(volume.Move<EslonDisk>().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(float));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                float cast = (float)value;

                return new EslonDisk((Single.IsNaN(cast) || Single.IsInfinity(cast) ? EslonDiskMode.Cosmetic : EslonDiskMode.Absolute), ExtraRender.ToString(cast));
            }
        }

        private class Editor_Double : EslonEditor
        {
            public Editor_Double() : base(typeof(double), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                double value;

                if (Double.TryParse(volume.Move<EslonDisk>().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(double));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                double cast = (double)value;

                return new EslonDisk((Double.IsNaN(cast) || Double.IsInfinity(cast) ? EslonDiskMode.Cosmetic : EslonDiskMode.Absolute), ExtraRender.ToString(cast));
            }
        }

        private class Editor_Decimal : EslonEditor
        {
            public Editor_Decimal() : base(typeof(decimal), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                decimal value;

                if (Decimal.TryParse(volume.Move<EslonDisk>().Value, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(decimal));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Cosmetic, ExtraRender.ToString((decimal)value));
            }
        }

        private class Editor_DateTime : EslonEditor
        {
            public Editor_DateTime() : base(typeof(DateTime), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                DateTime value;

                if (PanTime.Parse(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(DateTime));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Cosmetic, PanTime.Serialize((DateTime)value));
            }
        }

        private class Editor_DateTimeOffset : EslonEditor
        {
            public Editor_DateTimeOffset() : base(typeof(DateTimeOffset), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                DateTimeOffset value;

                if (PanTime.Parse(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(DateTimeOffset));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Cosmetic, PanTime.Serialize((DateTimeOffset)value));
            }
        }

        private class Editor_TimeSpan : EslonEditor
        {
            public Editor_TimeSpan() : base(typeof(TimeSpan), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                TimeSpan value;

                if (TimeSpan.TryParse(volume.Move<EslonDisk>().Value, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(TimeSpan));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Cosmetic, ((TimeSpan)value).ToStringInvariant());
            }
        }

        private class Editor_Guid : EslonEditor
        {
            public Editor_Guid() : base(typeof(Guid), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                Guid value;

                if (Guid.TryParse(volume.Move<EslonDisk>().Value, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(Guid));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Cosmetic, ((Guid)value).ToStringInvariant());
            }
        }

        private class Editor_Uri : EslonEditor
        {
            public Editor_Uri() : base(typeof(Uri), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                Uri value;

                if (Uri.TryCreate(volume.Move<EslonDisk>().Value, UriKind.RelativeOrAbsolute, out value))
                {
                    return value;
                }

                throw new EslonMoveException(volume, typeof(Uri));
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Auto, ((Uri)value).OriginalString);
            }
        }

        private class Editor_ByteArray : EslonEditor
        {
            public Editor_ByteArray() : base(typeof(byte[]), JavaCanon.Reader, JavaCanon.Writer) { }

            protected override object AutoCast(EslonVolume volume, object parent)
            {
                try
                {
                    return Convert.FromBase64String(volume.Move<EslonDisk>().Value);
                }
                catch (FormatException exception)
                {
                    throw new EslonMoveException(volume, typeof(byte[]), exception.Message, exception);
                }
            }

            protected override EslonVolume AutoDigest(object value)
            {
                return new EslonDisk(EslonDiskMode.Auto, Convert.ToBase64String((byte[])value, Base64FormattingOptions.None));
            }
        }
    }
}
