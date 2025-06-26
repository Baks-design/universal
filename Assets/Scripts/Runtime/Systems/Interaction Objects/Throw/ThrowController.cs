using KBCore.Refs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.InteractionObjects 
{
    public class ThrowController : MonoBehaviour
    {
        [SerializeField, Self] Interactor interactor;
        [SerializeField] AssetReferenceGameObject throwableObject;
        [SerializeField] Transform spawn;
        [SerializeField] ThrowConfiguration config;
        IPlayerInputReader inputReader;

        void OnEnable()
        {
            ServiceLocator.Global.Get(out inputReader);
            inputReader.Throw += OnThrowStarted;
        }

        void OnDisable() => inputReader.Throw -= OnThrowStarted;

        void OnThrowStarted()
        {
            var proj = Addressables
                .InstantiateAsync(throwableObject, spawn.position, Quaternion.identity)
                .WaitForCompletion();
            if (proj.TryGetComponent(out ThrowableObject throwable))
                throwable.Throw(interactor.GetAimDirection * config.Force);
        }
    }
}