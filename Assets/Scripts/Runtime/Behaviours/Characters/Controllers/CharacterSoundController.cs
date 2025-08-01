using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Systems.SoundEffects;
using System.Collections.Generic;
using Alchemy.Inspector;
using static Freya.Random;
using Universal.Runtime.Utilities.Tools.Updates;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Behaviours.Characters
{
    public class CharacterSoundController : MonoBehaviour, IUpdatable
    {
        [SerializeField, Parent] CharacterController controller;
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
            InitializeConfigs();
        }

        void InitializeServices()
        {
            ServiceLocator.Global.Get(out soundService);
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

        void InitializeConfigs() => nextStepTime = Time.time + settings.walkStepInterval;

        void OnEnable() => this.AutoRegisterUpdates();

        void OnDisable() => this.AutoUnregisterUpdates();

        public void OnUpdate()
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

            if (Time.time >= nextStepTime)
            {
                var surface = GetCurrentSurface();
                if (surface != null && surface.footstepSounds.Length > 0)
                    PlayRandomSound(surface.footstepSounds);

                nextStepTime = Time.time + GetSpeedInterval();
            }
        }

        float GetSpeedInterval()
        {
            var stepInterval = 0f;
            if (movement.IsWalking) stepInterval = settings.walkStepInterval;
            else if (movement.IsRunning) stepInterval = settings.runStepInterval;
            else if (movement.IsCrouching) stepInterval = settings.crouchStepInterval;
            return stepInterval;
        }

        void HandleLanding()
        {
            if (!collision.JustLanded || !collision.IsGrounded || controller.velocity.y >= 0f) return;

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