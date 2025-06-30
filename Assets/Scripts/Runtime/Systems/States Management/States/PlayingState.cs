using UnityEngine;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Systems.StatesManagement
{
    public class PlayingState : IState
    {
        readonly GameStateManager gameStateManager;

        public PlayingState(GameStateManager gameStateManager)
        => this.gameStateManager = gameStateManager;

        public void OnEnter()
        {
            Time.timeScale = 1f;
            gameStateManager.InputServices.ChangeToPlayerMap();
            gameStateManager.InputServices.SetCursorLocked(true);
        }
    }
}