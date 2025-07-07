using Random = UnityEngine.Random;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class CombatSystem
    {
        readonly HitChanceCalculator hitChanceCalculator;

        public CombatSystem(HitChanceCalculator hitChanceCalculator)
        => this.hitChanceCalculator = hitChanceCalculator;

        public AttackResult CalculateAttack(BodyPart targetPart, float playerAccuracy, float baseDamage)
        {
            if (targetPart == null)
                return new AttackResult { hitSuccess = false };

            var hitChance = hitChanceCalculator.Calculate(targetPart, playerAccuracy);
            var hitSuccess = Random.value <= hitChance;

            return new AttackResult
            {
                hitSuccess = hitSuccess,
                damageMultiplier = targetPart.HitMultiplier,
                damageDealt = hitSuccess ? baseDamage * targetPart.HitMultiplier : 0f
            };
        }
    }
}