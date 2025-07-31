using Player;
using UnityEngine;
using UnityEngine.UI;
using Environment.Blocks.BlockTypes;

namespace UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private PlayerController _playerController;

        private int _currentScore;
        private Block _lastLandedBlock;

        private void OnEnable()
        {
            if (_playerController) _playerController.OnLanded += HandlePlayerLanded;
        }

        private void OnDisable()
        {
            if (_playerController) _playerController.OnLanded -= HandlePlayerLanded;
        }

        private void Start()
        {
            _currentScore = 0;
            _lastLandedBlock = _playerController.StartBlock; 
            UpdateScoreText();
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            if (landedBlock != _lastLandedBlock)
            {
                _currentScore++;
                _lastLandedBlock = landedBlock;
                UpdateScoreText();
            }
        }

        private void UpdateScoreText()
        {
            if (_scoreText) _scoreText.text = $"Score: {_currentScore}";
        }
    }
}