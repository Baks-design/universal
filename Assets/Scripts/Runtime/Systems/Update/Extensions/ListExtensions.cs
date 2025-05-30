using System.Collections.Generic;

namespace Universal.Runtime.Systems.Update.Extensions
{
    public static class ListExtensions
    {
        public static void RemoveAtSwapBack<T>(this IList<T> list, int index, out T swappedValue)
        {
            var lastIndex = list.Count - 1;
            swappedValue = (uint)index < (uint)lastIndex ? list[index] = list[lastIndex] : default;
            list.RemoveAt(lastIndex);
        }

        public static void Swap<T>(this IList<T> list, int sourceIndex, int destinationIndex, out T newDestinationValue)
        {
            newDestinationValue = list[sourceIndex];
            list[sourceIndex] = list[destinationIndex];
            list[destinationIndex] = newDestinationValue;
        }
    }
}
