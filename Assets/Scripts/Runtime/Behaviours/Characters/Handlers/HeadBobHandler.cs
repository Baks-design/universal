using UnityEngine;
using static Freya.Mathfs;

namespace Universal.Runtime.Behaviours.Characters
{
    public class HeadBobHandler
    {
        readonly HeadBobSettings headBobSettings;
        Vector3 finalOffset;
        float xScroll;
        float yScroll;
        float currentStateHeight;
        bool resetted;

        public Vector3 FinalOffset => finalOffset;
        public bool Resetted => resetted;
        public float CurrentStateHeight
        {
            get => currentStateHeight;
            set => currentStateHeight = value;
        }

        public HeadBobHandler(HeadBobSettings headBobSettings, float moveBackwardsMultiplier, float moveSideMultiplier)
        {
            this.headBobSettings = headBobSettings;

            headBobSettings.MoveBackwardsFrequencyMultiplier = moveBackwardsMultiplier;
            headBobSettings.MoveSideFrequencyMultiplier = moveSideMultiplier;

            ResetHeadBob();
        }

        /// <summary>
        /// Updates head bob offset based on movement state.
        /// </summary>
        /// <param name="running">Whether the player is running.</param>
        /// <param name="crouching">Whether the player is crouching.</param>
        /// <param name="input">Movement input vector.</param>
        public void ScrollHeadBob(bool running, bool crouching, Vector2 input)
        {
            resetted = false;

            var amplitudeMultiplier = GetAmplitudeMultiplier(running, crouching);
            var frequencyMultiplier = GetFrequencyMultiplier(running, crouching);
            var additionalMultiplier = GetAdditionalMultiplier(input);

            xScroll += Time.deltaTime * headBobSettings.xFrequency * frequencyMultiplier;
            yScroll += Time.deltaTime * headBobSettings.yFrequency * frequencyMultiplier;

            var xValue = headBobSettings.xCurve.Evaluate(xScroll);
            var yValue = headBobSettings.yCurve.Evaluate(yScroll);

            finalOffset.x = xValue * headBobSettings.xAmplitude * amplitudeMultiplier * additionalMultiplier;
            finalOffset.y = yValue * headBobSettings.yAmplitude * amplitudeMultiplier * additionalMultiplier;
        }

        /// <summary>
        /// Resets head bob effect to initial state.
        /// </summary>
        public void ResetHeadBob()
        {
            resetted = true;
            xScroll = 0f;
            yScroll = 0f;
            finalOffset = Vector3.zero;
        }

        float GetAmplitudeMultiplier(bool running, bool crouching)
        {
            if (crouching) return headBobSettings.crouchAmplitudeMultiplier;
            if (running) return headBobSettings.runAmplitudeMultiplier;
            return 1f;
        }

        float GetFrequencyMultiplier(bool running, bool crouching)
        {
            if (crouching) return headBobSettings.crouchFrequencyMultiplier;
            if (running) return headBobSettings.runFrequencyMultiplier;
            return 1f;
        }

        float GetAdditionalMultiplier(Vector2 input)
        {
            if (Approximately(input.y, -1f))
                return headBobSettings.MoveBackwardsFrequencyMultiplier;
            if (input.x != 0f && Approximately(input.y, 0f))
                return headBobSettings.MoveSideFrequencyMultiplier;
            return 1f;
        }
    }
}