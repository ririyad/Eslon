using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Eslon
{
    /// <summary>
    /// Presents a configuration of sectors and comments.
    /// </summary>
    public sealed class EslonBatch
    {
        private bool strict;
        private bool descriptive;
        private EslonComment header;
        private EslonComment poster;
        private EslonSectorCollection sectors;
        private EslonCommentCollection comments;
        private EslonBatchReader reader;
        private EslonBatchWriter writer;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonBatch"/>.
        /// </summary>
        public EslonBatch() : this(true, true) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonBatch"/>.
        /// </summary>
        /// <param name="strict">
        /// Cites true to enable strict mode.
        /// </param>
        /// <param name="descriptive">
        /// Cites true to enable descriptive mode.
        /// </param>
        public EslonBatch(bool strict, bool descriptive)
        {
            this.strict = strict;
            this.descriptive = descriptive;
            this.sectors = new EslonSectorCollection();
            this.comments = new EslonCommentCollection();
            this.reader = new EslonBatchReader(this);
            this.writer = new EslonBatchWriter(this);
        }

        /// <summary>
        /// Cites true if the batch is strict.
        /// </summary>
        /// <returns>
        /// True if the batch is strict.
        /// </returns>
        public bool Strict
        {
            get { return strict; }
        }

        /// <summary>
        /// Cites true if the batch is descriptive.
        /// </summary>
        /// <returns>
        /// True if the batch is descriptive.
        /// </returns>
        public bool Descriptive
        {
            get { return descriptive; }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <returns>
        /// The configured header.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot set a header because the batch is not descriptive.
        /// </exception>
        public EslonComment Header
        {
            get { return header; }

            set
            {
                if (!descriptive)
                {
                    API.ThrowInvalidOperationException("Cannot set a header because the batch is not descriptive.");
                }

                header = value;
            }
        }

        /// <summary>
        /// Gets or sets the poster.
        /// </summary>
        /// <returns>
        /// The configured poster.
        /// </returns>
        public EslonComment Poster
        {
            get { return poster; }
            set { poster = value; }
        }

        /// <summary>
        /// Gets the configured sectors.
        /// </summary>
        /// <returns>
        /// The configured sectors.
        /// </returns>
        public EslonSectorCollection Sectors
        {
            get { return sectors; }
        }

        /// <summary>
        /// Gets the configured comments.
        /// </summary>
        /// <returns>
        /// The configured comments.
        /// </returns>
        public EslonCommentCollection Comments
        {
            get { return comments; }
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        public void Read(string text)
        {
            API.Check(text, nameof(text));

            reader.Read(text);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to read.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support reading.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        public void Read(Stream stream)
        {
            Read(stream, null, true, 0);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to read.
        /// </param>
        /// <param name="encoding">
        /// The encoding to employ.
        /// </param>
        /// <param name="detectEncoding">
        /// Cites true to detect encoding.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support reading.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        public void Read(Stream stream, Encoding encoding, bool detectEncoding)
        {
            Read(stream, encoding, detectEncoding, 0);
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to read.
        /// </param>
        /// <param name="encoding">
        /// The encoding to employ.
        /// </param>
        /// <param name="detectEncoding">
        /// Cites true to detect encoding.
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize"/> is less than 0.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        public void Read(Stream stream, Encoding encoding, bool detectEncoding, int bufferSize)
        {
            API.Check(stream, nameof(stream));

            if (!stream.CanRead)
            {
                API.ThrowArgumentException("The stream does not support reading.", nameof(stream));
            }

            if (bufferSize < 0)
            {
                API.ThrowArgumentOutOfRangeException(nameof(bufferSize));
            }

            reader.Read(stream, encoding, detectEncoding, bufferSize);
        }

        /// <summary>
        /// Writes the current batch to a new string.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        public string Write()
        {
            StringBuilder builder = new StringBuilder(8192);

            try
            {
                writer.Write(builder);

                return builder.ToString();
            }
            finally
            {
                builder.Clear();
            }
        }

        /// <summary>
        /// Writes the current batch using the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support writing.
        /// </exception>
        public void Write(Stream stream)
        {
            Write(stream, null, 0);
        }

        /// <summary>
        /// Writes the current batch using the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to employ.
        /// </param>
        /// <param name="encoding">
        /// The encoding to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support writing.
        /// </exception>
        public void Write(Stream stream, Encoding encoding)
        {
            Write(stream, encoding, 0);
        }

        /// <summary>
        /// Writes the current batch using the specified stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to employ.
        /// </param>
        /// <param name="encoding">
        /// The encoding to employ.
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="stream"/> does not support writing.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="bufferSize"/> is less than 0.
        /// </exception>
        public void Write(Stream stream, Encoding encoding, int bufferSize)
        {
            API.Check(stream, nameof(stream));

            if (!stream.CanWrite)
            {
                API.ThrowArgumentException("The stream does not support writing.", nameof(stream));
            }

            if (bufferSize < 0)
            {
                API.ThrowArgumentOutOfRangeException(nameof(bufferSize));
            }

            writer.Write(stream, encoding, bufferSize);
        }

        /// <summary>
        /// Casts an object from the current batch.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="engine">
        /// The engine to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="engine"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEngine.Cast{T}(EslonVolume, System.Object)"/>
        public T Cast<T>(EslonEngine engine)
        {
            return Cast<T>(engine, null);
        }

        /// <summary>
        /// Casts an object from the current batch.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="engine">
        /// The engine to employ.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="engine"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEngine.Cast{T}(EslonVolume, System.Object)"/>
        public T Cast<T>(EslonEngine engine, object parent)
        {
            API.Check(engine, nameof(engine));

            return engine.Cast<T>(new EslonMap(sectors.Core), parent);
        }

        /// <summary>
        /// Configures the batch using the specified object.
        /// </summary>
        /// <param name="value">
        /// The object to employ.
        /// </param>
        /// <param name="engine">
        /// The engine to employ.
        /// </param>
        /// <param name="preserveComments">
        /// Cites true to preserve comments.
        /// </param>
        /// <returns>
        /// The current instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="engine"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="engine"/> contains an invalid reader.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        /// <exception cref="EslonBatchException">
        /// The configuration was canceled.
        /// </exception>
        public EslonBatch Configure(object value, EslonEngine engine, bool preserveComments)
        {
            API.Check(engine, nameof(engine));

            if (engine.Reader.GetType() != EslonCanon.Reader.GetType())
            {
                API.ThrowArgumentException("The specified engine contains an invalid reader.", nameof(engine));
            }

            EslonMap map = engine.Digest(value).Move<EslonMap>();
            EslonComment[] array = (strict && preserveComments) ? ExtractComments() : null;

            Clear();

            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                if (node.Value.Symbol != EslonSymbol.Map && node.Value.Symbol != EslonSymbol.List)
                {
                    throw new EslonBatchException(COM.Express("Cannot include a volume of '%'.", node.Value.Symbol.ToString()));
                }

                sectors.Core.Add(node.Key, node.Value);
            }

            if (array != null && array.Length != 1)
            {
                InsertComments(array);
            }

            return this;
        }

        /// <summary>
        /// Copies the included comments to a new array.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public EslonComment[] ExtractComments()
        {
            List<EslonComment> list = new List<EslonComment>();

            list.Add(poster);
            ExtractComments(sectors.Core, list, true);

            return list.ToArray();
        }

        /// <summary>
        /// Includes the specified comments.
        /// </summary>
        /// <param name="array">
        /// The comments to include.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="EslonBatchException">
        /// <paramref name="array"/> is insufficient.
        /// </exception>
        public void InsertComments(EslonComment[] array)
        {
            API.Check(array, nameof(array));

            comments.Core.Clear();

            int position = 1;

            if (array.Length == 0 || !InsertComments(array, sectors.Core, true, ref position))
            {
                throw new EslonBatchException("The array is insufficient.");
            }

            poster = array[0];
        }

        private void ExtractComments(EslonMap map, List<EslonComment> collection, bool reduce)
        {
            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                EslonComment comment;

                comments.Core.TryGetValue(node.Value, out comment);
                collection.Add(comment);

                if (reduce && node.Value.Symbol == EslonSymbol.Map)
                {
                    ExtractComments((EslonMap)node.Value, collection, false);
                }
            }
        }

        private bool InsertComments(EslonComment[] array, EslonMap map, bool reduce, ref int position)
        {
            int length = array.Length;

            for (EslonMapNode node = map.Begin; node != null; node = node.Next)
            {
                if (position == length)
                {
                    return false;
                }

                EslonComment comment = array[position++];

                if (comment != null)
                {
                    comments.Core.Add(node.Value, comment);
                }

                if (reduce && node.Value.Symbol == EslonSymbol.Map && !InsertComments(array, (EslonMap)node.Value, false, ref position))
                {
                    return false;
                }
            }

            return true;
        }

        private void Clear()
        {
            sectors.Core.Clear();
            comments.Core.Clear();
            poster = null;
        }
    }

    /// <summary>
    /// Presents a comment collection for <see cref="EslonBatch"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class EslonCommentCollection : IEnumerable<KeyValuePair<EslonVolume, EslonComment>>
    {
        private Dictionary<EslonVolume, EslonComment> core;

        internal EslonCommentCollection()
        {
            this.core = new Dictionary<EslonVolume, EslonComment>();
        }

        internal Dictionary<EslonVolume, EslonComment> Core
        {
            get { return core; }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key that decides the resulting value.
        /// </param>
        /// <param name="value">
        /// Contains the resulting value, or null if the key is missing.
        /// </param>
        /// <returns>
        /// True if the specified key yields.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public bool Open(EslonVolume key, out EslonComment value)
        {
            API.Check(key, nameof(key));

            return core.TryGetValue(key, out value);
        }

        /// <summary>
        /// Sets the specified key and value.
        /// </summary>
        /// <param name="key">
        /// The key of the value to set.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public void Set(EslonVolume key, EslonComment value)
        {
            API.Check(key, nameof(key));

            if (value == null)
            {
                core.Remove(key);
            }
            else
            {
                if (core.ContainsKey(key))
                {
                    core[key] = value;
                }
                else
                {
                    core.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Removes all items from the comment collection.
        /// </summary>
        public void Clear()
        {
            core.Clear();
        }

        /// <summary>
        /// Acquires an enumerator for the current comment collection.
        /// </summary>
        /// <returns>
        /// An enumerator for the current comment collection.
        /// </returns>
        public IEnumerator<KeyValuePair<EslonVolume, EslonComment>> GetEnumerator()
        {
            return core.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return core.GetEnumerator();
        }
    }

    /// <summary>
    /// Presents a sector collection for <see cref="EslonBatch"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class EslonSectorCollection : IEnumerable<KeyValuePair<string, EslonVolume>>
    {
        private EslonMap core;

        internal EslonSectorCollection()
        {
            this.core = new EslonMap();
        }

        internal EslonMap Core
        {
            get { return core; }
        }

        /// <summary>
        /// Opens the map associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the map to get.
        /// </param>
        /// <returns>
        /// The map associated with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        public EslonMap OpenMap(string key)
        {
            API.Check(key, nameof(key));

            return core[key].Move<EslonMap>();
        }

        /// <summary>
        /// Opens the list associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the list to get.
        /// </param>
        /// <returns>
        /// The list associated with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        public EslonList OpenList(string key)
        {
            API.Check(key, nameof(key));

            return core[key].Move<EslonList>();
        }

        /// <summary>
        /// Opens or creates the map associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the map to get.
        /// </param>
        /// <returns>
        /// The map associated with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        public EslonMap CreateMap(string key)
        {
            API.Check(key, nameof(key));

            EslonVolume volume;

            if (!core.TryGetValue(key, out volume))
            {
                volume = new EslonMap();
                core.Add(key, volume);
            }

            return volume.Move<EslonMap>();
        }

        /// <summary>
        /// Opens or creates the list associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the list to get.
        /// </param>
        /// <returns>
        /// The list associated with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        public EslonList CreateList(string key)
        {
            API.Check(key, nameof(key));

            EslonVolume volume;

            if (!core.TryGetValue(key, out volume))
            {
                volume = new EslonList();
                core.Add(key, volume);
            }

            return volume.Move<EslonList>();
        }

        /// <summary>
        /// Removes the specified key from the sector collection.
        /// </summary>
        /// <param name="key">
        /// The key to remove.
        /// </param>
        /// <returns>
        /// True if the specified key was removed from the sector collection.
        /// </returns>
        public bool Remove(string key)
        {
            return core.Remove(key);
        }

        /// <summary>
        /// Removes all items from the sector collection.
        /// </summary>
        public void Clear()
        {
            core.Clear();
        }

        /// <summary>
        /// Acquires an enumerator for the current sector collection.
        /// </summary>
        /// <returns>
        /// An enumerator for the current sector collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, EslonVolume>> GetEnumerator()
        {
            return core.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return core.GetEnumerator();
        }
    }

    /// <summary>
    /// The exception that is thrown upon failing to configure a batch.
    /// </summary>
    public class EslonBatchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EslonBatchException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        public EslonBatchException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonBatchException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that causes the current exception.
        /// </param>
        public EslonBatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
