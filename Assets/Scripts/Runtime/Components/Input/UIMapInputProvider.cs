using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class UIMapInputProvider
    {
        static readonly Dictionary<string, InputAction> actions = new();
        static readonly Dictionary<string, Func<InputAction>> ActionSetupMethods = new()
        {
            { "Unpause", () => InputSystem.actions.FindAction("UI/Unpause")},
        };

        public static InputAction Unpause => GetAction("Unpause");

        static InputAction GetAction(string actionId)
        {
            if (actions.TryGetValue(actionId, out var action))
                return action;

            Debug.LogError($"Input action {actionId} not found. Did you call InitializeActions()?");
            return null; // Instead of throwing, return null
        }

        public static void InitializeActions()
        {
            foreach (var actionSetup in ActionSetupMethods)
            {
                var action = actionSetup.Value();
                if (action == null)
                    Debug.LogError($"Input action {actionSetup.Key} not found! Check path: {actionSetup.Key}");
                else
                    actions[actionSetup.Key] = action;
            }
        }
    }
}