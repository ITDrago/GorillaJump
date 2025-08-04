using Quests.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Quest
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _progressText;
        [SerializeField] private Slider _progressBar;

        public void Setup(Quests.Data.Quest quest)
        {
            _descriptionText.text = quest.GetDescription();

            if (quest.Status == QuestStatus.Completed)
            {
                _progressText.text = "Completed";
            }
            else
            {
                _progressText.text = $"{quest.CurrentProgress}/{quest.TargetValue}";
                _progressBar.gameObject.SetActive(true);
                _progressBar.maxValue = quest.TargetValue;
                _progressBar.value = quest.CurrentProgress;
            }
        }
    }
}