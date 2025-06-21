using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Systems.InteractionObjects //TODO: Adjust Trow
{
    public class ThrowController : MonoBehaviour
    {
        [SerializeField, Self] Interactor interactor;
        [SerializeField] AssetReferenceGameObject throwableObject;
        [SerializeField] Transform spawn;
        [SerializeField] ThrowConfiguration config;

        void OnEnable() => PlayerMapInputProvider.Aim.started += OnThrowStarted;

        void OnDisable() => PlayerMapInputProvider.Aim.started -= OnThrowStarted;

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