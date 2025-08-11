using Core.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace UI.Menu
{
    public sealed class ScoreDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text _recordText;
        [SerializeField] private LocalizedString _recordFormat;

        [Header("Animation Settings")]
        [SerializeField] private float _pulseScale = 1;
        [SerializeField] private float _pulseDuration = 0.5f;

        private void OnEnable() => _recordFormat.StringChanged += OnLocalizedTextChanged;

        private void OnDisable() => _recordFormat.StringChanged -= OnLocalizedTextChanged;

        private void Start()
        {
            DisplayRecord();
            AnimateRecordText();
        }

        private void OnDestroy()
        {
            if (_recordText) _recordText.transform.DOKill();
        }

        private void DisplayRecord()
        {
            var highScore = ScoreSaver.LoadScore();
            _recordFormat.Arguments = new object[] { highScore };
            _recordFormat.RefreshString();
        }

        private void OnLocalizedTextChanged(string value)
        {
            if (_recordText) _recordText.text = value;
        }

        private void AnimateRecordText()
        {
            _recordText.transform
                .DOScale(_pulseScale, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}