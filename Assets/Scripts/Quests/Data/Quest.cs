using Quests.Objectives;
using UnityEngine;

namespace Quests.Data
{
    public class Quest
    {
        public string TemplateID { get; }
        public QuestType Type { get; }
        public int CurrentProgress { get; set; }
        public int TargetValue { get; }
        public int Reward { get; }
        public bool IsCompleted => CurrentProgress >= TargetValue;

        public QuestTemplateSO Template { get; }
        public QuestObjective ObjectiveInstance { get; private set; }
        
        public Quest(QuestTemplateSO template, QuestType type, Transform objectiveParent)
        {
            Template = template;
            TemplateID = template.ID;
            Type = type;
            CurrentProgress = 0;
            
            TargetValue = type == QuestType.Daily ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            var rewardRange = type == QuestType.Daily ? template.DailyRewardRange : template.WeeklyRewardRange;
            Reward = Random.Range(rewardRange.x, rewardRange.y + 1);

            InstantiateObjective(objectiveParent);
        }

        public Quest(QuestTemplateSO template, QuestProgressData progressData, Transform objectiveParent)
        {
            Template = template;
            TemplateID = template.ID;
            Type = progressData.Type;
            CurrentProgress = progressData.CurrentProgress;
            Reward = progressData.Reward;

            TargetValue = Type == QuestType.Daily ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            
            InstantiateObjective(objectiveParent);
        }

        private void InstantiateObjective(Transform parent)
        {
            if (!Template || !Template.ObjectivePrefab || !parent) return;

            ObjectiveInstance = Object.Instantiate(Template.ObjectivePrefab, parent);

            ObjectiveInstance.Initialize(
                TargetValue, 
                CurrentProgress, 
                () => QuestManager.Instance.UpdateQuestProgress(this), 
                () => QuestManager.Instance.CompleteQuest(this)
            );
        }
    }
}