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
        
        private QuestSaveSystem _saveSystem;
        private QuestResetService _resetService;
        private QuestFactory _questFactory;

        public IReadOnlyList<Quest> DailyQuests => _dailyQuests;
        public IReadOnlyList<Quest> WeeklyQuests => _weeklyQuests;
        public DateTime NextDailyResetTime => _resetService.NextDailyResetTime;
        public DateTime NextWeeklyResetTime => _resetService.NextWeeklyResetTime;

        public event Action OnQuestDataUpdated;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _questFactory = gameObject.AddComponent<QuestFactory>();
            _questFactory.Initialize(_allQuestTemplates, _objectivesParent);

            _saveSystem = gameObject.AddComponent<QuestSaveSystem>();
            _saveSystem.Initialize(_questFactory);

            _resetService = gameObject.AddComponent<QuestResetService>();
        }

        private void Start()
        {
            Debug.Log("[QuestManager] Start: Beginning quest load/reset process.");
            LoadOrResetQuests();
        }

        private void LoadOrResetQuests()
        {
            Debug.Log("[QuestManager] LoadOrResetQuests: Checking for resets...");
            var questsWereReset = _resetService.CheckAndPerformResets(GenerateQuests);
            Debug.Log($"[QuestManager] LoadOrResetQuests: Were quests reset by time? -> {questsWereReset}");

            if (!questsWereReset)
            {
                Debug.Log("[QuestManager] LoadOrResetQuests: No reset was needed. Checking for saved data...");
                if (_saveSystem.HasSavedQuests())
                {
                    Debug.Log("[QuestManager] LoadOrResetQuests: Save data found. Loading quests.");
                    (_dailyQuests, _weeklyQuests) = _saveSystem.Load();
                }
                else
                {
                    Debug.Log("[QuestManager] LoadOrResetQuests: No save data found.");
                }
            }

            if (_dailyQuests.Count == 0) GenerateQuests(QuestType.Daily);
            if (_weeklyQuests.Count == 0) GenerateQuests(QuestType.Weekly);

            Debug.Log($"[QuestManager] LoadOrResetQuests: Final quest counts: Daily={_dailyQuests.Count}, Weekly={_weeklyQuests.Count}. Invoking UI update.");
            OnQuestDataUpdated?.Invoke();
        }
        
        private void GenerateQuests(QuestType type)
        {
            Debug.Log($"[QuestManager] GenerateQuests: Generating for type: {type}. Template count: {_allQuestTemplates.Count}");
            var targetList = type == QuestType.Daily ? _dailyQuests : _weeklyQuests;
            var count = type == QuestType.Daily ? _dailyQuestsCount : _weeklyQuestsCount;
            
            targetList.Clear();

            if (_allQuestTemplates.Count == 0) return;
            
            var random = new System.Random();
            var selectedTemplates = _allQuestTemplates.OrderBy(_ => random.Next()).Take(count);

            var questTemplateSos = selectedTemplates.ToList();
            Debug.Log($"[QuestManager] GenerateQuests: Selected {questTemplateSos.Count()} templates to create quests.");

            foreach (var template in questTemplateSos)
            {
                Debug.Log($"[QuestManager] GenerateQuests: Creating quest from template '{template.ID}'.");
                targetList.Add(_questFactory.CreateFromTemplate(template, type));
            }

            Debug.Log($"[QuestManager] GenerateQuests: Generation for type {type} complete. Saving quests...");
            SaveQuests();
        }
        
        public void UpdateQuestProgress(Quest quest)
        {
            if (quest.IsCompleted) return;
            quest.CurrentProgress++;
            SaveQuests();
            OnQuestDataUpdated?.Invoke();
        }

        public void CompleteQuest(Quest quest)
        {
            quest.CurrentProgress = quest.TargetValue;
            SaveQuests();
            OnQuestDataUpdated?.Invoke();
        }

        private void SaveQuests() => _saveSystem.Save(_dailyQuests, _weeklyQuests);
        
        [ContextMenu("Clear All Quest PlayerPrefs")]
        public void ClearQuestPlayerPrefs()
        {
            _saveSystem.ClearQuestSaveData();
            _resetService.ClearResetTimeData();
            PlayerPrefs.Save();
            Debug.Log("Quest data in PlayerPrefs cleared!");
        }
    }
}