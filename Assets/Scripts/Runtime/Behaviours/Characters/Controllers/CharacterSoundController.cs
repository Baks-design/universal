using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Alchemy.Inspector;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField, Self] Transform bodyTransfom;
        [SerializeField, Parent] CharacterMovementController movementController;
        [SerializeField, InlineEditor] CharacterData data;
        [SerializeField, InlineEditor] FootstepsSoundLibrary[] soundLib;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        SoundBuilder soundBuilder = null;
        ISoundEffectsServices soundService = null;
        float stepTimer = 0f;

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
            surfaceLookup = new Dictionary<SurfaceType, FootstepsSoundLibrary>();
            for (var i = 0; i < soundLib.Length; i++)
                surfaceLookup[soundLib[i].surfaceType] = soundLib[i];
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
                return data.walkStepInterval;
            return currentStep;
        }

        void PlayFootstepSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;

            var rnd = Random.Range(0, surface.footstepSounds.Length);
            var clip = surface.footstepSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(bodyTransfom.localPosition)
                .Play(clip);
        }

        FootstepsSoundLibrary GetCurrentSurface()
        {
            if (surfaceLookup == null || surfaceLookup.Count == 0)
                return default;

            if (Physics.Raycast(
                bodyTransfom.position, Vector3.down, out var hit,
                data.footstepsDistance, data.floorMask))
            {
                hit.collider.TryGetComponent(out SurfaceTag surfaceTag);
                surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var data);
                return data;
            }

            return default;
        }
    }
}
