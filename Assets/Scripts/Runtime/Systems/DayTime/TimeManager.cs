using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Universal.Runtime.Systems.Update.Interface;
using Universal.Runtime.Utilities.Tools;

namespace Universal.Runtime.Systems.DayTime
{
    public class TimeManager : PersistentSingleton<TimeManager>, IUpdatable
    {
        [SerializeField] Light directional;
        [SerializeField] AnimationCurve lightIntensityCurve;
        [SerializeField] float maxSunIntensity = 1f;
        [SerializeField] float maxMoonIntensity = 0.5f;
        [SerializeField] Color dayAmbientLight;
        [SerializeField] Color nightAmbientLight;
        [SerializeField] Volume volume;
        [SerializeField] Material skyboxMaterial;
        [SerializeField, InLineEditor] TimeSettings timeSettings;
        TimeService service;
        ColorAdjustments colorAdjustments;
        readonly int _Blend = Shader.PropertyToID("_Blend");

        public event Action OnSunrise
        {
            add => service.OnSunrise += value;
            remove => service.OnSunrise -= value;
        }
        public event Action OnSunset
        {
            add => service.OnSunset += value;
            remove => service.OnSunset -= value;
        }
        public event Action OnHourChange
        {
            add => service.OnHourChange += value;
            remove => service.OnHourChange -= value;
        }

        protected override void Awake()
        {
            base.Awake();

            service = new TimeService(timeSettings);

            volume.profile.TryGet(out colorAdjustments);

            OnSunrise += () => Debug.Log("Sunrise");
            OnSunset += () => Debug.Log("Sunset");
            OnHourChange += () => Debug.Log("Hour change");
        }

        void IUpdatable.ManagedUpdate(float deltaTime)
        {
            RotateSun();
            UpdateLightSettings();
            UpdateSkyBlend();
            UpdateTimeOfDay(deltaTime);
        }

        void UpdateSkyBlend()
        {
            var dotProduct = Vector3.Dot(directional.transform.forward, Vector3.up);
            var blend = Mathf.Lerp(0f, 1f, lightIntensityCurve.Evaluate(dotProduct));
            skyboxMaterial.SetFloat(_Blend, blend);
        }

        void UpdateLightSettings()
        {
            var dotProduct = Vector3.Dot(directional.transform.forward, Vector3.down);
            var lightIntensity = lightIntensityCurve.Evaluate(dotProduct);

            var dayLight = Mathf.Lerp(0f, maxSunIntensity, lightIntensity);
            var nightLight = Mathf.Lerp(0f, maxMoonIntensity, lightIntensity);
            directional.intensity = service.IsDayTime ? dayLight : nightLight;

            colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);
        }

        void RotateSun()
        => directional.transform.rotation = Quaternion.AngleAxis(service.CalculateSunAngle(), Vector3.right);

        void UpdateTimeOfDay(float deltaTime) => service.UpdateTime(deltaTime);
    }
}