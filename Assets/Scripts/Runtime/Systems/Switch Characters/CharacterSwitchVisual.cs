using System.Collections;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.VFX;

namespace Universal.Runtime.Systems.SwitchCharacters
{
    public class CharacterSwitchVisual : MonoBehaviour
    {
        [SerializeField, Self] CharacterManager characterManager;
        IEnumerator switchEffectCoroutine;
        const float dissolveRatio = 0.5f;

        public void HandleChangedEffect(int index, Character currentCharacter)
        {
            if (switchEffectCoroutine != null)
                StopCoroutine(switchEffectCoroutine);

            switchEffectCoroutine = SwitchEffectRoutine(index, currentCharacter);
            StartCoroutine(switchEffectCoroutine);
        }

        IEnumerator SwitchEffectRoutine(int index, Character currentCharacter)
        {
            currentCharacter.TryGetComponent<DissolveEffect>(out var dissolve);
            if (dissolve == null || currentCharacter.Data == null)
                yield break;

            yield return dissolve.PlayDissolve(dissolveRatio, currentCharacter.Data.dissolveMaterial);

            currentCharacter.Deactivate();
            characterManager.CurrentIndex = index;
            currentCharacter.Activate();

            yield return dissolve.PlayDissolve(dissolveRatio, currentCharacter.Data.dissolveMaterial, true);

            CharacterEvents.RaiseCharacterSwitched(currentCharacter);
        }
    }
}