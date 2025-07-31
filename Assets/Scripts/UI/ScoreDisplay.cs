using Core;
using UnityEngine;
using UnityEngine.UI;
using Environment.Blocks.BlockTypes;

namespace UI
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