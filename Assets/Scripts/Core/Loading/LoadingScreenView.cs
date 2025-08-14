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

        private CancellationTokenSource _cancellationSource;

        private void Awake()
        {
            _cancellationSource = new CancellationTokenSource();

            if (_progressBar) _progressBar.value = 0;
        }

        private void OnDestroy()
        {
            _cancellationSource.Cancel();
            _cancellationSource.Dispose();
        }

        public void UpdateProgress(float progress)
        {
            if (_progressBar) _progressBar.value = progress;
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