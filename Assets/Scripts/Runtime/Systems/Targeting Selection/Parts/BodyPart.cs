using KBCore.Refs;
using UnityEngine;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class BodyPart : MonoBehaviour, IHighlightable, IDamageablePart
    {
        [SerializeField, Self] Collider partCollider;
        [SerializeField, Self] Renderer rend;
       // [SerializeField, Self] Material highlightMaterial;
        [SerializeField] string partName;
        [SerializeField] float hitMultiplier = 1f;
        [SerializeField] float baseHitChance = 0.8f;
        //Material originalMaterial;

        public Collider PartCollider => partCollider;
        public float HitMultiplier => hitMultiplier;

        //void Awake() => originalMaterial = rend.sharedMaterial;

        public float CalculateHitChance(float playerAccuracy) => baseHitChance * playerAccuracy;

        public void Highlight()
        {
            //rend.material = highlightMaterial;
        }

        public void Unhighlight()
        {
            //rend.material = originalMaterial;
        }

        public void TakeDamage(float amount)
        => Debug.Log($"{partName} took {amount * hitMultiplier} damage!");
    }
}