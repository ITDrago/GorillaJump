using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private string _loadingSceneName = "LoadingScene";
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _minimumDisplayTime = 3.0f;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async void LoadScene(int buildIndex)
        {
            try { await LoadSceneRoutine(buildIndex); }
            catch { return; }
        }

        private async Awaitable LoadSceneRoutine(int buildIndex)
        {
            await SceneManager.LoadSceneAsync(_loadingSceneName);
            await Awaitable.EndOfFrameAsync(destroyCancellationToken);

            var view = (LoadingScreenView)FindFirstObjectByType(typeof(LoadingScreenView));
            if (view)
            {
                try { await view.FadeIn(_fadeDuration); }
                catch { return; }
            }

            var loadStartTime = Time.unscaledTime;
            var loadingOperation = SceneManager.LoadSceneAsync(buildIndex);
            loadingOperation!.allowSceneActivation = false;

            while (loadingOperation.progress < 0.9f)
            {
                var progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
                if (view)
                {
                    view.UpdateProgress(progress);
                }

                try { await Awaitable.NextFrameAsync(destroyCancellationToken); }
                catch { return; }
            }

            if (view)
            {
                view.UpdateProgress(1f);
            }

            var elapsedTime = Time.unscaledTime - loadStartTime;
            if (elapsedTime < _minimumDisplayTime)
            {
                try { await Awaitable.WaitForSecondsAsync(_minimumDisplayTime - elapsedTime, destroyCancellationToken); }
                catch { return; }
            }

            if (view)
            {
                try { await view.FadeOut(_fadeDuration); }
                catch { return; }
            }

            loadingOperation.allowSceneActivation = true;
        }
    }
}