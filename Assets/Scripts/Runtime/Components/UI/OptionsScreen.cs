using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.UI
{
    public class OptionsScreen: MonoBehaviour
    {
        [SerializeField, Self] UIDocument document;
        VisualElement root;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;

        public bool IsPaused { get; private set; } = false;

        void Awake()
        {
            root = document.rootVisualElement;
            root.style.display = DisplayStyle.None;

            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");
        }

        void OnEnable()
        {
            //UIMapInputProvider.Unpause.started += OnBackPressed;

            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;
        }

        void OnDisable()
        {
            //UIMapInputProvider.Unpause.started -= OnBackPressed;

            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;
        }

        void OnBackPressed(InputAction.CallbackContext _) { }

        void OnOptionsPressed() { /*change to options menu;*/}

        void OnMainMenuPressed() {/*change to main menu;*/}
    }
}