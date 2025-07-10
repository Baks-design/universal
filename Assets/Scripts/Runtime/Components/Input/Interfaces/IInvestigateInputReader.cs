using System;
using UnityEngine;

namespace Universal.Runtime.Components.Input
{
    public interface IInvestigateInputReader
    {
        event Action OpenPauseScreen;
        event Action ToMovement;
        event Action ToCombat;
        event Action AddCharacter;
        event Action RemoveCharacter;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action<Vector2> Look;
        event Action Aim;
        event Action Interact;
    }
}