using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class ThrowController : MonoBehaviour
    {
        [SerializeField, Self] Interactor interactor;
        [SerializeField] AssetReferenceGameObject throwableObject;
        [SerializeField] Transform spawn;
        [SerializeField] ThrowConfiguration config;

        void OnEnable() => PlayerMapInputProvider.Throw.started += OnThrowStarted;

        void OnDisable() => PlayerMapInputProvider.Throw.started -= OnThrowStarted;

        void OnThrowStarted(InputAction.CallbackContext context)
        {
            var proj = Addressables
                .InstantiateAsync(throwableObject, spawn.position, Quaternion.identity)
                .WaitForCompletion();
            if (proj.TryGetComponent(out ThrowableObject throwable))
                throwable.Throw(interactor.GetAimDirection * config.Force);
        }
    }
}