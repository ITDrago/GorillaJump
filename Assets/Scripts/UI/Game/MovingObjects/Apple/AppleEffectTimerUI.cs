using System.Collections;
using Core.Time;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.MovingObjects.Apple
{
    public class AppleEffectTimerUI : MonoBehaviour
    {
        [SerializeField] private GameObject _timerPlaceholder;
        [SerializeField] private Text _timerText;

        private Coroutine _timerCoroutine;

        private void OnEnable()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnEffectStarted += HandleEffectStarted;
                TimeManager.Instance.OnEffectFinished += HandleEffectFinished;
            }
        }

        private void OnDisable()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnEffectStarted -= HandleEffectStarted;
                TimeManager.Instance.OnEffectFinished -= HandleEffectFinished;
            }
        }

        private void Start()
        {
            if (_timerPlaceholder)
            {
                _timerPlaceholder.SetActive(false);
            }
        }

        private void HandleEffectStarted(float duration)
        {
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            
            if (_timerPlaceholder)
            {
                _timerPlaceholder.SetActive(true);
            }
            
            _timerCoroutine = StartCoroutine(Countdown(duration));
        }

        private void HandleEffectFinished()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            
            if (_timerPlaceholder) _timerPlaceholder.SetActive(false);
        }

        private IEnumerator Countdown(float duration)
        {
            var endTime = Time.unscaledTime + duration;
            var timeLeft = duration;

            while (timeLeft > 0)
            {
                _timerText.text = timeLeft.ToString("F1");
                yield return null;
                timeLeft = endTime - Time.unscaledTime;
            }

            _timerText.text = "0.0";
        }
    }
}