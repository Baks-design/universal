using System;
using UnityEngine;

namespace Universal.Runtime.Behaviours.Characters
{
    [Serializable]
    public class HeadBobSettings
    {
        [Header("Curves")]
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;

        [Header("Amplitude")]
        public float xAmplitude;
        public float yAmplitude;

        [Header("Frequency")]
        public float xFrequency;
        public float yFrequency;

        [Header("Run Multipliers")]
        public float runAmplitudeMultiplier;
        public float runFrequencyMultiplier;

        [Header("Crouch Multipliers")]
        public float crouchAmplitudeMultiplier;
        public float crouchFrequencyMultiplier;

        public float MoveBackwardsFrequencyMultiplier { get; set; }
        public float MoveSideFrequencyMultiplier { get; set; }
    }
}