using Core.Localization;
using UnityEngine;

namespace UI.Menu.Localization
{
    public class LanguageSwitcher : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup _russianButtonCanvasGroup;
        [SerializeField] private CanvasGroup _englishButtonCanvasGroup;

        [Header("Visual Settings")]
        [SerializeField] private float _selectedAlpha = 1;
        [SerializeField] private float _deselectedAlpha = 0.5f;

        private void OnEnable()
        {
            LocalizationManager.OnLocaleChanged += UpdateButtonsVisuals;
            if (LocalizationManager.Instance)
            {
                UpdateButtonsVisuals(LocalizationManager.Instance.GetCurrentLocaleIndex());
            }
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocaleChanged -= UpdateButtonsVisuals;
        }

        public void SetRussian() => LocalizationManager.Instance.SetLocale(1);

        public void SetEnglish() => LocalizationManager.Instance.SetLocale(0);

        private void UpdateButtonsVisuals(int selectedLocaleIndex)
        {
            if (_russianButtonCanvasGroup)
            {
                _russianButtonCanvasGroup.alpha = selectedLocaleIndex == 0 ? _selectedAlpha : _deselectedAlpha;
            }

            if (_englishButtonCanvasGroup)
            {
                _englishButtonCanvasGroup.alpha = selectedLocaleIndex == 1 ? _selectedAlpha : _deselectedAlpha;
            }
        }
    }
}