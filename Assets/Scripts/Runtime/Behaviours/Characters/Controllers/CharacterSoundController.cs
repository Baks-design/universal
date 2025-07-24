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
        [SerializeField] CharacterSoundSettings settings;
        [SerializeField, InlineEditor] FootstepsSoundLibrary[] soundLibraries;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        ISoundEffectsServices soundService;
        SoundBuilder soundBuilder;
        float nextStepTime;

        void Awake()
        {
            InitializeServices();
            BuildSurfaceDictionary();
        }

        void InitializeServices()
        {
            ServiceLocator.Global.TryGet(out soundService);
            soundBuilder = soundService.CreateSoundBuilder();
        }

        void BuildSurfaceDictionary()
        {
            surfaceLookup = new Dictionary<SurfaceType, FootstepsSoundLibrary>(soundLibraries.Length);
            for (var i = 0; i < soundLibraries.Length; i++)
            {
                var library = soundLibraries[i];
                if (library != null && !surfaceLookup.ContainsKey(library.surfaceType))
                    surfaceLookup[library.surfaceType] = library;
            }
        }

        void Update()
        {
            HandleFootsteps();
            HandleLanding();
        }

        void HandleFootsteps()
        {
            if (!movement.IsMoving)
            {
                nextStepTime = Time.time;
                return;
            }

            var stepInterval = movement.IsRunning ? settings.runStepInterval : settings.walkStepInterval;

            if (Time.time >= nextStepTime)
            {
                var surface = GetCurrentSurface();
                if (surface != null && surface.footstepSounds.Length > 0)
                    PlayRandomSound(surface.footstepSounds);

                nextStepTime = Time.time + stepInterval;
            }
        }

        void HandleLanding()
        {
            if (!collision.JustLanded || !collision.IsGrounded) return;

            var surface = GetCurrentSurface();
            if (surface != null && surface.landSounds.Length > 0)
                PlayRandomSound(surface.landSounds);
        }

        void PlayRandomSound(SoundData[] sounds)
        {
            if (sounds == null || sounds.Length == 0 || soundService == null) return;

            var index = Range(0, sounds.Length);
            soundBuilder
               .WithRandomPitch()
               .WithPosition(tr.localPosition)
               .Play(sounds[index]);
        }

        FootstepsSoundLibrary GetCurrentSurface()
        {
            if (!collision.IsGrounded || surfaceLookup == null || surfaceLookup.Count == 0)
                return null;

            if (collision.GroundHit.collider.TryGetComponent(out SurfaceTag surfaceTag))
            {
                surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var library);
                return library;
            }

            return null;
        }
    }
}