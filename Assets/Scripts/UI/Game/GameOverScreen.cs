using Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
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
        
        public void SetRestartButtonInteractable(bool isInteractable)
        {
            if (_restartButton) _restartButton.interactable = isInteractable;
        }

        public void Show() => _panel.SetActive(true);
    }
}