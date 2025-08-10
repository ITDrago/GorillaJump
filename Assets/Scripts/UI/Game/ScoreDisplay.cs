using Core.Game;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace UI.Game
{
    public sealed class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private ProgressManager _progressManager;
        [SerializeField] private LocalizedString _scoreFormat;

        private void OnEnable()
        {
            if (_progressManager) _progressManager.OnBlockPassed += UpdateScore;
            _scoreFormat.StringChanged += OnLocalizedTextChanged;
        }

        private void OnDisable()
        {
            if (_progressManager) _progressManager.OnBlockPassed -= UpdateScore;
            _scoreFormat.StringChanged -= OnLocalizedTextChanged;
        }

        private void Start()
        {
            var initialScore = _progressManager ? _progressManager.BlocksPassedCount : 0;
            UpdateScore(initialScore);
        }

        private void UpdateScore() => UpdateScore(_progressManager ? _progressManager.BlocksPassedCount : 0);

        private void UpdateScore(int score)
        {
            _scoreFormat.Arguments = new object[] { score };
            _scoreFormat.RefreshString();
        }

        private void OnLocalizedTextChanged(string value)
        {
            if (_scoreText) _scoreText.text = value;
        }
    }
}