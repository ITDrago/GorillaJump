using DG.Tweening;
using UI.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MainMenuAnimator : MonoBehaviour
    {
        [SerializeField] private Button _missionsButton;
        [SerializeField] private Button _closeMissionsButton;
    
        [SerializeField] private RectTransform _mainPanel;
        [SerializeField] private CanvasGroup _missionsPanelCanvasGroup;
        [SerializeField] private CanvasGroup _dimmingPanelCanvasGroup;

        [SerializeField] private QuestUI _questUI;

        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _animationEase = Ease.OutQuint;

        private Vector2 _mainPanelOnscreenPosition;

        private void Start()
        {
            _missionsPanelCanvasGroup.gameObject.SetActive(true);
            _mainPanelOnscreenPosition = _mainPanel.anchoredPosition;
            _missionsButton.onClick.AddListener(ShowMissions);
            _closeMissionsButton.onClick.AddListener(HideMissions);

            _missionsPanelCanvasGroup.alpha = 0;
            _missionsPanelCanvasGroup.interactable = false;
            _missionsPanelCanvasGroup.blocksRaycasts = false;
        }

        private void ShowMissions()
        {
            _questUI.DisplayQuests();

            _missionsPanelCanvasGroup.interactable = true;
            _missionsPanelCanvasGroup.blocksRaycasts = true;

            var sequence = DOTween.Sequence();
            sequence.Append(_dimmingPanelCanvasGroup.DOFade(1f, _animationDuration))
                .Join(_mainPanel.DOAnchorPosX(-_mainPanel.rect.width, _animationDuration).SetEase(_animationEase))
                .Join(_missionsPanelCanvasGroup.DOFade(1f, _animationDuration));
        }

        private void HideMissions()
        {
            _missionsPanelCanvasGroup.interactable = false;
            _missionsPanelCanvasGroup.blocksRaycasts = false;
        
            var sequence = DOTween.Sequence();
            sequence.Append(_dimmingPanelCanvasGroup.DOFade(0f, _animationDuration))
                .Join(_mainPanel.DOAnchorPosX(_mainPanelOnscreenPosition.x, _animationDuration).SetEase(_animationEase))
                .Join(_missionsPanelCanvasGroup.DOFade(0f, _animationDuration));
        }
    }
}