using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Runtime.Components.Camera;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CameraEffects
    {
        readonly CharacterMovementController movementController;
        readonly CharacterCameraController cameraController;
        bool duringRunAnimation;

        public CameraEffects(
            CharacterMovementController movementController,
            CharacterCameraController cameraController)
        {
            this.movementController = movementController;
            this.cameraController = cameraController;
        }

        public void RegisterInput()
        {
            PlayerMapInputProvider.Move.performed += OnFOVMovePerformed;
            PlayerMapInputProvider.Move.canceled += OnFOVMoveCanceled;

            PlayerMapInputProvider.Run.started += OnFOVRunStarted;
            PlayerMapInputProvider.Run.canceled += OnFOVRunCanceled;
        }

        public void UnregisterInput()
        {
            PlayerMapInputProvider.Move.performed -= OnFOVMovePerformed;
            PlayerMapInputProvider.Move.canceled -= OnFOVMoveCanceled;
            
            PlayerMapInputProvider.Run.started -= OnFOVRunStarted;
            PlayerMapInputProvider.Run.canceled -= OnFOVRunCanceled;
        }

        void OnFOVMovePerformed(InputAction.CallbackContext context)
        {
            // Handle case where player starts moving while already running
            if (movementController.CharacterMovement.IsRunning &&
                !duringRunAnimation &&
                !movementController.CharacterMovement.IsBlocked)
            {
                duringRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }
        }

        void OnFOVMoveCanceled(InputAction.CallbackContext context)
        {
            // If movement stops while running, cancel FOV change
            if (duringRunAnimation)
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }

        void OnFOVRunStarted(InputAction.CallbackContext context)
        {
            if (context.ReadValue<Vector2>() != Vector2.zero &&
                !movementController.CharacterMovement.IsBlocked)
            {
                duringRunAnimation = true;
                cameraController.ChangeRunFOV(false);
            }
        }

        void OnFOVRunCanceled(InputAction.CallbackContext context)
        {
            if (duringRunAnimation)
            {
                duringRunAnimation = false;
                cameraController.ChangeRunFOV(true);
            }
        }

        public void HandleCameraSway() => cameraController.HandleSway(
           movementController.CharacterMovement.InputBuffer,
           PlayerMapInputProvider.Move.ReadValue<Vector2>().x);
    }
}