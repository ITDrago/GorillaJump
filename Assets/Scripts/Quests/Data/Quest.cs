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
        public QuestType Type { get; private set; }
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
            Type = type;
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
            
            var logMessage = $"[Quest Generation] Создан квест '{Template.ID}'" +
                             $"\nТип: {Type}" +
                             $"\nЦели в шаблоне (Daily: {template.BaseDailyTarget}, Weekly: {template.BaseWeeklyTarget})" +
                             $"\nИтоговая цель: {TargetValue}";

            if ((Type == QuestType.Daily && TargetValue != template.BaseDailyTarget) ||
                (Type == QuestType.Weekly && TargetValue != template.BaseWeeklyTarget))
            {
                Debug.LogError(logMessage + "\nНесоответствие");
            }
            else
            {
                Debug.Log(logMessage);
            }
    
            // --- КОНЕЦ ДИАГНОСТИЧЕСКОГО КОДА ---

            InstantiateObjective();
        }

        public Quest(QuestTemplateSO template, QuestProgressData progressData)
        {
            Template = template;
            TemplateID = template.ID;
            Type = progressData.Type;
            Status = progressData.Status;
            CurrentProgress = progressData.CurrentProgress;
            Reward = progressData.Reward;

            TargetValue = Type == QuestType.Daily ? template.BaseDailyTarget : template.BaseWeeklyTarget;
            
            InstantiateObjective();
        }

        private void InstantiateObjective()
        {
            if (!QuestManager.Instance || !Template.ObjectivePrefab) return;

            var parent = QuestManager.Instance.transform.Find("Objectives");
            if (!parent) return;
            
            ObjectiveInstance = UnityEngine.Object.Instantiate(Template.ObjectivePrefab, parent);

            Action onProgress = () => QuestManager.Instance.UpdateQuestProgress(this);
            Action onComplete = () => QuestManager.Instance.CompleteQuest(this);

            ObjectiveInstance.Initialize(TargetValue, CurrentProgress, onProgress, onComplete);
        }
    }
}