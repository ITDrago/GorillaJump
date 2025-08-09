using Core.Game;
using Environment.Blocks.BlockTypes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private ProgressManager _progressManager;

        private Block _lastLandedBlock;

        private void OnEnable()
        {
            if (_progressManager) _progressManager.OnBlockPassed += UpdateScoreText;
        }

        private void OnDisable()
        {
            if (_progressManager) _progressManager.OnBlockPassed -= UpdateScoreText;
        }

        private void Start() => UpdateScoreText();

        private void UpdateScoreText()
        {
            if (_scoreText) _scoreText.text = $"Score: {_progressManager.BlocksPassedCount}";
        }
    }
}