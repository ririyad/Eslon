using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eslon
{
    /// <summary>
    /// Presents a periodic list.
    /// </summary>
    public sealed class EslonList : EslonVolume, IList<EslonVolume>
    {
        private const int StandardCapacity = 4;

        private static readonly EslonVolume[] Empty = new EslonVolume[0];

        private int size;
        private int version;
        private EslonVolume[] code;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonList"/>.
        /// </summary>
        public EslonList()
        {
            this.code = new EslonVolume[StandardCapacity];
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonList"/>.
        /// </summary>
        /// <param name="capacity">
        /// The initial capacity of the new list.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than 0.
        /// </exception>
        public EslonList(int capacity)
        {
            if (capacity < 0)
            {
                API.ThrowArgumentOutOfRangeException(nameof(capacity));
            }

            this.code = Empty;

            Expand(capacity);
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to get or set.
        /// </param>
        /// <returns>
        /// The item at the specified index.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is out of range.
        /// </exception>
        public EslonVolume this[int index]
        {
            get
            {
                if (index < 0 || index >= size)
                {
                    API.ThrowArgumentOutOfRangeException(nameof(index));
                }

                return code[index];
            }

            set
            {
                API.Check(value, nameof(value));

                if (index < 0 || index >= size)
                {
                    API.ThrowArgumentOutOfRangeException(nameof(index));
                }

                code[index] = value;
                version++;
            }
        }

        /// <summary>
        /// Gets the number of items contained in the list.
        /// </summary>
        /// <returns>
        /// The number of items contained in the list.
        /// </returns>
        public int Count
        {
            get { return size; }
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
                return EslonSymbol.List;
            }
        }

        internal EslonVolume[] Code
        {
            get { return code; }
        }

        bool ICollection<EslonVolume>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        public void Add(EslonVolume item)
        {
            API.Check(item, nameof(item));

            if (size == code.Length)
            {
                Expand();
            }

            code[size++] = item;
            version++;
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index where the item is inserted.
        /// </param>
        /// <param name="item">
        /// The item to insert.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="item"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is out of range.
        /// </exception>
        public void Insert(int index, EslonVolume item)
        {
            API.Check(item, nameof(item));

            if (index < 0 || index > size)
            {
                API.ThrowArgumentOutOfRangeException(nameof(index));
            }

            if (size == code.Length)
            {
                Expand();
            }

            if (index != size)
            {
                Array.Copy(code, index, code, index + 1, size - index);
            }

            code[index] = item;
            size++;
            version++;
        }

        /// <summary>
        /// Removes the specified item from the list.
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <returns>
        /// True if the item was removed from the list.
        /// </returns>
        public bool Remove(EslonVolume item)
        {
            int index = Array.IndexOf(code, item, 0, size);

            if (index != -1)
            {
                RemoveAt(index);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is out of range.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= size)
            {
                API.ThrowArgumentOutOfRangeException(nameof(index));
            }

            Array.Copy(code, index + 1, code, index, (size - index) - 1);
            code[--size] = null;
            version++;
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            if (size != 0)
            {
                Array.Clear(code, 0, size);
                size = 0;
                version++;
            }
        }

        /// <summary>
        /// Determines if the list contains the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to locate.
        /// </param>
        /// <returns>
        /// True if the list contains the specified item.
        /// </returns>
        public bool Contains(EslonVolume item)
        {
            return (IndexOf(item) != -1);
        }

        /// <summary>
        /// Gets the zero-based index of the specified item.
        /// </summary>
        /// <param name="item">
        /// The item to locate.
        /// </param>
        /// <returns>
        /// The zero-based index of the specified item, or –1 if the item is missing.
        /// </returns>
        public int IndexOf(EslonVolume item)
        {
            return Array.IndexOf(code, item, 0, size);
        }

        /// <summary>
        /// Acquires an enumerator for the current list.
        /// </summary>
        /// <returns>
        /// An enumerator for the current list.
        /// </returns>
        public IEnumerator<EslonVolume> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Copies the entire list to the specified array.
        /// </summary>
        /// <param name="array">
        /// The array to manipulate.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index where copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0 or does not designate a viable range.
        /// </exception>
        public void CopyTo(EslonVolume[] array, int arrayIndex)
        {
            API.Check(array, nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length - size)
            {
                API.ThrowArgumentOutOfRangeException(nameof(arrayIndex));
            }

            Array.Copy(code, 0, array, arrayIndex, size);
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

        private void Expand()
        {
            Expand(code.Length < StandardCapacity ? StandardCapacity : code.Length * 2);
        }

        private void Expand(int capacity)
        {
            Array.Resize(ref code, ((uint)capacity > BCL.MaxArrayLength ? BCL.MaxArrayLength : capacity));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<EslonVolume>
        {
            private EslonList list;
            private int version;
            private int index;
            private EslonVolume value;

            public Enumerator(EslonList list)
            {
                this.list = list;
                this.version = list.version;
                this.index = 0;
                this.value = null;
            }

            public EslonVolume Current
            {
                get
                {
                    if (index == 0)
                    {
                        API.ThrowInvalidOperationException("A value is not available at this time.");
                    }

                    return value;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (version != list.version)
                {
                    API.ThrowInvalidOperationException("The operation is invalid because the list has changed.");
                }

                if (index != list.size)
                {
                    value = list.code[index++];

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                if (version != list.version)
                {
                    API.ThrowInvalidOperationException("The operation is invalid because the list has changed.");
                }

                index = 0;
                value = null;
            }

            public void Dispose() { }
        }
    }
}
