using UnityEngine;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CameraEffects
    {
        readonly CharacterMovementController movementController;
        readonly CharacterCameraController cameraController;
        readonly IPlayerInputReader inputReader;

        public CameraEffects(
            CharacterMovementController movementController,
            CharacterCameraController cameraController,
            IPlayerInputReader inputReader)
        {
            this.movementController = movementController;
            this.cameraController = cameraController;
            this.inputReader = inputReader;
        }

        public void HandleCameraSway() => cameraController.HandleSway(
            movementController.CharacterMovement.InputBuffer,
            inputReader.MoveDirection.x);
    }
}