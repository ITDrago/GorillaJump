using DG.Tweening;
using Shop;
using UI.Quest;
using UnityEngine;

namespace UI.Menu
{
    public class MainMenuAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _animationEase = Ease.OutQuint;

        [Header("Core UI")]
        [SerializeField] private RectTransform _mainPanel;
        [SerializeField] private CanvasGroup _dimmingPanelCanvasGroup;

        [Header("Panels")]
        [SerializeField] private CanvasGroup _missionsPanelCanvasGroup;
        [SerializeField] private CanvasGroup _shopPanelCanvasGroup;
        [SerializeField] private CanvasGroup _settingsPanelCanvasGroup;
        
        [Header("Dependencies")]
        [SerializeField] private QuestUI _questUI;
        [SerializeField] private MenuCharacterDisplay _menuCharacterDisplay;
        [SerializeField] private ShopManager _shopManager;

        private Vector2 _mainPanelOnscreenPosition;
        private bool _isPanelOpen;
        private bool _isAnimating;

        private void Start()
        {
            _mainPanelOnscreenPosition = _mainPanel.anchoredPosition;
            InitializePanelState(_missionsPanelCanvasGroup);
            InitializePanelState(_shopPanelCanvasGroup);
            InitializePanelState(_settingsPanelCanvasGroup);
        }

        public void OpenMissionsPanel()
        {
            if (_questUI) _questUI.DisplayQuests();
            AnimatePanelOpen(_missionsPanelCanvasGroup);
        }

        public void CloseMissionsPanel() => AnimatePanelClose(_missionsPanelCanvasGroup);

        public void OpenShopPanel()
        {
            _shopManager?.Initialize();
            AnimatePanelOpen(_shopPanelCanvasGroup);
        }

        public void CloseShopPanel()
        {
            _shopManager?.Cleanup();
            AnimatePanelClose(_shopPanelCanvasGroup);
        }

        public void OpenSettingsPanel() => AnimatePanelOpen(_settingsPanelCanvasGroup);

        public void CloseSettingsPanel() => AnimatePanelClose(_settingsPanelCanvasGroup);

        private void InitializePanelState(CanvasGroup panel)
        {
            if (!panel) return;
            panel.gameObject.SetActive(false);
        }

        private void AnimatePanelOpen(CanvasGroup panel)
        {
            if (_isPanelOpen || _isAnimating || !panel) return;
            
            _isPanelOpen = true;
            _isAnimating = true;

            _menuCharacterDisplay?.SetCharacterVisible(false);
            
            panel.gameObject.SetActive(true);
            panel.alpha = 0;
            panel.interactable = true;
            panel.blocksRaycasts = true;

            var sequence = DOTween.Sequence();
            sequence.Append(_dimmingPanelCanvasGroup.DOFade(1, _animationDuration))
                .Join(_mainPanel.DOAnchorPosX(-_mainPanel.rect.width, _animationDuration).SetEase(_animationEase))
                .Join(panel.DOFade(1, _animationDuration))
                .OnComplete(() => 
                {
                    _isAnimating = false;
                });
        }

        private void AnimatePanelClose(CanvasGroup panel)
        {
            if (!_isPanelOpen || _isAnimating || !panel) return;
            
            _isPanelOpen = false;
            _isAnimating = true;

            _menuCharacterDisplay?.SetCharacterVisible(true);
            
            panel.interactable = false;
            panel.blocksRaycasts = false;
        
            var sequence = DOTween.Sequence();
            sequence.Append(_dimmingPanelCanvasGroup.DOFade(0, _animationDuration))
                .Join(_mainPanel.DOAnchorPosX(_mainPanelOnscreenPosition.x, _animationDuration).SetEase(_animationEase))
                .Join(panel.DOFade(0, _animationDuration))
                .OnComplete(() => 
                {
                    panel.gameObject.SetActive(false);
                    _isAnimating = false;
                });
        }
    }
}