using System;
using System.Collections;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.Update.Internal
{
    public class SortedList<T> : IComparer<T>, IEnumerable<T> where T : IComparable<T>
    {
        readonly List<T> _list = new();

        public int Count => _list.Count;

        public bool Add(T value)
        {
            var index = _list.BinarySearch(value, this);
            if (index >= 0)
                return false;

            _list.Insert(~index, value);
            return true;
        }

        public bool Remove(T value)
        {
            var index = _list.BinarySearch(value, this);
            if (index < 0)
                return false;

            _list.RemoveAt(index);
            return true;
        }

        public bool Contains(T value) => _list.BinarySearch(value, this) >= 0;

        public void Clear() => _list.Clear();

        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual int Compare(T x, T y) => x.CompareTo(y);
    }

    public class ReversedSortedList<T> : SortedList<T> where T : IComparable<T>
    {
        public override int Compare(T x, T y) => y.CompareTo(x);  // Reversed comparison
    }
}
