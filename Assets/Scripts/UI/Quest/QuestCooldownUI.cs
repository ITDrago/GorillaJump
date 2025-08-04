using System;
using Quests;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Quest
{
    public class QuestCooldownUI : MonoBehaviour
    {
        [SerializeField] private Text _dailyCooldownText;
        [SerializeField] private Text _weeklyCooldownText;

        private void OnEnable()
        {
            if (QuestManager.Instance)
            {
                QuestManager.Instance.OnQuestDataUpdated += UpdateCooldowns;
                UpdateCooldowns();
            }
        }

        private void OnDisable()
        {
            if (QuestManager.Instance) QuestManager.Instance.OnQuestDataUpdated -= UpdateCooldowns;
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        private void UpdateCooldowns()
        {
            if (!QuestManager.Instance) return;

            var dailyTimeLeft = QuestManager.Instance.NextDailyResetTime - DateTime.UtcNow;
            var weeklyTimeLeft = QuestManager.Instance.NextWeeklyResetTime - DateTime.UtcNow;

            _dailyCooldownText.text = FormatTimeSpan(dailyTimeLeft);
            _weeklyCooldownText.text = FormatTimeSpan(weeklyTimeLeft);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.Days > 0) return $"{ts.Days}d. {ts.Hours:D2}h. {ts.Minutes:D2}min.";

            return $"{ts.Hours:D2}h. {ts.Minutes:D2}min.";
        }
    }
}