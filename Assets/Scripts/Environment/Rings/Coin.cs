using DG.Tweening;
using UnityEngine;

namespace Environment.Rings
{
    public class Coin : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _moveDistance = 0.3f;
        [SerializeField] private float _jumpDuration = 0.8f;
        [SerializeField] private Ease _easeType = Ease.InOutCubic;
        [SerializeField] private float _loopDelay = 0.5f;

        [Header("Squash & Stretch Settings")]
        [SerializeField] private bool _enableSquashAndStretch = true;
        [SerializeField] private float _squashFactor = 0.85f;
        [SerializeField] private float _stretchFactor = 1.15f;
        
        private Sequence _animationSequence;
        private Vector3 _startPosition;
        private Vector3 _startScale;

        private void Start()
        {
            _startPosition = transform.localPosition;
            _startScale = transform.localScale;
            StartAnimation();
        }

        private void OnDestroy() => _animationSequence?.Kill();

        private void StartAnimation()
        {
            _animationSequence = DOTween.Sequence();
            var halfDuration = _jumpDuration / 2f;

            _animationSequence.Append(
                transform.DOLocalMoveY(_startPosition.y + _moveDistance, halfDuration)
                    .SetEase(_easeType)
            );

            if (_enableSquashAndStretch)
            {
                _animationSequence.Join(
                    transform.DOScaleY(_startScale.y * _squashFactor, halfDuration)
                        .SetEase(Ease.InSine)
                );
            }

            _animationSequence.Append(
                transform.DOLocalMoveY(_startPosition.y, halfDuration)
                    .SetEase(_easeType)
            );

            if (_enableSquashAndStretch)
            {
                var landingScaleSequence = DOTween.Sequence();
                landingScaleSequence.Append(transform.DOScaleY(_startScale.y * _stretchFactor, halfDuration * 0.5f).SetEase(Ease.OutSine));
                landingScaleSequence.Append(transform.DOScaleY(_startScale.y, halfDuration * 0.5f).SetEase(Ease.InSine));
                
                _animationSequence.Join(landingScaleSequence);
            }

            _animationSequence.AppendInterval(_loopDelay);
            _animationSequence.SetLoops(-1);
        }
    }
}