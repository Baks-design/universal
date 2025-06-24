using System;
using KBCore.Refs;
using UnityEngine;
using Universal.Runtime.Utilities.Tools.ServiceLocator;

namespace Universal.Runtime.Components.Input
{
    public class InputServicesManager : MonoBehaviour, IInputServices
    {
        [NonSerialized] public GameInputs gameInputs;
        [SerializeField, Self] PlayerInputReader playerInput;
        [SerializeField, Self] UIInputReader uIInput;

        void Awake()
        {
            ServiceLocator.Global.Register<IInputServices>(this);
            DontDestroyOnLoad(gameObject);
            SetCursorLocked(true);
            EnableActions();
        }

        public void SetCursorLocked(bool isSet)
        => Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;

        public void EnableActions()
        {
            if (gameInputs == null)
            {
                gameInputs = new GameInputs();
                gameInputs.Player.SetCallbacks(playerInput);
                gameInputs.UI.SetCallbacks(uIInput);
            }

            ChangeToPlayerMap();
        }

        public void ChangeToPlayerMap()
        {
            gameInputs.Player.Enable();
            gameInputs.UI.Disable();
        }

        public void ChangeToUIMap()
        {
            gameInputs.Player.Disable();
            gameInputs.UI.Enable();
        }
    }
}