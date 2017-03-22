using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eslon
{
    /// <summary>
    /// Presents a periodic map.
    /// </summary>
    public sealed class EslonMap : EslonVolume, IDictionary<string, EslonVolume>
    {
        private int length;
        private int version;
        private EslonMapNode begin;
        private EslonMapNode end;
        private Dictionary<string, EslonMapNode> core;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonMap"/>.
        /// </summary>
        public EslonMap()
        {
            Init(17);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonMap"/>.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the new map.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than 0.
        /// </exception>
        public EslonMap(int capacity)
        {
            if (capacity < 0)
            {
                API.ThrowArgumentOutOfRangeException(nameof(capacity));
            }

            Init(capacity);
        }

        internal EslonMap(EslonMap map)
        {
            Init(map.length);

            for (EslonMapNode node = map.begin; node != null; node = node.Next)
            {
                AutoAdd(new EslonMapNode(node.Key, node.Value));
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the value to get or set.
        /// </param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="key"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="value"/> is null.
        /// </p>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="key"/> is missing in the map.
        /// </exception>
        public EslonVolume this[string key]
        {
            get
            {
                API.Check(key, nameof(key));

                EslonMapNode node;

                if (!core.TryGetValue(key, out node))
                {
                    API.ThrowArgumentException("The specified key is missing.", nameof(key));
                }

                return node.Value;
            }

            set
            {
                API.Check(key, nameof(key));
                API.Check(value, nameof(value));

                EslonMapNode node;

                if (!core.TryGetValue(key, out node))
                {
                    API.ThrowArgumentException("The specified key is missing.", nameof(key));
                }

                node.Value = value;
                version++;
            }
        }

        /// <summary>
        /// Gets the first node.
        /// </summary>
        /// <returns>
        /// The first node, or null if the map is empty.
        /// </returns>
        public EslonMapNode Begin
        {
            get { return begin; }
        }

        /// <summary>
        /// Gets the last node.
        /// </summary>
        /// <returns>
        /// The last node, or null if the map is empty.
        /// </returns>
        public EslonMapNode End
        {
            get { return end; }
        }

        /// <summary>
        /// Gets the number of items contained in the map.
        /// </summary>
        /// <returns>
        /// The number of items contained in the map.
        /// </returns>
        public int Count
        {
            get { return length; }
        }

        /// <summary>
        /// Gets the assigned symbol.
        /// </summary>
        /// <returns>
        /// The assigned symbol.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override EslonSymbol Symbol
        {
            get
            {
                return EslonSymbol.Map;
            }
        }

        ICollection<string> IDictionary<string, EslonVolume>.Keys
        {
            get
            {
                return new KeyCollection(this);
            }
        }

        ICollection<EslonVolume> IDictionary<string, EslonVolume>.Values
        {
            get
            {
                return new ValueCollection(this);
            }
        }

        bool ICollection<KeyValuePair<string, EslonVolume>>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Collects the values associated with the specified keys.
        /// </summary>
        /// <param name="keys">
        /// The keys that decide the resulting values.
        /// </param>
        /// <param name="values">
        /// Contains the collected values, or null if a key is missing.
        /// </param>
        /// <returns>
        /// True if all specified keys yield.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="keys"/> is null.
        /// </exception>
        public bool Collect(string[] keys, out EslonVolume[] values)
        {
            API.Check(keys, nameof(keys));

            if (length != keys.Length)
            {
                values = null;

                return false;
            }

            EslonVolume[] array = new EslonVolume[length];
            EslonMapNode node = begin;

            for (int i = 0; i < length; i++)
            {
                if (node.Key == keys[i])
                {
                    array[i] = node.Value;
                }
                else if (!TryGetValue(keys[i], out array[i]))
                {
                    values = null;

                    return false;
                }

                node = node.Next;
            }

            values = array;

            return true;
        }

        /// <summary>
        /// Adds the specified key and value to the map.
        /// </summary>
        /// <param name="key">
        /// The key to add.
        /// </param>
        /// <param name="value">
        /// The value to add.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="key"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="value"/> is null.
        /// </p>
        /// </exception>
        public void Add(string key, EslonVolume value)
        {
            API.Check(key, nameof(key));
            API.Check(value, nameof(value));

            AutoAdd(new EslonMapNode(key, value));
        }

        /// <summary>
        /// Removes the specified key from the map.
        /// </summary>
        /// <param name="key">
        /// The key to remove.
        /// </param>
        /// <returns>
        /// True if the specified key was removed from the map.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public bool Remove(string key)
        {
            API.Check(key, nameof(key));

            EslonMapNode node;

            if (core.TryGetValue(key, out node))
            {
                AutoRemove(node);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all items from the map.
        /// </summary>
        public void Clear()
        {
            begin = null;
            end = null;
            core.Clear();
            length = 0;
            version++;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key that decides the resulting value.
        /// </param>
        /// <param name="value">
        /// Contains the resulting value, or null if the specified key is missing.
        /// </param>
        /// <returns>
        /// True if the specified key yields.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue(string key, out EslonVolume value)
        {
            API.Check(key, nameof(key));

            EslonMapNode node;

            if (core.TryGetValue(key, out node))
            {
                value = node.Value;

                return true;
            }

            value = null;

            return false;
        }

        /// <summary>
        /// Determines if the map contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate.
        /// </param>
        /// <returns>
        /// True if the map contains the specified key.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return core.ContainsKey(key);
        }

        /// <summary>
        /// Acquires an enumerator for the current map.
        /// </summary>
        /// <returns>
        /// An enumerator for the current map.
        /// </returns>
        public IEnumerator<KeyValuePair<string, EslonVolume>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Copies the map to a new array.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="System.Array"/>.
        /// </returns>
        public KeyValuePair<string, EslonVolume>[] ToArray()
        {
            KeyValuePair<string, EslonVolume>[] array = new KeyValuePair<string, EslonVolume>[length];

            int index = 0;

            for (EslonMapNode node = begin; node != null; node = node.Next)
            {
                array[index++] = new KeyValuePair<string, EslonVolume>(node.Key, node.Value);
            }

            return array;
        }

        /// <summary>
        /// Converts the volume to an instance of <typeparamref name="TVolume"/>.
        /// </summary>
        /// <typeparam name="TVolume">
        /// The type to cast.
        /// </typeparam>
        /// <returns>
        /// An instance of <typeparamref name="TVolume"/>.
        /// </returns>
        /// <exception cref="EslonMoveException">
        /// The conversion failed.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override TVolume Move<TVolume>()
        {
            return this as TVolume ?? base.Move<TVolume>();
        }

        private void AutoAdd(EslonMapNode node)
        {
            core.Add(node.Key, node);

            if (begin == null)
            {
                begin = node;
                end = node;
            }
            else
            {
                end.Next = node;
                node.Post = end;
                end = node;
            }

            length++;
            version++;
        }

        private void AutoRemove(EslonMapNode node)
        {
            core.Remove(node.Key);

            if (length == 1)
            {
                begin = null;
                end = null;
            }
            else
            {
                if (node == end)
                {
                    end = end.Post;
                    end.Next = null;
                }
                else if (node == begin)
                {
                    begin = begin.Next;
                    begin.Post = null;
                }
                else
                {
                    node.Post.Next = node.Next;
                    node.Next.Post = node.Post;
                }
            }

            length--;
            version++;
        }

        private EslonMapNode GetNode(KeyValuePair<string, EslonVolume> pair)
        {
            EslonMapNode node;

            if (core.TryGetValue(pair.Key, out node) && node.Value == pair.Value)
            {
                return node;
            }

            return null;
        }

        private void Init(int capacity)
        {
            core = new Dictionary<string, EslonMapNode>(capacity, StringComparer.Ordinal);
        }

        void ICollection<KeyValuePair<string, EslonVolume>>.Add(KeyValuePair<string, EslonVolume> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, EslonVolume>>.Remove(KeyValuePair<string, EslonVolume> item)
        {
            EslonMapNode node = GetNode(item);

            if (node != null)
            {
                AutoRemove(node);

                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<string, EslonVolume>>.Contains(KeyValuePair<string, EslonVolume> item)
        {
            return (GetNode(item) != null);
        }

        void ICollection<KeyValuePair<string, EslonVolume>>.CopyTo(KeyValuePair<string, EslonVolume>[] array, int arrayIndex)
        {
            API.Check(array, nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length - length)
            {
                API.ThrowArgumentOutOfRangeException(nameof(arrayIndex));
            }

            for (EslonMapNode node = begin; node != null; node = node.Next)
            {
                array[arrayIndex++] = new KeyValuePair<string, EslonVolume>(node.Key, node.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private class KeyCollection : ICollection<string>
        {
            private EslonMap map;

            public KeyCollection(EslonMap map)
            {
                this.map = map;
            }

            public int Count
            {
                get { return map.length; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(string item)
            {
                throw new NotSupportedException();
            }

            public bool Remove(string item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(string item)
            {
                return (item != null && map.core.ContainsKey(item));
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                API.Check(array, nameof(array));

                if (arrayIndex < 0 || arrayIndex > array.Length - map.length)
                {
                    API.ThrowArgumentOutOfRangeException(nameof(arrayIndex));
                }

                for (EslonMapNode node = map.begin; node != null; node = node.Next)
                {
                    array[arrayIndex++] = node.Key;
                }
            }

            public IEnumerator<string> GetEnumerator()
            {
                return new Enumerator(map);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(map);
            }

            private class Enumerator : EnumeratorBase<string>
            {
                public Enumerator(EslonMap map) : base(map) { }

                protected override string GetCurrent()
                {
                    return node.Key;
                }
            }
        }

        private class ValueCollection : ICollection<EslonVolume>
        {
            private EslonMap map;

            public ValueCollection(EslonMap map)
            {
                this.map = map;
            }

            public int Count
            {
                get { return map.length; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Add(EslonVolume item)
            {
                throw new NotSupportedException();
            }

            public bool Remove(EslonVolume item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(EslonVolume item)
            {
                for (EslonMapNode node = map.begin; node != null; node = node.Next)
                {
                    if (node.Value == item)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void CopyTo(EslonVolume[] array, int arrayIndex)
            {
                API.Check(array, nameof(array));

                if (arrayIndex < 0 || arrayIndex > array.Length - map.length)
                {
                    API.ThrowArgumentOutOfRangeException(nameof(arrayIndex));
                }

                for (EslonMapNode node = map.begin; node != null; node = node.Next)
                {
                    array[arrayIndex++] = node.Value;
                }
            }

            public IEnumerator<EslonVolume> GetEnumerator()
            {
                return new Enumerator(map);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(map);
            }

            private class Enumerator : EnumeratorBase<EslonVolume>
            {
                public Enumerator(EslonMap map) : base(map) { }

                protected override EslonVolume GetCurrent()
                {
                    return node.Value;
                }
            }
        }

        private class Enumerator : EnumeratorBase<KeyValuePair<string, EslonVolume>>
        {
            public Enumerator(EslonMap map) : base(map) { }

            protected override KeyValuePair<string, EslonVolume> GetCurrent()
            {
                return new KeyValuePair<string, EslonVolume>(node.Key, node.Value);
            }
        }

        private abstract class EnumeratorBase<T> : IEnumerator<T>
        {
            protected EslonMapNode node;

            private EslonMap map;
            private int version;
            private bool finished;

            public EnumeratorBase(EslonMap map)
            {
                this.map = map;
                this.version = map.version;
            }

            public T Current
            {
                get
                {
                    if (node == null)
                    {
                        API.ThrowInvalidOperationException("A value is not available at this time.");
                    }

                    return GetCurrent();
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (version != map.version)
                {
                    API.ThrowInvalidOperationException("The operation is invalid because the map has changed.");
                }

                if (!finished)
                {
                    if ((node = (node == null ? map.begin : node.Next)) != null)
                    {
                        return true;
                    }

                    finished = true;
                }

                return false;
            }

            public void Reset()
            {
                if (version != map.version)
                {
                    API.ThrowInvalidOperationException("The operation is invalid because the map has changed.");
                }

                node = null;
                finished = false;
            }

            public void Dispose() { }

            protected abstract T GetCurrent();
        }
    }

    /// <summary>
    /// Presents a node of <see cref="EslonMap"/>.
    /// </summary>
    public sealed class EslonMapNode
    {
        private string key;
        private EslonVolume value;
        private EslonMapNode post;
        private EslonMapNode next;

        internal EslonMapNode(string key, EslonVolume value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the assigned key.
        /// </summary>
        /// <returns>
        /// The assigned key.
        /// </returns>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Gets the assigned value.
        /// </summary>
        /// <returns>
        /// The assigned value.
        /// </returns>
        public EslonVolume Value
        {
            get { return value; }
            internal set { this.value = value; }
        }

        /// <summary>
        /// Gets the previous node.
        /// </summary>
        /// <returns>
        /// The previous node.
        /// </returns>
        public EslonMapNode Post
        {
            get { return post; }
            internal set { post = value; }
        }

        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <returns>
        /// The next node.
        /// </returns>
        public EslonMapNode Next
        {
            get { return next; }
            internal set { next = value; }
        }
    }
}
