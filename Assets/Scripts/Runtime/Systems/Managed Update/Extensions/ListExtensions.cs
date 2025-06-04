using System;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public static class ListExtensions
    {
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
