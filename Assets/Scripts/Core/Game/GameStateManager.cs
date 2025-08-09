using Core.Audio;
using Core.Data;
using Core.Time;
using Player.Health;
using UI;
using UI.Game;
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

            TimeManager.Instance.SetTimeScale(0);
            _gameOverScreen.Show();
        }

        public void RestartGame()
        {
            TimeManager.Instance.RestoreDefaultTimeScale();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}