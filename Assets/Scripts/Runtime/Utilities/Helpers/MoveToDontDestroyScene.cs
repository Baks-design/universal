using UnityEngine;

namespace Universal.Runtime.Systems.ScenesManagement
{
    public class MoveToDontDestroyScene : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(gameObject);
    }
}