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
        bool duringRunAnimation;

        public CameraEffects(
            CharacterMovementController movementController,
            CharacterCameraController cameraController,
            IPlayerInputReader inputReader)
        {
            this.movementController = movementController;
            this.cameraController = cameraController;
            this.inputReader = inputReader;
        }

        public void HandleRunFOV()
        {
            var isMoving = inputReader.MoveDirection != Vector2.zero;
            var isRunning = inputReader.Running > 0.1f;
            var movementBlocked = movementController.CharacterMovement.IsBlocked;

            // Handle starting to run
            if (isMoving && !movementBlocked)
            {
                var shouldStartRun = isRunning ||
                    (!duringRunAnimation && movementController.CharacterMovement.IsRunning);
                if (shouldStartRun)
                {
                    duringRunAnimation = true;
                    cameraController.ChangeRunFOV(false);
                    return; // Early exit since we've handled the running case
                }
            }

            // Handle stopping run conditions
            var shouldStopRun = !isRunning || !isMoving || movementBlocked;
            if (duringRunAnimation && shouldStopRun)
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }

        public void HandleCameraSway() => cameraController.HandleSway(
            movementController.CharacterMovement.InputBuffer,
            inputReader.MoveDirection.x);
    }
}