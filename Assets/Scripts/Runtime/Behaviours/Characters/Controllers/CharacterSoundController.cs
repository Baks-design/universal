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
        [SerializeField] CharacterData data;
        [SerializeField, InlineEditor] FootstepsSoundLibrary[] soundLib;
        Dictionary<SurfaceType, FootstepsSoundLibrary> surfaceLookup;
        SoundBuilder soundBuilder;
        ISoundEffectsServices soundService;
        float lastFootstepY;

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

        void Update()
        {
            PlayFootstepsSFX();
            PlayLandedSFX();
        }

        public void PlayFootstepsSFX()
        {
            var result = Abs(movementController.CharacterHeadBobbing.CurrentYPos - lastFootstepY) <= data.footstepThreshold;
            if (result) return;
            PlayFootstepSound(GetCurrentSurface());
            lastFootstepY = movementController.CharacterHeadBobbing.CurrentYPos;
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

            if (Physics.Raycast(
                bodyTransfom.position, Vector3.down, out var hit,
                data.footstepsRayDistance, data.floorMask))
            {
                hit.collider.TryGetComponent(out SurfaceTag surfaceTag);
                surfaceLookup.TryGetValue(surfaceTag.SurfaceType, out var data);
                return data;
            }

            return default;
        }
    }
}
