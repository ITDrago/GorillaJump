using System;
using Quests.Objectives;
using UnityEngine;

namespace Quests.Data
{
    public enum QuestStatus
    {
        InProgress,
        Completed
    }

    [Serializable]
    public class Quest
    {
        public string TemplateID { get; private set; }
        public QuestStatus Status { get; set; }
        public int CurrentProgress { get; set; }
        public int TargetValue { get; private set; }

        [NonSerialized] public QuestObjective ObjectiveInstance;
        [NonSerialized] public QuestTemplateSO Template;

        public Quest(QuestTemplateSO template, QuestType type)
        {
            Template = template;
            TemplateID = template.ID;
            Status = QuestStatus.InProgress;
            CurrentProgress = 0;
            TargetValue = type == QuestType.Daily ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            InstantiateObjective();
        }

        public Quest(QuestTemplateSO template, QuestProgressData progressData)
        {
            Template = template;
            TemplateID = template.ID;
            Status = progressData.Status;
            CurrentProgress = progressData.CurrentProgress;
            TargetValue = template.BaseDailyTarget > 0 ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            InstantiateObjective();
        }

        public string GetDescription()
        {
            return string.Format(Template.DescriptionFormat, TargetValue);
        }

        private void InstantiateObjective()
        {
            if (QuestManager.Instance == null || Template.ObjectivePrefab == null) return;

            var parent = QuestManager.Instance.transform.Find("Objectives");
            ObjectiveInstance = UnityEngine.Object.Instantiate(Template.ObjectivePrefab, parent);

            Action onProgress = () =>
                QuestManager.Instance.SendMessage("OnQuestProgress", this, SendMessageOptions.DontRequireReceiver);
            Action onComplete = () =>
                QuestManager.Instance.SendMessage("OnQuestCompleted", this, SendMessageOptions.DontRequireReceiver);

            ObjectiveInstance.Initialize(TargetValue, CurrentProgress, onProgress, onComplete);
        }
    }
}