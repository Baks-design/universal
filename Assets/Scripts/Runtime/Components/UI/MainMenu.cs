using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;

namespace Universal.Runtime.Components.UI
{
    public class MainMenu : MonoBehaviour //TODO: UI - Make Main Menu
    {
        [SerializeField, Self] UIDocument uIDocument;
        //VisualElement root;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;

        void Awake()
        {
            //root = uIDocument.rootVisualElement;

            // resumeButton = root.Q<Button>("resume-button");
            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");
        }

        void OnEnable()
        {
            // resumeButton.clicked += OnResumeClicked; 
            // optionsButton.clicked += OnOptionsClicked;
            // mainMenuButton.clicked += OnMainMenuClicked;
        }

        void OnDisable()
        {
            // resumeButton.clicked -= OnResumeClicked; 
            // optionsButton.clicked -= OnOptionsClicked;
            // mainMenuButton.clicked -= OnMainMenuClicked;
        }

        void OnNewGameClicked() => Debug.Log("New game clicked");

        void OnLoadClicked() => Debug.Log("Load game clicked");

        void OnOptionsClicked() => Debug.Log("Options clicked");

        void OnQuitClicked() => Debug.Log("Quit clicked");

        void Start() => InputServiceProvider.EnableUIMap();
    }
}