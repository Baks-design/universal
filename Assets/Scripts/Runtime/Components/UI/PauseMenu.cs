using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField, Self] UIDocument uIDocument;
        VisualElement root;
        Button resumeButton;
        Button optionsButton;
        Button mainMenuButton;

        public bool IsPaused { get; private set; } = false;

        void OnEnable()
        {
            // Get the root VisualElement
            root = uIDocument.rootVisualElement;
            // Hide the menu initially
            root.style.display = DisplayStyle.None;

            // Query the buttons
            resumeButton = root.Q<Button>("resume-button");
            optionsButton = root.Q<Button>("options-button");
            mainMenuButton = root.Q<Button>("main-menu-button");

            // Register button callbacks
            // resumeButton.clicked += OnResumeClicked; //TODO:Pause Menu
            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;

            PlayerMapInputProvider.Pause.started += OnPausePressed;
            UIMapInputProvider.Unpause.started += OnResumePressed;
        }

        void OnDisable()
        {
            // Unregister button callbacks
            // resumeButton.clicked -= OnResumeClicked;
            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;

            PlayerMapInputProvider.Pause.started -= OnPausePressed;
            UIMapInputProvider.Unpause.started -= OnResumePressed;
        }

        void OnDestroy()
        {
            // Unregister button callbacks
            // resumeButton.clicked -= OnResumeClicked;
            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;

            PlayerMapInputProvider.Pause.started -= OnPausePressed;
            UIMapInputProvider.Unpause.started -= OnResumePressed;
        }

        void OnPausePressed(InputAction.CallbackContext _)
        {
            IsPaused = true;
            Time.timeScale = 0f;
            root.style.display = DisplayStyle.Flex;
            InputServiceProvider.EnableUIMap();
            InputServiceProvider.SetCursorLocked(false);
            Debug.Log("Opened Menu");
        }

        void OnResumePressed(InputAction.CallbackContext _) => ResumeGame();

        void OnResumeClicked() => ResumeGame();

        void ResumeGame()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            root.style.display = DisplayStyle.None;
            InputServiceProvider.EnablePlayerMap();
            InputServiceProvider.SetCursorLocked(true);
            Debug.Log("Closed Menu");
        }

        void OnOptionsClicked()
        {
            // Implement options menu logic
            Debug.Log("Options clicked");
        }

        void OnMainMenuClicked()
        {
            // Return to main menu
            Time.timeScale = 1f;
            //SceneManager.LoadScene("MainMenu");
        }
    }
}