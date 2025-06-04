using System;
using System.Collections;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public class FastRemoveList<T> : IReadOnlyList<T>
    {
        readonly List<T> list = new();
        readonly Dictionary<T, int> indexMap = new();

        public int Count => list.Count;

        public T this[int index] => list[index];

        public bool Add(T value)
        {
            if (value == null || indexMap.ContainsKey(value))
                return false;

            indexMap.Add(value, list.Count);
            list.Add(value);
            return true;
        }

        public bool Remove(T value)
        {
            if (value == null || !indexMap.TryGetValue(value, out var index))
                return false;

            RemoveAt(index);
            return true;
        }

        private void RemoveAt(int index)
        {
            var lastIndex = list.Count - 1;
            var lastValue = list[lastIndex];

            if (index != lastIndex)
            {
                list[index] = lastValue;
                indexMap[lastValue] = index;
            }

            list.RemoveAt(lastIndex);
            indexMap.Remove(list[index]);
        }

        public void Clear()
        {
            list.Clear();
            indexMap.Clear();
        }

        public Enumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            readonly FastRemoveList<T> list;
            readonly int version;
            int index;

            public Enumerator(FastRemoveList<T> list)
            {
                this.list = list;
                version = list.list.Count; // Simple version check
                index = -1;
            }

            public T Current => list.list[index];
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (version != list.list.Count)
                    throw new InvalidOperationException("Collection was modified");

                if (index < list.list.Count - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            public void Reset() => index = -1;
        }
    }
}