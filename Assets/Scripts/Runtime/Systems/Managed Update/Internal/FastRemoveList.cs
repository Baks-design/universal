using System;
using System.Collections;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ManagedUpdate
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
        int loopIndex = -1;
        readonly List<T> list = new();
        readonly Dictionary<T, int> indexMap = new();

        public int Count => list.Count;

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return list[index];
            }
        }

        public bool Add(T value)
        {
            if (indexMap.ContainsKey(value))
                return false;

            indexMap.Add(value, list.Count);
            list.Add(value);
            return true;
        }

        public bool Remove(T value)
        {
            // Early exit if value doesn't exist
            if (!indexMap.TryGetValue(value, out var indexToRemove))
                return false;

            // Remove from dictionary first to maintain consistency
            indexMap.Remove(value);

            // Handle enumeration position adjustment
            if (indexToRemove <= loopIndex)
            {
                if (indexToRemove == loopIndex)
                    loopIndex--;
                else // indexToRemove < loopIndex
                {
                    // Swap with current enumeration position
                    ListExtensions.Swap(list, loopIndex, indexToRemove);
                    indexToRemove = loopIndex;
                    loopIndex--;
                }
            }

            // Perform the actual removal
            ListExtensions.RemoveAtSwapBack(list, indexToRemove, out _);
            return true;
        }

        public void Clear()
        {
            list.Clear();
            indexMap.Clear();
            loopIndex = -1;
        }

        public Enumerator GetEnumerator() => new(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            readonly FastRemoveList<T> list;
            int currentIndex;

            public Enumerator(FastRemoveList<T> list)
            {
                this.list = list;
                currentIndex = -1;
            }

            public readonly T Current => list.list[currentIndex];
            readonly object IEnumerator.Current => Current;

            public readonly void Dispose() { }

            public bool MoveNext()
            {
                if (currentIndex < list.Count - 1)
                {
                    currentIndex++;
                    return true;
                }
                return false;
            }

            public void Reset() => currentIndex = -1;
        }
    }
}