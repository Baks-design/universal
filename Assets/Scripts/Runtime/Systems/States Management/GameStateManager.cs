using System;
using System.Collections;
using UnityEngine;
using Universal.Runtime.Behaviours.Characters;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Components.UI;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.EventBus;
using Universal.Runtime.Utilities.Tools.ServicesLocator;
using Universal.Runtime.Utilities.Tools.StateMachine;
using UnityUtils;

namespace Universal.Runtime.Systems.StatesManagement
{
    public class GameStateManager : StatefulEntity
    {
        [NonSerialized] public IInputReaderServices InputServices;
        EventBinding<UIEvent> uiEventBinding;
        bool isPauseState;
        ICharacterServices characterServices;

        void OnEnable()
        {
            uiEventBinding = new EventBinding<UIEvent>(HandleUIEvent);
            EventBus<UIEvent>.Register(uiEventBinding);
        }

        void OnDisable() => EventBus<UIEvent>.Deregister(uiEventBinding);

        void HandleUIEvent(UIEvent uIEvent) => isPauseState = uIEvent.IsPaused;

        IEnumerator Start()
        {
            GetServices();
            SetupStateMachine();
            yield return WaitFor.EndOfFrame;
            ServiceLocator.Global.Get(out characterServices);
            characterServices.AddCharacterToRoster();
        }

        void GetServices() => ServiceLocator.Global.Get(out InputServices);

        void SetupStateMachine()
        {
            var mainMenuState = new MainMenuState(this);
            var playingState = new PlayingState(this);
            var pausedState = new PausedState(this);

            At(playingState, pausedState, () => isPauseState);
            At(pausedState, playingState, () => !isPauseState);

            stateMachine.SetState(playingState);
        }

        void OnGUI()
        {
            GUIScaler.BeginScaledGUI();

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)(32 * GUIScaler.GetCurrentScale()),
                normal = { textColor = Color.white }
            };

            GUIScaler.DrawProportionalLabel(
                new Vector2(0f, 0f),
                $"Current Game State: {stateMachine.CurrentState}",
                style);

            GUI.matrix = Matrix4x4.identity;
        }
    }
}