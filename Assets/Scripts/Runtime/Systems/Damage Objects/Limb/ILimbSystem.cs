namespace Universal.Runtime.Systems.DamageObjects
{
    public interface ILimbSystem
    {
        Limb[] GetLimbs();
        Limb GetLimb(LimbType type);
        void RegisterLimb(Limb limb);
    }
}