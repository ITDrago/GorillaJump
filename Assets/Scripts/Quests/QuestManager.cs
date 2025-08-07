using System;
using System.Collections.Generic;
using System.Linq;
using Quests.Data;
using UnityEngine;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [SerializeField] private List<QuestTemplateSO> _allQuestTemplates;
        [SerializeField] private int _dailyQuestsCount = 3;
        [SerializeField] private int _weeklyQuestsCount = 2;
        [SerializeField] private Transform _objectivesParent;

        private List<Quest> _dailyQuests = new();
        private List<Quest> _weeklyQuests = new();

        public IReadOnlyList<Quest> DailyQuests => _dailyQuests;
        public IReadOnlyList<Quest> WeeklyQuests => _weeklyQuests;

        public DateTime NextDailyResetTime { get; private set; }
        public DateTime NextWeeklyResetTime { get; private set; }

        public event Action OnQuestDataUpdated;

        private const string QUEST_SAVE_KEY = "QuestSaveData";
        private const string LAST_DAILY_RESET_KEY = "LastDailyResetTime";
        private const string LAST_WEEKLY_RESET_KEY = "LastWeeklyResetTime";

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadOrResetQuests();
        }

        public void UpdateQuestProgress(Quest quest)
        {
            quest.CurrentProgress++;
            SaveQuests();
            OnQuestDataUpdated?.Invoke();
        }

        public void CompleteQuest(Quest quest)
        {
            quest.Status = QuestStatus.Completed;
            SaveQuests();
            OnQuestDataUpdated?.Invoke();
        }

        private void LoadOrResetQuests()
        {
            var questsWereReset = CheckForResets();

            if (!questsWereReset)
            {
                LoadQuests();
            }

            UpdateResetTimers();
            OnQuestDataUpdated?.Invoke();
        }

        private bool CheckForResets()
        {
            var lastDailyReset = GetSavedTime(LAST_DAILY_RESET_KEY);
            var lastWeeklyReset = GetSavedTime(LAST_WEEKLY_RESET_KEY);

            var didDailyReset = false;
            if (lastDailyReset.Date < DateTime.UtcNow.Date)
            {
                GenerateQuests(QuestType.Daily);
                SaveTime(LAST_DAILY_RESET_KEY, DateTime.UtcNow);
                didDailyReset = true;
            }

            var today = DateTime.UtcNow.Date;
            var daysSinceMonday = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
            var startOfThisWeek = today.AddDays(-daysSinceMonday);

            var didWeeklyReset = false;
            if (lastWeeklyReset.Date < startOfThisWeek)
            {
                GenerateQuests(QuestType.Weekly);
                SaveTime(LAST_WEEKLY_RESET_KEY, DateTime.UtcNow);
                didWeeklyReset = true;
            }

            return didDailyReset || didWeeklyReset;
        }

        private void UpdateResetTimers()
        {
            NextDailyResetTime = DateTime.UtcNow.Date.AddDays(1);

            var today = DateTime.UtcNow.Date;
            var daysSinceMonday = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
            var startOfThisWeek = today.AddDays(-daysSinceMonday);
            NextWeeklyResetTime = startOfThisWeek.AddDays(7);
        }

        private void GenerateQuests(QuestType type)
        {
            var questList = type == QuestType.Daily ? _dailyQuests : _weeklyQuests;
            questList.Clear();

            var count = type == QuestType.Daily ? _dailyQuestsCount : _weeklyQuestsCount;
            var availableTemplates = new List<QuestTemplateSO>(_allQuestTemplates);

            for (var i = 0; i < count; i++)
            {
                if (availableTemplates.Count == 0) break;
                var template = availableTemplates[UnityEngine.Random.Range(0, availableTemplates.Count)];
                availableTemplates.Remove(template);
                questList.Add(new Quest(template, type));
            }

            SaveQuests();
        }

        private void SaveQuests()
        {
            var saveData = new QuestSaveData
            {
                DailyQuests = _dailyQuests.Select(q => new QuestProgressData
                    { TemplateID = q.TemplateID, CurrentProgress = q.CurrentProgress, Status = q.Status, Reward = q.Reward }).ToList(),
                WeeklyQuests = _weeklyQuests.Select(q => new QuestProgressData
                    { TemplateID = q.TemplateID, CurrentProgress = q.CurrentProgress, Status = q.Status, Reward = q.Reward }).ToList()
            };

            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(QUEST_SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadQuests()
        {
            if (!PlayerPrefs.HasKey(QUEST_SAVE_KEY)) return;

            var json = PlayerPrefs.GetString(QUEST_SAVE_KEY);
            var saveData = JsonUtility.FromJson<QuestSaveData>(json);

            _dailyQuests = LoadQuestList(saveData.DailyQuests);
            _weeklyQuests = LoadQuestList(saveData.WeeklyQuests);
        }

        private List<Quest> LoadQuestList(List<QuestProgressData> progressData)
        {
            var list = new List<Quest>();
            foreach (var data in progressData)
            {
                var template = _allQuestTemplates.FirstOrDefault(t => t.ID == data.TemplateID);
                if (template) list.Add(new Quest(template, data));
            }

            return list;
        }

        private void SaveTime(string key, DateTime time)
        {
            PlayerPrefs.SetString(key, time.ToString("o"));
        }

        private DateTime GetSavedTime(string key)
        {
            var savedTime = PlayerPrefs.GetString(key, null);
            return string.IsNullOrEmpty(savedTime) ? DateTime.MinValue : DateTime.Parse(savedTime);
        }

        [ContextMenu("Force Regenerate Quests Now")]
        public void ForceRegenerateQuests()
        {
            GenerateQuests(QuestType.Daily);
            GenerateQuests(QuestType.Weekly);
            SaveTime(LAST_DAILY_RESET_KEY, DateTime.UtcNow);
            SaveTime(LAST_WEEKLY_RESET_KEY, DateTime.UtcNow);

            UpdateResetTimers();
            OnQuestDataUpdated?.Invoke();
        }
    }
}