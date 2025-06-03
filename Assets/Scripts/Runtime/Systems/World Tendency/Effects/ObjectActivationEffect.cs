using System.Collections.Generic;
using UnityEngine;

namespace Universal.Runtime.Systems.WorldTendency
{
    public class ObjectActivationEffect : ITendencyEffect
    {
        class WorldObjects
        {
            public GameObject[] WhiteObjects;
            public GameObject[] PureWhiteObjects;
            public GameObject[] BlackObjects;
            public GameObject[] PureBlackObjects;
        }
        readonly Dictionary<int, WorldObjects> worldData = new();

        public void RegisterWorld(
            int worldIndex, GameObject[] white, GameObject[] pureWhite,
            GameObject[] black, GameObject[] pureBlack)
            => worldData[worldIndex] = new WorldObjects
            {
                WhiteObjects = white,
                PureWhiteObjects = pureWhite,
                BlackObjects = black,
                PureBlackObjects = pureBlack
            };

        public void ApplyEffect(int worldIndex, TendencyState state)
        {
            if (!worldData.TryGetValue(worldIndex, out var objects)) return;
            SetObjectsActive(objects.WhiteObjects, state == TendencyState.White || state == TendencyState.PureWhite);
            SetObjectsActive(objects.PureWhiteObjects, state == TendencyState.PureWhite);
            SetObjectsActive(objects.BlackObjects, state == TendencyState.Black || state == TendencyState.PureBlack);
            SetObjectsActive(objects.PureBlackObjects, state == TendencyState.PureBlack);
        }

        void SetObjectsActive(GameObject[] objects, bool active)
        {
            for (var i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                if (obj != null)
                    obj.SetActive(active);
            }
        }
    }
}