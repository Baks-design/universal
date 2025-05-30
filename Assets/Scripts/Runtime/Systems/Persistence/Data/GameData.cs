using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.Persistence.Data
{
    [Serializable]
    public class GameData
    {
        [Header("Game Settings")]
        public string Name;
        public string CurrentLevelName;

        [Header("Entities Settings")]
        [NonSerialized] public List<EntityData> EntityData = new();
    }
}