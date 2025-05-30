using System.Collections;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.Update.Internal
{
    /// <summary>Generic list with O(1) insertion and removal.</summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Adding duplicate values and removing items that are not present are both no-op.</item>
    ///   <item>
    ///     Items may be added and removed while the collection is being enumerated without raising exceptions.
    ///     This collection guarantees that all items are enumerated, 
    ///     even if insertions or removals happen during enumeration.
    ///   </item>
    ///   <item>
    ///     All enumerators reference the same list index, so avoid having more than one enumerator at the same time.
    ///     Creating a new enumerator resets this shared index.
    ///   </item>
    /// </list>
    /// </remarks>
    public class FastRemoveList<T> : IReadOnlyList<T>
    {
        int _loopIndex;
        readonly List<T> _list = new();
        readonly Dictionary<T, int> _indexMap = new();

        public int Count => _list.Count;
        public T this[int index]
        {
            get
            {
                if ((uint)index < (uint)_list.Count)
                    return _list[index];
                return default;
            }
        }

        public bool Add(T value)
        {
            if (_indexMap.ContainsKey(value))
                return false;

            _indexMap.Add(value, _list.Count);
            _list.Add(value);
            return true;
        }

        public bool Remove(T value)
        {
            if (!_indexMap.TryGetValue(value, out int indexToRemove))
                return false;

            _indexMap.Remove(value);

            if (indexToRemove == _loopIndex)
                _loopIndex--;
            else if (indexToRemove < _loopIndex)
            {
                SwapElements(_loopIndex, indexToRemove);
                indexToRemove = _loopIndex;
                _loopIndex--;
            }

            RemoveAtSwapBack(indexToRemove);
            return true;
        }

         void SwapElements(int a, int b)
        {
            var valA = _list[a];
            var valB = _list[b];
            _list[a] = valB;
            _list[b] = valA;
            _indexMap[valA] = b;
            _indexMap[valB] = a;
        }

         void RemoveAtSwapBack(int index)
        {
            var lastIndex = _list.Count - 1;
            if (index != lastIndex)
            {
                T lastItem = _list[lastIndex];
                _list[index] = lastItem;
                _indexMap[lastItem] = index;
            }
            _list.RemoveAt(lastIndex);
        }

        public void Clear()
        {
            _list.Clear();
            _indexMap.Clear();
        }

        public Enumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly struct Enumerator : IEnumerator<T>
        {
            readonly FastRemoveList<T> _list;

            public Enumerator(FastRemoveList<T> list) => _list = list;
            public readonly T Current => _list[_list._loopIndex];
            readonly object IEnumerator.Current => Current;

            public readonly void Dispose() { }

            public readonly bool MoveNext()
            {
                if (_list._loopIndex < _list.Count - 1)
                {
                    _list._loopIndex++;
                    return true;
                }
                return false;
            }

            public readonly void Reset() => _list._loopIndex = -1;
        }
    }
}
