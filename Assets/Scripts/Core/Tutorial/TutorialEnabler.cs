using UI.Menu.Tutorial;
using UnityEngine;

namespace Core.Tutorial
{
    public class TutorialEnabler : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool _disableForDevelopment = false;
        
        [Header("Dependencies")]
        [SerializeField] private TutorialManager _tutorialManager;

        private const string TUTORIAL_COMPLETED_KEY = "TutorialWasShown";

        private void Start()
        {
            if (_disableForDevelopment) return;
            if (!_tutorialManager) return;
            
            var tutorialWasShown = PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 1;

            if (!tutorialWasShown) _tutorialManager.ShowTutorialPanel(true);
        }
    }
}