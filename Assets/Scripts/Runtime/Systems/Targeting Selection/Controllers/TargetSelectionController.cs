using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Systems.TargetingSelection
{
    public class TargetSelectionController : MonoBehaviour 
    {
        [SerializeField, Self] TargetingController targetingController;
        [SerializeField, Self] OutlineHighlightController highlightController;
        ICombatInputReader combatInput;

        void Awake() => ServiceLocator.Global.Get(out combatInput);

        void OnEnable()
        {
            targetingController.OnTargetChanged += HandleTargetChanged;
            combatInput.Target += HandleSelectTarget;
            combatInput.Selection += HandleCycleSelection;
        }

        void OnDisable()
        {
            targetingController.OnTargetChanged -= HandleTargetChanged;
            combatInput.Target -= HandleSelectTarget;
            combatInput.Selection -= HandleCycleSelection;
        }

        void HandleTargetChanged(BodyPart newTarget) => highlightController.SetHighlight(newTarget);

        void HandleSelectTarget() => targetingController.TrySelectTarget(out _);

        void HandleCycleSelection(float value) => targetingController.CycleSelection(value);
    }
}