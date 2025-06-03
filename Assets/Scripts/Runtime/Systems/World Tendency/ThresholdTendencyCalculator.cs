namespace Universal.Runtime.Systems.WorldTendency
{
    public class ThresholdTendencyCalculator : ITendencyCalculator
    {
        readonly float[] thresholds;

        public ThresholdTendencyCalculator(float[] thresholds) => this.thresholds = thresholds;

        public TendencyState CalculateTendency(float tendencyValue)
        {
            if (tendencyValue >= thresholds[0]) return TendencyState.PureWhite;
            if (tendencyValue >= thresholds[1]) return TendencyState.White;
            if (tendencyValue <= thresholds[3]) return TendencyState.PureBlack;
            if (tendencyValue <= thresholds[2]) return TendencyState.Black;
            return TendencyState.Neutral;
        }
    }
}