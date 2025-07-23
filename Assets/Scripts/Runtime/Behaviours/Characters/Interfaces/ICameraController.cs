using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    public interface ICameraController
    {
        Transform Transform { get; }

        void HandleSway(Vector2 inputVector, float mouseX);
        void ChangeRunFOV(bool shouldReturn);
    }
}