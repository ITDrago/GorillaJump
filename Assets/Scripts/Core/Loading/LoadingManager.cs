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
        [SerializeField] private Vector2Int _loadingTimeRange = new(3, 5);

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

            var view = FindFirstObjectByType<LoadingScreenView>();
            if (view)
            {
                await view.FadeIn(_fadeDuration);
            }

            var randomLoadDuration = Random.Range(_loadingTimeRange.x, _loadingTimeRange.y + 1);
            var loadingOperation = SceneManager.LoadSceneAsync(buildIndex);
            loadingOperation!.allowSceneActivation = false;

            float timer = 0;
            while (timer < randomLoadDuration)
            {
                var visualProgress = timer / randomLoadDuration;
                if (view) view.UpdateProgress(visualProgress);

                timer += UnityEngine.Time.unscaledDeltaTime;
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            if (view) 
                view.UpdateProgress(1f);

            while (loadingOperation.progress < 0.9f)
            {
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }

            if (view)
            {
                await view.FadeOut(_fadeDuration);
            }

            loadingOperation.allowSceneActivation = true;
        }
    }
}