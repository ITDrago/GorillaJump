using Core.Loading;
using UnityEngine;

namespace Core
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool _loadOnStart;
        [SerializeField] private int _buildIndex;
        private void Start()
        {
            if (_loadOnStart) LoadingManager.Instance.LoadScene(_buildIndex);
        }
        
        public void LoadScene(int buildIndex) => LoadingManager.Instance.LoadScene(buildIndex);
    }
}