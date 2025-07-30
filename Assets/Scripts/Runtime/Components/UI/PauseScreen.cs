using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.EventBus;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Components.UI
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField, Self] UIDocument uIDocument;
        VisualElement root;
        IMovementInputReader movementInput;
        IInvestigateInputReader investigateInput;
        ICombatInputReader combatInput;
        IUIInputReader uIInputReader;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;

        void Awake()
        {
            root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;
            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");
        }

        void Start()
        {
            GetServices();
            RegisterInputs();
        }

        void OnDisable() => UnregisterInputs();

        void GetServices()
        {
            ServiceLocator.Global.Get(out movementInput);
            ServiceLocator.Global.Get(out investigateInput);
            ServiceLocator.Global.Get(out combatInput);
            ServiceLocator.Global.Get(out uIInputReader);
        }

        void RegisterInputs()
        {
            movementInput.OpenPauseScreen += OnPausePressed;
            investigateInput.OpenPauseScreen += OnPausePressed;
            combatInput.OpenPauseScreen += OnPausePressed;
            uIInputReader.ClosePauseScreen += OnResumePressed;
            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;
        }

        void UnregisterInputs()
        {
            movementInput.OpenPauseScreen -= OnPausePressed;
            investigateInput.OpenPauseScreen -= OnPausePressed;
            combatInput.OpenPauseScreen -= OnPausePressed;
            uIInputReader.ClosePauseScreen -= OnResumePressed;
            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;
        }

        void OnPausePressed()
        {
            EventBus<UIEvent>.Raise(new UIEvent { IsPaused = true });
            root.style.display = DisplayStyle.Flex;
        }

        void OnResumePressed()
        {
            EventBus<UIEvent>.Raise(new UIEvent { IsPaused = false });
            root.style.display = DisplayStyle.None;
        }

        //void OnOptionsPressed() { /*change to options menu;*/}

        //void OnMainMenuPressed() {/*change to main menu;*/}
    }
}