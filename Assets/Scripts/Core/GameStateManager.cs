using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private GameOverScreen _gameOverScreen;

        private void OnEnable() => _playerHealth.OnDied += HandlePlayerDeath;

        private void OnDisable() => _playerHealth.OnDied -= HandlePlayerDeath;

        private void HandlePlayerDeath()
        {
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