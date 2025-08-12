using System.Collections;
using Player.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Effects
{
    public class ShieldEffectTimerUI : MonoBehaviour
    {
        [SerializeField] private PlayerShieldController _playerShieldController;
        [SerializeField] private GameObject _timerPlaceholder;
        [SerializeField] private Text _timerText;

        private Coroutine _timerCoroutine;

        private void OnEnable()
        {
            if (_playerShieldController)
            {
                _playerShieldController.OnEffectStarted += HandleEffectStarted;
                _playerShieldController.OnEffectFinished += HandleEffectFinished;
            }
        }

        private void OnDisable()
        {
            if (_playerShieldController)
            {
                _playerShieldController.OnEffectStarted -= HandleEffectStarted;
                _playerShieldController.OnEffectFinished -= HandleEffectFinished;
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
                if(_timerText) _timerText.text = timeLeft.ToString("F1");
                yield return null;
                timeLeft = endTime - Time.unscaledTime;
            }
            
            if(_timerText) _timerText.text = "0.0";
        }
    }
}