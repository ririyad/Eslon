using System;
using System.Collections;
using System.Collections.Generic;

namespace EslonTest
{
    class Collection<T> : ICollection<T>
    {
        private List<T> list;

        public Collection()
        {
            this.list = new List<T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
