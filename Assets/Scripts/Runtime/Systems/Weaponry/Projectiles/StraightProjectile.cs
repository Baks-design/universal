namespace Universal.Runtime.Systems.Weaponry
{
    public class StraightProjectile : Projectile
    {
        ProjectileStraightData projectileStraightSettings;

        protected override void CastData() => projectileStraightSettings = projectileData as ProjectileStraightData;

        protected override void UpdatePosition(float deltaTime)
        {
            transform.position += deltaTime * projectileStraightSettings.SpecificSettings.Speed * currentDirection;
            base.UpdatePosition(deltaTime);
        }
    }
}
