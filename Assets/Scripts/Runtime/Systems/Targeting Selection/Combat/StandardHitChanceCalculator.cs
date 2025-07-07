namespace Universal.Runtime.Systems.TargetingSelection
{
    public class StandardHitChanceCalculator : HitChanceCalculator
    {
        public override float Calculate(BodyPart part, float playerAccuracy)
        => part.CalculateHitChance(playerAccuracy);
    }
}