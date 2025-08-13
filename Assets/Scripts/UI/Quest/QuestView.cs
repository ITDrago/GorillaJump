using Quests.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace UI.Quest
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _progressText;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Text _rewardText;
        [SerializeField] private GameObject _rewardPanel;
        
        private LocalizedString _localizedDescription;
        private LocalizedString _localizedStatus;

        private void OnDisable() => CleanUpSubscriptions();

        public void Setup(Quests.Data.Quest quest)
        {
            _rewardText.text = quest.Reward.ToString();
            CleanUpSubscriptions();

            var template = quest.Template;
            SetupDescription(quest, template);

            if (quest.IsCompleted)
            {
                SetupForCompleted(quest, template);
            }
            else
            {
                SetupForInProgress(quest, template);
            }
        }

        private void SetupDescription(Quests.Data.Quest quest, QuestTemplateSO template)
        {
            _localizedDescription = new LocalizedString(template.DescriptionFormat.TableReference, template.DescriptionFormat.TableEntryReference);
            _localizedDescription.Arguments = new object[] { quest.TargetValue };
            _localizedDescription.StringChanged += UpdateDescriptionText;
            _localizedDescription.RefreshString();
        }

        private void SetupForInProgress(Quests.Data.Quest quest, QuestTemplateSO template)
        {
            _localizedStatus = new LocalizedString(template.ProgressFormat.TableReference, template.ProgressFormat.TableEntryReference);
            _localizedStatus.Arguments = new object[] { quest.CurrentProgress, quest.TargetValue };
            _localizedStatus.StringChanged += UpdateProgressText;
            _localizedStatus.RefreshString();
            
            if (_rewardPanel) _rewardPanel.SetActive(true);
            
            _progressBar.gameObject.SetActive(true);
            _progressBar.maxValue = quest.TargetValue;
            _progressBar.value = quest.CurrentProgress;
        }

        private void SetupForCompleted(Quests.Data.Quest quest, QuestTemplateSO template)
        {
            _localizedStatus = new LocalizedString(template.CompletedStatusText.TableReference, template.CompletedStatusText.TableEntryReference);
            _localizedStatus.Arguments = null;
            _localizedStatus.StringChanged += UpdateProgressText;
            _localizedStatus.RefreshString();
        }

        private void CleanUpSubscriptions()
        {
            if (_localizedDescription != null) _localizedDescription.StringChanged -= UpdateDescriptionText;
            if (_localizedStatus != null) _localizedStatus.StringChanged -= UpdateProgressText;
        }
        
        private void UpdateDescriptionText(string value)
        {
            if (_descriptionText) _descriptionText.text = value;
        }

        private void UpdateProgressText(string value)
        {
            if (_progressText) _progressText.text = value;
        }
    }
}