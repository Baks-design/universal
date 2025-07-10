using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Alchemy.Inspector;
using static Freya.Random;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Parent] CharacterMovementController movement;
        [SerializeField, Parent] CharacterCollisionController collision;
        [SerializeField, InlineEditor] FootstepsSoundLibrary[] soundLib;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        SoundBuilder soundBuilder;
        ISoundEffectsServices soundService;
        bool hasMovedThisStep;

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
            if (movement.IsAnimating && !hasMovedThisStep)
            {
                PlayFootstepSound(GetCurrentSurface());
                hasMovedThisStep = true;
            }
            else if (!movement.IsAnimating)
                hasMovedThisStep = false;
        }

        void PlayFootstepSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;
            var rnd = Range(0, surface.footstepSounds.Length);
            var clip = surface.footstepSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(tr.localPosition)
                .Play(clip);
        }

        void PlayLandedSFX()
        {
            if (!collision.JustLanded) return;
            PlayLandedSound(GetCurrentSurface());
        }

        void PlayLandedSound(FootstepsSoundLibrary surface)
        {
            if (surface == null) return;
            var rnd = Range(0, surface.landSounds.Length);
            var clip = surface.landSounds[rnd];
            soundBuilder
                .WithRandomPitch()
                .WithPosition(tr.localPosition)
                .Play(clip);
        }

        FootstepsSoundLibrary GetCurrentSurface()
        {
            if (surfaceLookup == null || surfaceLookup.Count == 0) return default;

            if (collision.IsGrounded)
            {
                collision.GroundHit.collider.TryGetComponent(out SurfaceTag surfaceTag);
                surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var data);
                return data;
            }

            return default;
        }
    }
}
