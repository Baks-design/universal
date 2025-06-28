using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universal.Runtime.Systems.InteractionObjects 
{
    public class ThrowController : MonoBehaviour
    {
        [SerializeField, Self] PickupController interactor;
        [SerializeField] AssetReferenceGameObject throwableObject;
        [SerializeField] Transform spawn;
        [SerializeField] ThrowConfiguration config;

        public void OnThrowStarted()
        {
            var proj = Addressables
                .InstantiateAsync(throwableObject, spawn.position, Quaternion.identity)
                .WaitForCompletion();
            if (proj.TryGetComponent(out ThrowableObject throwable))
                throwable.Throw(interactor.GetAimDirection * config.Force);
        }
    }
}