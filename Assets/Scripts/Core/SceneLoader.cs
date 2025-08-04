using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool _loadOnStart;
        [SerializeField] private int _buildIndex;
        private void Start()
        {
            if (_loadOnStart) SceneManager.LoadScene(sceneBuildIndex: _buildIndex);
        }
        
        public void LoadScene(int buildIndex) => SceneManager.LoadScene(sceneBuildIndex: buildIndex);
    }
}