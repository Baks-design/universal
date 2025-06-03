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
            { "SwitchCharacter", () => InputSystem.actions.FindAction("Player/SwitchCharacter")},
            { "UseAbility", () => InputSystem.actions.FindAction("Player/UseAbility")}
        };

        public static Vector2 MousePos => Mouse.current.position.ReadValue();
        public static InputAction SwitchCharacter => GetAction("SwitchCharacter");
        public static InputAction UseAbility => GetAction("UseAbility");

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
