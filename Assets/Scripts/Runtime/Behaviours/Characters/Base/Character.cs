using UnityEngine;
using Universal.Runtime.Systems.ManagedUpdate;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class Character : AManagedBehaviour, IUpdatable, ICharacter, IAbility, IMovement
    {
        [SerializeField] protected CharacterDataSO characterData;
        protected Transform tr;
        protected ICharacterState currentState;
        protected bool isActive = false;

        public CharacterDataSO Data => characterData;

        void Start() => tr = transform;

        public abstract void UseAbility();

        void IUpdatable.ManagedUpdate(float deltaTime, float time)
        {
            if (!isActive) return;

            currentState?.Update(this);
            ProcessMovement(deltaTime);
        }

        public abstract void ProcessMovement(float deltaTime);

        public virtual void Activate()
        {
            isActive = true;
            SetState(new IdleState());
        }

        public virtual void Deactivate()
        {
            isActive = false;
            SetState(new DisabledState());
        }

        void SetState(ICharacterState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
        }
    }
}