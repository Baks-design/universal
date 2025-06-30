using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.EventBus;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.UI
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField, Self] UIDocument uIDocument;
        VisualElement root;
        IPlayerInputReader playerInputReader;
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