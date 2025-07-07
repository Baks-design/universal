using System;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public interface ITargetingSystem
    {
        BodyPart CurrentSelectedPart { get; }

        event Action<BodyPart> OnTargetChanged;

        bool TrySelectTarget(out BodyPart targetPart);
        void CycleSelection(float forward);
    }
}