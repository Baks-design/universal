using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Utilities.Tools.ServicesLocator;

namespace Universal.Runtime.Components.UI
{
    public class MenuScreen : MonoBehaviour 
    {
        [SerializeField, Self] UIDocument uIDocument;
        //VisualElement root;
        // Button resumeButton;
        // Button optionsButton;
        // Button mainMenuButton;
        IInputReaderServices inputServices;

        void Start()
        {
            ServiceLocator.Global.Get(out inputServices);
            inputServices.ChangeToUIMap();

            //root = uIDocument.rootVisualElement;

            // resumeButton = root.Q<Button>("resume-button");
            // optionsButton = root.Q<Button>("options-button");
            // mainMenuButton = root.Q<Button>("main-menu-button");

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
    }
}