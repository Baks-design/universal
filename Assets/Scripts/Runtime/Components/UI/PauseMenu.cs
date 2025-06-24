using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.UI
{
    public class PauseMenu : MonoBehaviour //TODO: UI - Make Pause Menu
    {
        [SerializeField, Self] UIDocument uIDocument;
        VisualElement root;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;
        IInputServices inputServices;
        IPlayerInputReader playerInputReader;
        IUIInputReader uIInputReader;

        public bool IsPaused { get; private set; } = false;

        void Awake()
        {
            root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;

            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");
        }

        void Start()
        {
            ServiceLocator.Global.Get(out inputServices);
            ServiceLocator.Global.Get(out playerInputReader);
            ServiceLocator.Global.Get(out uIInputReader);

            playerInputReader.Pause += OnPausePressed;
            uIInputReader.Unpause += OnResumePressed;

            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;
        }

        void OnDisable()
        {
            playerInputReader.Pause -= OnPausePressed;
            uIInputReader.Unpause -= OnResumePressed;

            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;
        }

        void OnPausePressed()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            root.style.display = DisplayStyle.Flex;
            inputServices.ChangeToUIMap();
            inputServices.SetCursorLocked(false);
        }

        void OnResumePressed()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            root.style.display = DisplayStyle.None;
            inputServices.ChangeToPlayerMap();
            inputServices.SetCursorLocked(true);
        }

        void OnOptionsPressed() { /*change to options menu;*/}

        void OnMainMenuPressed() {/*change to main menu;*/}
    }
}