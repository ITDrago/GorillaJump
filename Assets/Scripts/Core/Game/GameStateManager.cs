using Core.Audio;
using Core.Data;
using Player.Health;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Game
{
    public class GameStateManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private ProgressManager _progressManager;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private GameOverScreen _gameOverScreen;

        private void OnEnable() => _playerHealth.OnDied += HandlePlayerDeath;

        private void OnDisable() => _playerHealth.OnDied -= HandlePlayerDeath;

        private void HandlePlayerDeath()
        {
            var finalScore = _progressManager.BlocksPassedCount;
            ScoreSaver.SaveScore(finalScore);
            
            Time.timeScale = 0f;
            _gameOverScreen.Show();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}