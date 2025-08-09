using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Loading
{
    public class LoadingScreenView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private Slider _progressBar;

        [Header("World Elements")]
        [SerializeField] private Transform _heroTransform;
        [SerializeField] private Transform _heroStartPoint;
        [SerializeField] private Transform _heroEndPoint;

        private CancellationTokenSource _cancellationSource;

        private void Awake()
        {
            _cancellationSource = new CancellationTokenSource();

            if (_progressBar) _progressBar.value = 0;

            if (_heroTransform && _heroStartPoint) _heroTransform.position = _heroStartPoint.position;
        }

        private void OnDestroy()
        {
            _cancellationSource.Cancel();
            _cancellationSource.Dispose();
        }

        public void UpdateProgress(float progress)
        {
            if (_progressBar)
            {
                _progressBar.value = progress;
            }

            if (_heroTransform && _heroStartPoint && _heroEndPoint)
            {
                _heroTransform.position = Vector3.Lerp(_heroStartPoint.position, _heroEndPoint.position, progress);
            }
        }

        public Awaitable FadeIn(float duration) => Fade(_mainCanvasGroup, 1f, duration, _cancellationSource.Token);

        public Awaitable FadeOut(float duration) => Fade(_mainCanvasGroup, 0f, duration, _cancellationSource.Token);

        private async Awaitable Fade(CanvasGroup targetGroup, float targetAlpha, float duration, CancellationToken cancellationToken)
        {
            var startAlpha = targetGroup.alpha;
            var time = 0f;

            while (time < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                
                targetGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                time += UnityEngine.Time.unscaledDeltaTime;
                await Awaitable.NextFrameAsync(cancellationToken);
            }

            targetGroup.alpha = targetAlpha;
        }
    }
}