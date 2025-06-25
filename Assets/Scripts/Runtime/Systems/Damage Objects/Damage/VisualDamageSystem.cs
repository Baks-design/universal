using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;
using UnityUtils;
using Universal.Runtime.Systems.SoundEffects;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Random = UnityEngine.Random;

namespace Universal.Runtime.Systems.DamageObjects
{
    public class VisualDamageSystem : MonoBehaviour
    {
        [SerializeField] LimbVisuals[] limbVisuals;
        [Serializable]
        class LimbVisuals
        {
            public LimbType LimbType;
            public Renderer Renderer;
            public Material HealthyMaterial;
            public Material DamagedMaterial;
            public Material CriticalMaterial;
            public AssetReferenceGameObject SeveredPrefab;
            public VisualEffect DamageParticles;
        }
        [SerializeField] VisualEffect criticalHitEffect;
        [SerializeField] SoundData criticalHitSound;
        [SerializeField] float damageFlashDuration = 0.1f;
        Dictionary<LimbType, LimbVisuals> limbVisualsMap;
        readonly Dictionary<Limb, Coroutine> flashRoutines = new();
        ISoundEffectsServices soundService;
        SoundBuilder soundBuilder;

        void Start()
        {
            ServiceLocator.Global.Get(out soundService);
            soundBuilder = soundService.CreateSoundBuilder();
        }

        public void Initialize( )
        {
            limbVisualsMap = new Dictionary<LimbType, LimbVisuals>();
            for (var i = 0; i < limbVisuals.Length; i++)
            {
                var limbVisual = limbVisuals[i];
                limbVisualsMap[limbVisual.LimbType] = limbVisual;
            }
        }

        public void ApplyVisualDamage(Limb limb, float healthPercentage, DamageSeverity severity)
        {
            if (!limbVisualsMap.TryGetValue(limb.Type, out var visuals)) return;

            // Handle critical hits
            if (severity is DamageSeverity.Critical)
                PlayCriticalEffects(limb.LimbTransform.position);

            // Handle limb severing
            if (limb.IsSevered)
            {
                visuals.Renderer.enabled = false;
                if (visuals.SeveredPrefab != null)
                {
                    var severedPart = Addressables.InstantiateAsync(
                        visuals.SeveredPrefab,
                        limb.LimbTransform.position,
                        limb.LimbTransform.rotation
                    ).WaitForCompletion();
                    if (severedPart.TryGetComponent<Rigidbody>(out var rb))
                        rb.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
                }
                return;
            }

            // Handle damage visuals
            var materialToUse = visuals.HealthyMaterial;
            switch (healthPercentage)
            {
                case < 0.3f:
                    materialToUse = visuals.DamagedMaterial;
                    break;
                default:
                    if (severity is DamageSeverity.Critical)
                        materialToUse = visuals.CriticalMaterial != null
                            ? visuals.CriticalMaterial : visuals.DamagedMaterial;
                    break;
            }

            // Flash effect
            if (flashRoutines.TryGetValue(limb, out var routine))
                StopCoroutine(routine);
            flashRoutines[limb] = StartCoroutine(DamageFlash(limb, visuals, materialToUse));

            // Damage particles
            if (visuals.DamageParticles != null)
                visuals.DamageParticles.Play();
        }

        IEnumerator DamageFlash(Limb limbKey, LimbVisuals visuals, Material targetMaterial)
        {
            var originalMaterial = visuals.Renderer.sharedMaterial;
            visuals.Renderer.material = targetMaterial;

            yield return Helpers.GetWaitForSeconds(damageFlashDuration);

            visuals.Renderer.material = originalMaterial;
            flashRoutines.Remove(limbKey);
        }

        void PlayCriticalEffects(Vector3 position)
        {
            if (criticalHitEffect != null)
                Addressables
                    .InstantiateAsync(criticalHitEffect, position, Quaternion.identity)
                    .WaitForCompletion();

            if (criticalHitSound != null)
                soundBuilder
                    .WithPosition(position)
                    .Play(criticalHitSound);
        }
    }
}