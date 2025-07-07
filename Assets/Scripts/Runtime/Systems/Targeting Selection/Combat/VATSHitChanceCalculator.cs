namespace Universal.Runtime.Systems.TargetingSelection
{
    public class VATSHitChanceCalculator : HitChanceCalculator
    {
        public float vatsBonus = 1.2f;

        public override float Calculate(BodyPart part, float playerAccuracy)
        => part.CalculateHitChance(playerAccuracy) * vatsBonus;
    }
}