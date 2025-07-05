using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Alchemy.Inspector;
using static Freya.Random;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField, Self] Transform bodyTransfom;
        [SerializeField, Parent] CharacterMovementController movementController;
        [SerializeField, InlineEditor] CharacterData data;
        [SerializeField, InlineEditor] FootstepsSoundLibrary[] soundLib;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        SoundBuilder soundBuilder;
        ISoundEffectsServices soundService;
        float lastFootstepY;

        void Awake()
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

        void Update()
        {
            PlayFootstepsSFX();
            PlayLandedSFX();
        }

        void PlayFootstepsSFX()
        {
            // Get current head bobbing Y position
            var currentYPos = movementController.CharacterHeadBobbing.CurrentYPos;
            // Check if vertical movement exceeds threshold
            if (Abs(currentYPos - lastFootstepY) <= data.footstepThreshold) return;

            // Play footstep sound for current surface
            PlayFootstepSound(GetCurrentSurface());

            // Update last tracked position
            lastFootstepY = currentYPos;
        }

        void PlayFootstepSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;

            var rnd = Range(0, surface.footstepSounds.Length);
            var clip = surface.footstepSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(bodyTransfom.localPosition)
                .Play(clip);
        }

        void PlayLandedSFX()
        {
            if (!movementController.CharacterCollision.JustLanded) return;
            PlayLandedSound(GetCurrentSurface());
        }

        void PlayLandedSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;

            var rnd = Range(0, surface.landSounds.Length);
            var clip = surface.landSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(bodyTransfom.localPosition)
                .Play(clip);
        }

        FootstepsSoundLibrary GetCurrentSurface()
        {
            if (surfaceLookup == null || surfaceLookup.Count == 0) return default;

            if (movementController.CharacterCollision.IsGrounded)
            {
                movementController.CharacterCollision.GroundHit
                    .collider.TryGetComponent(out SurfaceTag surfaceTag);
                surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var data);
                return data;
            }

            return default;
        }
    }
}
