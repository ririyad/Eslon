using System;
using System.Collections;
using System.Collections.Generic;

namespace EslonTest
{
    class Enumerable<T> : IEnumerable<T>
    {
        private List<T> list;

        public Enumerable()
        {
            this.list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
