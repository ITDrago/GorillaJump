using System;

namespace Core.Time
{
    public interface ITimeManager
    {
        public event Action<float> OnEffectStarted;
        public event Action OnEffectFinished;

        void ApplyEffect(float slowdownFactor, float duration);
        void SetTimeScale(float factor);
        void RestoreDefaultTimeScale();
    }
}