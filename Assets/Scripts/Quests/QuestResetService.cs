using System;
using Quests.Data;
using UnityEngine;

namespace Quests
{
    public class QuestResetService : MonoBehaviour
    {
        private const string LAST_DAILY_RESET_KEY = "LastDailyResetTime";
        private const string LAST_WEEKLY_RESET_KEY = "LastWeeklyResetTime";

        public DateTime NextDailyResetTime { get; private set; }
        public DateTime NextWeeklyResetTime { get; private set; }

        public bool CheckAndPerformResets(Action<QuestType> generateQuestsCallback)
        {
            var didDailyReset = CheckDailyReset();
            var didWeeklyReset = CheckWeeklyReset();

            if (didDailyReset)
            {
                generateQuestsCallback(QuestType.Daily);
                SaveTime(LAST_DAILY_RESET_KEY, DateTime.UtcNow);
            }

            if (didWeeklyReset)
            {
                generateQuestsCallback(QuestType.Weekly);
                SaveTime(LAST_WEEKLY_RESET_KEY, DateTime.UtcNow);
            }
            
            UpdateResetTimers();
            return didDailyReset || didWeeklyReset;
        }

        private bool CheckDailyReset()
        {
            var lastReset = GetSavedTime(LAST_DAILY_RESET_KEY);
            return lastReset.Date < DateTime.UtcNow.Date;
        }

        private bool CheckWeeklyReset()
        {
            var lastReset = GetSavedTime(LAST_WEEKLY_RESET_KEY);
            var today = DateTime.UtcNow.Date;
            var daysSinceMonday = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
            var startOfThisWeek = today.AddDays(-daysSinceMonday);
            
            return lastReset.Date < startOfThisWeek;
        }
        
        private void UpdateResetTimers()
        {
            var today = DateTime.UtcNow.Date;
            NextDailyResetTime = today.AddDays(1);
            
            var daysSinceMonday = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
            var startOfThisWeek = today.AddDays(-daysSinceMonday);
            NextWeeklyResetTime = startOfThisWeek.AddDays(7);
        }

        private void SaveTime(string key, DateTime time) => PlayerPrefs.SetString(key, time.ToString("o"));

        private DateTime GetSavedTime(string key)
        {
            var savedTime = PlayerPrefs.GetString(key, string.Empty);
            return string.IsNullOrEmpty(savedTime) ? DateTime.MinValue : DateTime.Parse(savedTime);
        }

        public void ClearResetTimeData()
        {
            PlayerPrefs.DeleteKey(LAST_DAILY_RESET_KEY);
            PlayerPrefs.DeleteKey(LAST_WEEKLY_RESET_KEY);
        }
    }
}