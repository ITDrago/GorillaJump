using UI.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Advertisement;
using Core.Time;

namespace Core.Game
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private Player.Health.PlayerHealth _playerHealth;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private InterstitialAd _interstitialAd;

        [SerializeField] private int _lossesToShowAD = 3;
        
        private static int _sLossCounter;
        
        private void OnEnable()
        {
            _playerHealth.OnDied += HandlePlayerDeath;
            _interstitialAd.OnAdFlowFinished += HandleAdFlowFinished;
        }

        private void OnDisable()
        {
            _playerHealth.OnDied -= HandlePlayerDeath;
            _interstitialAd.OnAdFlowFinished -= HandleAdFlowFinished;
        }

        private void HandlePlayerDeath()
        {
            TimeManager.Instance.SetTimeScale(0);
            _gameOverScreen.Show();
            
            _sLossCounter++;

            if (_sLossCounter >= _lossesToShowAD)
            {
                _sLossCounter = 0;
                
                _gameOverScreen.SetRestartButtonInteractable(false); 
                
                _interstitialAd.ShowInterstitial();
            }
        }

        private void HandleAdFlowFinished() => _gameOverScreen.SetRestartButtonInteractable(true);

        public void RestartGame()
        {
            TimeManager.Instance.SetTimeScale(1);
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    }
}