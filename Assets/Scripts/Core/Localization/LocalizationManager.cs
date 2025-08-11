using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Core.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }
        public static event Action<int> OnLocaleChanged;
        
        public bool IsInitialized { get; private set; }
        
        private const string LOCALE_PLAYER_PREFS_KEY = "SelectedLocaleIndex";
        private int _currentLocaleIndex;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(InitializeLocale());
        }

        public void SetLocale(int localeIndex)
        {
            if (localeIndex >= LocalizationSettings.AvailableLocales.Locales.Count) return;
            
            _currentLocaleIndex = localeIndex;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_currentLocaleIndex];
            PlayerPrefs.SetInt(LOCALE_PLAYER_PREFS_KEY, _currentLocaleIndex);
            
            OnLocaleChanged?.Invoke(_currentLocaleIndex);
        }

        public int GetCurrentLocaleIndex() => _currentLocaleIndex;

        private IEnumerator InitializeLocale()
        {
            IsInitialized = false;
            yield return LocalizationSettings.InitializationOperation;

            _currentLocaleIndex = PlayerPrefs.GetInt(LOCALE_PLAYER_PREFS_KEY, 0);
            
            if (_currentLocaleIndex < LocalizationSettings.AvailableLocales.Locales.Count)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_currentLocaleIndex];
            }
            
            IsInitialized = true;
        }
    }
}