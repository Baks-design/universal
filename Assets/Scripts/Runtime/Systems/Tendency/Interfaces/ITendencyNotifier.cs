using System;

namespace Universal.Runtime.Systems.Tendency.Interfaces
{
    public interface ITendencyNotifier
    {
        event Action<int, TendencyState> OnTendencyChanged;
    }
}