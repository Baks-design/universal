using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Universal.Runtime.Components.Input
{
    public static class InputServiceProvider
    {
        static readonly Dictionary<string, InputActionMap> maps = new();
        static readonly Dictionary<string, Func<InputActionMap>> MapsSetup = new()
        {
            { "Player", () => InputSystem.actions.FindActionMap("Player")},
            { "UI", () => InputSystem.actions.FindActionMap("UI")}
        };

        public static InputActionMap PlayerMap => GetAction("Player");
        public static InputActionMap UIMap => GetAction("UI");

        static InputActionMap GetAction(string mapId)
        {
            if (maps.TryGetValue(mapId, out var map))
                return map;

            Debug.LogError($"Input action {mapId} not found. Did you call InitializeActions()?");
            return null; // Instead of throwing, return null
        }

        public static void InitializeMaps()
        {
            foreach (var mapSetup in MapsSetup)
            {
                var action = mapSetup.Value();
                if (action == null)
                    Debug.LogError($"Input action {mapSetup.Key} not found! Check path: {mapSetup.Key}");
                else
                    maps[mapSetup.Key] = action;
            }
        }

        public static void EnablePlayerMap()
        {
            UIMap.Disable();
            PlayerMap.Enable();
        }

        public static void EnableUIMap()
        {
            UIMap.Enable();
            PlayerMap.Disable();
        }

        public static void SetCursorLocked(bool isSet)
        => Cursor.lockState = isSet ? CursorLockMode.Locked : CursorLockMode.None;
    }
}