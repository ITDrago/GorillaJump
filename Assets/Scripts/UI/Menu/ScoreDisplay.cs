using Core.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _recordDisplay;
        [SerializeField] private Text _recordDisplayText;

        [Header("Animation Settings")]
        [SerializeField] private float _pulseScale = 1.05f;
        [SerializeField] private float _pulseDuration = 1f;

        private void Start()
        {
            DisplayRecord();
            AnimateRecordDisplay();
        }

        private void OnDestroy()
        {
            if (_recordDisplayText) _recordDisplayText.transform.DOKill();
        }

        private void DisplayRecord()
        {
            var highScore = ScoreSaver.LoadScore();
            _recordDisplayText.text = $"{highScore}!";
        }

        private void AnimateRecordDisplay()
        {
            _recordDisplay.transform
                .DOScale(_pulseScale, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}