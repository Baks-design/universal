using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public static class ListExtensions
    {
        /// <summary>
        /// Swaps two elements in the list by their indices.
        /// </summary>
        /// <param name="list">The list to modify</param>
        /// <param name="a">First index</param>
        /// <param name="b">Second index</param>
        public static void Swap<T>(this List<T> list, int a, int b)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if ((uint)a >= (uint)list.Count)
                throw new ArgumentOutOfRangeException(nameof(a));

            if ((uint)b >= (uint)list.Count)
                throw new ArgumentOutOfRangeException(nameof(b));

            if (a == b) return;

            (list[b], list[a]) = (list[a], list[b]);
        }

        /// <summary>
        /// Removes an element at the specified index by swapping it with the last element, then truncating the list.
        /// This is O(1) compared to RemoveAt's O(n) for non-end elements.
        /// </summary>
        /// <param name="list">The list to modify</param>
        /// <param name="index">Index of element to remove</param>
        /// <param name="swappedValue">The value that was swapped from the end</param>
        /// <returns>True if swap occurred, false if element was already last</returns>
        public static bool RemoveAtSwapBack<T>(this List<T> list, int index, out T swappedValue)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var lastIndex = list.Count - 1;

            if ((uint)index >= (uint)list.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index < lastIndex)
            {
                swappedValue = list[index] = list[lastIndex];
                list.RemoveAt(lastIndex);
                return true;
            }

            swappedValue = default;
            list.RemoveAt(lastIndex);
            return false;
        }
    }
}
