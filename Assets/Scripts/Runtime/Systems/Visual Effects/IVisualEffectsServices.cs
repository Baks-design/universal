namespace Universal.Runtime.Systems.VisualEffects
{
    public interface IVisualEffectsServices
    {
        EffectsBuilder CreateSoundBuilder();
        bool CanPlaySound(EffectsData data);
        EffectsEmitter Get();
        void ReturnToPool(EffectsEmitter effectEmitter);
        void StopAll();
    }
}