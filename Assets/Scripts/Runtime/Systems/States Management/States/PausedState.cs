using UnityEngine;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Systems.StatesManagement
{
    public class PausedState : IState
    {
        readonly GameStateManager gameStateManager;

        public PausedState(GameStateManager gameStateManager)
        => this.gameStateManager = gameStateManager;

        public void OnEnter()
        {
            Time.timeScale = 0f;
            gameStateManager.InputServices.ChangeToUIMap();
            gameStateManager.InputServices.SetCursorLocked(false);
        }
    }
}