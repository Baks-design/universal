using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class PlayerMapInputProvider
    {
        static readonly Dictionary<string, InputAction> actions = new();
        static readonly Dictionary<string, Func<InputAction>> ActionSetupMethods = new()
        {
            { "Move", () => InputSystem.actions.FindAction("Player/Move")},
            { "Look", () => InputSystem.actions.FindAction("Player/Look")},
            { "SwitchCharacter", () => InputSystem.actions.FindAction("Player/SwitchCharacter")},
            { "AddCharacter", () => InputSystem.actions.FindAction("Player/AddCharacter")},
            { "Pause", () => InputSystem.actions.FindAction("Player/Pause")},
        };

        public static Vector2 MousePos => Mouse.current.position.ReadValue();
        public static InputAction Move => GetAction("Move");
        public static InputAction Look => GetAction("Look");
        public static InputAction SwitchCharacter => GetAction("SwitchCharacter");
        public static InputAction AddCharacter => GetAction("AddCharacter");
        public static InputAction Pause => GetAction("Pause");

        static InputAction GetAction(string actionId)
        {
            if (actions.TryGetValue(actionId, out var action))
                return action;

            throw new KeyNotFoundException($"Input action {actionId} not found");
        }

        public static void InitializeActions()
        {
            foreach (var actionSetup in ActionSetupMethods)
                actions[actionSetup.Key] = actionSetup.Value();
        }

        public static void SetCursor(bool isSet)
        => Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
