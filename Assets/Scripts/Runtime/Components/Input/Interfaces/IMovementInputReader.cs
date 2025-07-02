using System;

namespace Universal.Runtime.Components.Input
{
    public interface IMovementInputReader
    {
        event Action OpenPauseScreen;
        event Action ToInvestigate;
        event Action ToCombat;
        event Action NextCharacter;
        event Action PreviousCharacter;
        event Action TurnRight;
        event Action TurnLeft;
        event Action MoveForward;
        event Action MoveBackward;
        event Action StrafeRight;
        event Action StrafeLeft;
    }
}