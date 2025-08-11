using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class SceneBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
#if UNITY_EDITOR
            
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == 0)
                return;
            SceneManager.LoadScene(0);
#endif
        }
    }
}