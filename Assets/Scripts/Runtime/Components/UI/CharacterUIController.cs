using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal.Runtime.Behaviours.Characters;

namespace Universal.Runtime.Components.UI
{
    public class CharacterUIController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] Image abilityIcon;

        void OnEnable() => CharacterEvents.OnCharacterSwitched += UpdateUI;

        void OnDestroy() => CharacterEvents.OnCharacterSwitched -= UpdateUI;

        void UpdateUI(Character character)
        {
            nameText.text = character.Data.characterName;
            abilityIcon.sprite = character.Data.icon;
        }
    }
}