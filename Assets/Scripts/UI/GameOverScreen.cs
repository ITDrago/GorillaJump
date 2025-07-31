using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private GameStateManager _gameStateManager;

        private void Awake()
        {
            _panel.SetActive(false);
            _restartButton.onClick.AddListener(_gameStateManager.RestartGame);
        }

        public void Show() => _panel.SetActive(true);
    }
}