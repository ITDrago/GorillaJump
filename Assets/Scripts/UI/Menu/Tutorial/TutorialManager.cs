using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.4f;
        [SerializeField] private Ease _fadeEase = Ease.OutCubic;

        [Header("UI Components")]
        [SerializeField] private CanvasGroup _tutorialPanelCanvasGroup;
        [SerializeField] private CanvasGroup _mainMenuCanvasGroup;
        [SerializeField] private GameObject[] _chapterPages;

        private const string TUTORIAL_COMPLETED_KEY = "TutorialWasShown";
        private int _currentPageIndex;

        private void Start() => InitializePanel();

        public void ShowTutorialPanel(bool isFirstLaunch = false)
        {
            AnimatePanel(true, isFirstLaunch);
            UpdatePageView();
        }

        public void CloseTutorialPanel()
        {
            AnimatePanel(false, false);
            PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        }

        public void ShowNextPage()
        {
            _currentPageIndex = (_currentPageIndex + 1) % _chapterPages.Length;
            UpdatePageView();
        }

        public void ShowPreviousPage()
        {
            _currentPageIndex--;
            if (_currentPageIndex < 0)
            {
                _currentPageIndex = _chapterPages.Length - 1;
            }
            UpdatePageView();
        }
        
        private void InitializePanel()
        {
            if (!_tutorialPanelCanvasGroup) return;
            
            _tutorialPanelCanvasGroup.alpha = 0;
            _tutorialPanelCanvasGroup.interactable = false;
            _tutorialPanelCanvasGroup.blocksRaycasts = false;
            _tutorialPanelCanvasGroup.gameObject.SetActive(false);
            
            UpdatePageView();
        }

        private void UpdatePageView()
        {
            for (var i = 0; i < _chapterPages.Length; i++)
            {
                if (_chapterPages[i])
                {
                    _chapterPages[i].SetActive(i == _currentPageIndex);
                }
            }
        }

        private void AnimatePanel(bool show, bool isFirstLaunch)
        {
            if (show)
            {
                _tutorialPanelCanvasGroup.gameObject.SetActive(true);
                _tutorialPanelCanvasGroup.DOFade(1, _fadeDuration).SetEase(_fadeEase);
                _tutorialPanelCanvasGroup.interactable = true;
                _tutorialPanelCanvasGroup.blocksRaycasts = true;

                if (_mainMenuCanvasGroup)
                {
                    _mainMenuCanvasGroup.interactable = false;
                    _mainMenuCanvasGroup.DOFade(0, _fadeDuration);
                }
            }
            else
            {
                _tutorialPanelCanvasGroup.DOFade(0, _fadeDuration)
                    .SetEase(_fadeEase)
                    .OnComplete(() =>
                    {
                        _tutorialPanelCanvasGroup.interactable = false;
                        _tutorialPanelCanvasGroup.blocksRaycasts = false;
                        _tutorialPanelCanvasGroup.gameObject.SetActive(false);
                    });

                if (_mainMenuCanvasGroup)
                {
                    _mainMenuCanvasGroup.interactable = true;
                    _mainMenuCanvasGroup.DOFade(1, _fadeDuration);
                }
            }
        }
    }
}