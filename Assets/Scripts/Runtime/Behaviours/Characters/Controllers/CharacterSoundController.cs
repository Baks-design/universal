using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField, Parent] CharacterCollisionController collisionController;
        [SerializeField, Parent] CharacterMovementController movementController;
        [SerializeField, InLineEditor] CharacterData characterData;
        [SerializeField, InLineEditor] FootstepsSoundLibrary[] surfaceData;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        Transform playPos;
        SoundBuilder soundBuilder;
        ISoundEffectsServices soundService;
        float stepTimer;

        void Start()
        {
            GetService();
            SetupDictionary();
        }

        void GetService()
        {
            ServiceLocator.Global.Get(out soundService);
            soundBuilder = soundService.CreateSoundBuilder();
        }

        void SetupDictionary()
        {
            playPos = transform;
            surfaceLookup = new Dictionary<SurfaceType, FootstepsSoundLibrary>();
            for (var i = 0; i < surfaceData.Length; i++)
                surfaceLookup[surfaceData[i].surfaceType] = surfaceData[i];
        }

        void Update() => FootstepsSoundHandler();

        void FootstepsSoundHandler()
        {
            if (movementController.CharacterMovement.IsMoving)
            {
                var currentStepInterval = GetCurrentStepInterval();
                stepTimer += Time.deltaTime;
                if (stepTimer >= currentStepInterval)
                {
                    PlayFootstepSound(GetCurrentSurface());
                    stepTimer = 0f;
                }
            }
            else
                stepTimer = 0f;
        }

        float GetCurrentStepInterval()
        {
            var currentStep = 0f;
            if (movementController.CharacterMovement.IsMoving)
                return characterData.walkStepInterval;
            else if (movementController.IsRunHold)
                return characterData.runStepInterval;
            return currentStep;
        }

        void PlayFootstepSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;

            var rnd = Random.Range(0, surface.footstepSounds.Length);
            var clip = surface.footstepSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(playPos.position)
                .Play(clip);
        }

        FootstepsSoundLibrary GetCurrentSurface()
        {
            if (surfaceLookup == null || surfaceLookup.Count == 0)
                return default;

            if (collisionController.GroundChecker.IsGrounded &&
                collisionController.GroundChecker.IsGroundHit.collider != null)
            {
                var isGetComponent = collisionController
                                        .GroundChecker
                                        .IsGroundHit
                                        .collider
                                        .TryGetComponent(out SurfaceTag surfaceTag);
                                        
                var isGetValue = surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var data);

                if (isGetComponent && isGetValue)
                    return data;
            }

            return default;
        }
    }
}
