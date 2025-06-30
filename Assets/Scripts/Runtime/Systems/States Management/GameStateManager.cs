using System;
using Universal.Runtime.Components.Input;
using Universal.Runtime.Components.UI;
using Universal.Runtime.Utilities.Helpers;
using Universal.Runtime.Utilities.Tools.EventBus;
using Universal.Runtime.Utilities.Tools.ServiceLocator;
using Universal.Runtime.Utilities.Tools.StateMachine;

namespace Universal.Runtime.Systems.StatesManagement
{
    public class GameStateManager : StatefulEntity
    {
        [NonSerialized] public IInputServices InputServices;
        EventBinding<UIEvent> uiEventBinding;
        bool isPauseState;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            uiEventBinding = new EventBinding<UIEvent>(HandleUIEvent);
            EventBus<UIEvent>.Register(uiEventBinding);
        }

        void OnDisable() => EventBus<UIEvent>.Deregister(uiEventBinding);

        void HandleUIEvent(UIEvent uIEvent) => isPauseState = uIEvent.IsPaused;

        void Start()
        {
            GetServices();
            SetupStateMachine();
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

        // protected override void Update()
        // {
        //     base.Update();
        //     Logger.Info($"Current Game State: {stateMachine.CurrentState}");
        // }
    }
}