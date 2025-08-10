using UnityEngine;
using UnityEngine.Localization;
using Quests.Objectives;

namespace Quests.Data
{
    public enum QuestType { Daily, Weekly }

    [CreateAssetMenu(fileName = "NewQuestTemplate", menuName = "Quests/Quest Template")]
    public class QuestTemplateSO : ScriptableObject
    {
        [SerializeField] private string _id;
        public string ID => _id;

        [Header("Localization Keys")]
        [SerializeField] private LocalizedString _descriptionFormat;
        public LocalizedString DescriptionFormat => _descriptionFormat;

        [SerializeField] private LocalizedString _completedStatusText;
        public LocalizedString CompletedStatusText => _completedStatusText;

        [SerializeField] private LocalizedString _progressFormat;
        public LocalizedString ProgressFormat => _progressFormat;


        [Header("Objective")]
        [SerializeField] private QuestObjective _objectivePrefab;
        public QuestObjective ObjectivePrefab => _objectivePrefab;
    
        [Header("Difficulty Settings")]
        [SerializeField] private int _baseDailyTarget = 10;
        public int BaseDailyTarget => _baseDailyTarget;
    
        [SerializeField] private int _baseWeeklyTarget = 100;
        public int BaseWeeklyTarget => _baseWeeklyTarget;

        [Header("Reward Settings")]
        [SerializeField] private Vector2Int _dailyRewardRange = new(50, 100);
        public Vector2Int DailyRewardRange => _dailyRewardRange;
        
        [SerializeField] private Vector2Int _weeklyRewardRange = new(250, 500);
        public Vector2Int WeeklyRewardRange => _weeklyRewardRange;
    }
}