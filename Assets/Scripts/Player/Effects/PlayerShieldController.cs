using System;
using System.Collections;
using Core.Audio;
using Skins;
using UnityEngine;

namespace Player.Effects
{
    [RequireComponent(typeof(PlayerCore))]
    public class PlayerShieldController : MonoBehaviour
    {
        [SerializeField] private GameObject _shield;
        [SerializeField] private float _shieldDuration = 20;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _shieldDeactivationSound;
        
        public bool IsShieldActive { get; private set; }
        
        public event Action<float> OnEffectStarted;
        public event Action OnEffectFinished;

        private Coroutine _shieldTimerCoroutine;

        private void Start()
        {
            if (_shield) _shield.gameObject.SetActive(false);
        }

        public void ActivateShield()
        {
            if (!_shield) return;
            
            var durationMultiplier = ActiveSkinManager.Instance.CurrentSkin?.EffectDurationMultiplier ?? 1;
            var finalDuration = _shieldDuration * durationMultiplier;

            if (IsShieldActive)
            {
                if (_shieldTimerCoroutine != null) StopCoroutine(_shieldTimerCoroutine);
                _shieldTimerCoroutine = StartCoroutine(ShieldTimer(finalDuration));
                OnEffectStarted?.Invoke(finalDuration);
                return;
            }

            IsShieldActive = true;
            _shield.gameObject.SetActive(true);
            
            _shieldTimerCoroutine = StartCoroutine(ShieldTimer(finalDuration));
            OnEffectStarted?.Invoke(finalDuration);
        }

        public void Deactivate()
        {
            if (!IsShieldActive || !_shield) return;

            if (_shieldTimerCoroutine != null)
            {
                StopCoroutine(_shieldTimerCoroutine);
                _shieldTimerCoroutine = null;
            }
            
            _shield.gameObject.SetActive(false);
            
            SoundManager.Instance.PlaySfx(_shieldDeactivationSound);
            IsShieldActive = false;
            OnEffectFinished?.Invoke();
        }

        private IEnumerator ShieldTimer(float duration)
        {
            var warningTime = 3;
            var endTime = Time.unscaledTime + duration;
            var timeLeft = duration;
            var warningTriggered = false;

            while (timeLeft > 0)
            {
                if (!warningTriggered && timeLeft <= warningTime)
                {
                    warningTriggered = true;
                }
                
                yield return null;
                timeLeft = endTime - Time.unscaledTime;
            }
            
            Deactivate();
        }
    }
}