namespace Universal.Runtime.Systems.SoundEffects
{
    public interface ISoundEffectsServices
    {
        SoundBuilder CreateSoundBuilder();
        bool CanPlaySound(SoundData data);
        SoundEmitter Get();
        void ReturnToPool(SoundEmitter soundEmitter);
        void StopAll();
    }
}