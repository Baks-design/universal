using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.UI
{
    public class PauseMenu : MonoBehaviour //TODO: UI - Make Pause Menu
    {
        [SerializeField, Self] UIDocument uIDocument;
        VisualElement root;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;

        public bool IsPaused { get; private set; } = false;

        void Awake()
        {
            root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;

            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");
        }

        void OnEnable()
        {
            PlayerMapInputProvider.Pause.started += OnPausePressed;
            UIMapInputProvider.Unpause.started += OnResumePressed;

            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;
        }

        void OnDisable()
        {
            PlayerMapInputProvider.Pause.started -= OnPausePressed;
            UIMapInputProvider.Unpause.started -= OnResumePressed;

            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;
        }

        void OnPausePressed(InputAction.CallbackContext _)
        {
            IsPaused = true;
            Time.timeScale = 0f;
            root.style.display = DisplayStyle.Flex;
            InputServiceProvider.EnableUIMap();
            InputServiceProvider.SetCursorLocked(false);
        }

        void OnResumePressed(InputAction.CallbackContext _)
        {
            IsPaused = false;
            Time.timeScale = 1f;
            root.style.display = DisplayStyle.None;
            InputServiceProvider.EnablePlayerMap();
            InputServiceProvider.SetCursorLocked(true);
        }

        void OnOptionsPressed() { /*change to options menu;*/}

        void OnMainMenuPressed() {/*change to main menu;*/}
    }
}