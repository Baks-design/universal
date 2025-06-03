using System;

namespace Universal.Runtime.Systems.WorldTendency
{
    public interface ITendencyNotifier
    {
        event Action<int, TendencyState> OnTendencyChanged;
    }
}