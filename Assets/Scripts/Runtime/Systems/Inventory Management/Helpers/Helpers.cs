using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static Freya.Mathfs;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public static class Helpers
    {
        public static Guid CreateGuidFromString(string input)
        => new(MD5.Create().ComputeHash(Encoding.Default.GetBytes(input)));

        public static Vector2 ClampToScreen(VisualElement element, Vector2 targetPosition)
        {
            var x = Clamp(targetPosition.x, 0f, Screen.width - element.layout.width);
            var y = Clamp(targetPosition.y, 0f, Screen.height - element.layout.height);
            return new Vector2(x, y);
        }
    }
}