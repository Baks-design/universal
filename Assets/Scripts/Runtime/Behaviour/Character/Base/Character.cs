using UnityEngine;
using Universal.Runtime.Systems.ManagedUpdate;

namespace Universal.Runtime.Behaviours.Characters
{
    public abstract class Character : AManagedBehaviour
    {
        [SerializeField] CharacterDataSO characterData;
        protected ICharacterState currentState;

        public CharacterDataSO Data => characterData;
        protected AbilityDataSO Ability => characterData.ability; 

        public virtual void Activate() => SetState(new IdleState());
        public virtual void Deactivate() => SetState(new DisabledState());
        public virtual void UseAbility()
        {
            if (Ability != null)
                Ability.UseAbility(this);
        }

        public abstract void Move();

        public void SetState(ICharacterState newState)
        {
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
        }
    }
}