using Core.Audio;
using Core.Game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class UnlockChargedJumpNotifier : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Text _unlockText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _scaleUpSize = 3.9f;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        [SerializeField] private Ease _easeType = Ease.InOutExpo;

        [Header("Audio")]
        [SerializeField] private AudioClip _unlockSound;
        
        [Header("References")]
        [SerializeField] private ProgressManager _progressManager;

        private void Start()
        {
            if (!_canvasGroup) return;
            _canvasGroup.alpha = 0;

            if (_progressManager) _progressManager.OnChargedJumpUnlocked += ShowUnlockNotification;
        }

        private void ShowUnlockNotification()
        {
            if (!_unlockText || !_canvasGroup) return;
            
            PlayUnlockSound();
            
            var sequence = DOTween.Sequence();
            
            sequence.Append(_canvasGroup.DOFade(1, _fadeInDuration))
                .Join(transform.DOScale(_scaleUpSize, _fadeInDuration).SetEase(_easeType))
                .AppendInterval(_displayDuration)
                .Append(_canvasGroup.DOFade(0, _fadeOutDuration))
                .OnComplete(() => transform.localScale = Vector3.one);
        }

        private void PlayUnlockSound()
        {
            if (SoundManager.Instance && _unlockSound) SoundManager.Instance.PlaySfx(_unlockSound, 0.6f);
        }
        
        private void OnDestroy()
        {
            if(_progressManager) _progressManager.OnChargedJumpUnlocked -= ShowUnlockNotification;
        }
    }
}