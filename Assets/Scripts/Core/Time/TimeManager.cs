using System;
using System.Collections;
using UnityEngine;

namespace Core.Time
{
    public class TimeManager : MonoBehaviour, ITimeManager
    {
        public static ITimeManager Instance { get; private set; }

        public event Action<float> OnEffectStarted;
        public event Action OnEffectFinished;

        private Coroutine _restoreCoroutine;

        private const float DEFAULT_TIME_SCALE = 1;
        private const float DEFAULT_FIXED_DELTA_TIME = 0.02f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if ((TimeManager)Instance == this)
            {
                Instance = null;
                StopPreviousEffect();
                RestoreTimeScaleInternal();
            }
        }

        public void ApplyEffect(float slowdownFactor, float duration)
        {
            StopPreviousEffect();
            
            SetTimeScaleInternal(slowdownFactor);
            OnEffectStarted?.Invoke(duration);
            
            _restoreCoroutine = StartCoroutine(WaitAndRestore(duration));
        }

        public void SetTimeScale(float factor)
        {
            StopPreviousEffect();
            SetTimeScaleInternal(factor);
        }

        public void RestoreDefaultTimeScale()
        {
            StopPreviousEffect();
            RestoreTimeScaleInternal();
            OnEffectFinished?.Invoke();
        }

        private IEnumerator WaitAndRestore(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            _restoreCoroutine = null;
            RestoreTimeScaleInternal();
            OnEffectFinished?.Invoke();
        }

        private void StopPreviousEffect()
        {
            if (_restoreCoroutine != null)
            {
                StopCoroutine(_restoreCoroutine);
                _restoreCoroutine = null;
            }
        }

        private void SetTimeScaleInternal(float factor)
        {
            UnityEngine.Time.timeScale = factor;
            UnityEngine.Time.fixedDeltaTime = UnityEngine.Time.timeScale * DEFAULT_FIXED_DELTA_TIME;
        }

        private void RestoreTimeScaleInternal()
        {
            UnityEngine.Time.timeScale = DEFAULT_TIME_SCALE;
            UnityEngine.Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME;
        }
    }
}