using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.InteractionObjects
{
    public class ThrowController : MonoBehaviour
    {
        [SerializeField, Self] PickupController interactor;
        [SerializeField] AssetReferenceGameObject throwableObject;
        [SerializeField] Transform spawn;
        [SerializeField] ThrowConfiguration config;
        IInvestigateInputReader input;

        void Awake() => ServiceLocator.Global.Get(out input);

        void OnEnable() => input.Interact += OnThrow;

        void OnDisable() => input.Interact -= OnThrow;

        void OnThrow()
        {
            var proj = Addressables
                .InstantiateAsync(throwableObject, spawn.localPosition, Quaternion.identity)
                .WaitForCompletion();
            if (proj.TryGetComponent(out ThrowableObject throwable))
                throwable.Throw(interactor.GetAimDirection * config.Force);
        }
    }
}