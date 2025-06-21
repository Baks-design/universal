namespace Universal.Runtime.Systems.WorldTendency
{
    public class ThresholdTendencyCalculator : ITendencyCalculator
    {
        readonly float[] thresholds;

        public ThresholdTendencyCalculator(float[] thresholds) => this.thresholds = thresholds;

        public TendencyState CalculateTendency(float tendencyValue) => tendencyValue switch
        {
            _ when tendencyValue >= thresholds[0] => TendencyState.PureWhite,
            _ when tendencyValue >= thresholds[1] => TendencyState.White,
            _ when tendencyValue <= thresholds[3] => TendencyState.PureBlack,
            _ when tendencyValue <= thresholds[2] => TendencyState.Black,
            _ => TendencyState.Neutral
        };
    }
}