using Core.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text _recordText;

        [Header("Animation Settings")]
        [SerializeField] private float _pulseScale = 1.05f;
        [SerializeField] private float _pulseDuration = 1f;

        private void Start()
        {
            DisplayRecord();
            AnimateRecordText();
        }

        private void OnDestroy()
        {
            _recordText.transform.DOKill();
        }

        private void DisplayRecord()
        {
            var highScore = ScoreSaver.LoadScore();
            _recordText.text = $"Your record is {highScore}!";
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