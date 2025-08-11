using System;
using Quests.Objectives;

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
        public int Reward { get; private set; }

        [NonSerialized] public QuestObjective ObjectiveInstance;
        [NonSerialized] public QuestTemplateSO Template;

        public Quest(QuestTemplateSO template, QuestType type)
        {
            Template = template;
            TemplateID = template.ID;
            Status = QuestStatus.InProgress;
            CurrentProgress = 0;
            
            if (type == QuestType.Daily)
            {
                TargetValue = template.BaseDailyTarget;
                Reward = UnityEngine.Random.Range(template.DailyRewardRange.x, template.DailyRewardRange.y + 1);
            }
            else
            {
                TargetValue = template.BaseWeeklyTarget;
                Reward = UnityEngine.Random.Range(template.WeeklyRewardRange.x, template.WeeklyRewardRange.y + 1);
            }
            
            InstantiateObjective();
        }

        public Quest(QuestTemplateSO template, QuestProgressData progressData)
        {
            Template = template;
            TemplateID = template.ID;
            Status = progressData.Status;
            CurrentProgress = progressData.CurrentProgress;
            Reward = progressData.Reward;
            
            TargetValue = template.BaseDailyTarget > 0 ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            InstantiateObjective();
        }

        private void InstantiateObjective()
        {
            if (!QuestManager.Instance || !Template.ObjectivePrefab) return;

            var parent = QuestManager.Instance.transform.Find("Objectives");
            ObjectiveInstance = UnityEngine.Object.Instantiate(Template.ObjectivePrefab, parent);

            Action onProgress = () => QuestManager.Instance.UpdateQuestProgress(this);
            Action onComplete = () => QuestManager.Instance.CompleteQuest(this);

            ObjectiveInstance.Initialize(TargetValue, CurrentProgress, onProgress, onComplete);
        }
    }
}