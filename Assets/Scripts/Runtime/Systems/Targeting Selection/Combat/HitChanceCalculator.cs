namespace Universal.Runtime.Systems.TargetingSelection
{
    public abstract class HitChanceCalculator
    {
        public abstract float Calculate(BodyPart part, float playerAccuracy);
    }
}