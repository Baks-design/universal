using System.Collections;
using UnityEngine;

namespace Universal.Runtime.Components.VFX
{
    public class DissolveEffect : MonoBehaviour
    {
        readonly int _DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        public IEnumerator PlayDissolve(float duration, Material material, bool reverse = false)
        {
            var start = reverse ? 1f : 0f;
            var end = reverse ? 0f : 1f;
            var timer = 0f;

            while (timer < duration)
            {
                var dissolveAmount = Mathf.Lerp(start, end, timer / duration);
                material.SetFloat(_DissolveAmount, dissolveAmount); 
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}