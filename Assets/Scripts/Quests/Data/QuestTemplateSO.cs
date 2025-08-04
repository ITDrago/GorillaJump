using Quests.Objectives;
using UnityEngine;

namespace Quests.Data
{
    public enum QuestType { Daily, Weekly }

    [CreateAssetMenu(fileName = "NewQuestTemplate", menuName = "Quests/Quest Template")]
    public class QuestTemplateSO : ScriptableObject
    {
        [SerializeField] private string _id;
        public string ID => _id;

        [SerializeField, TextArea] 
        private string _descriptionFormat;
        public string DescriptionFormat => _descriptionFormat;

        [SerializeField] private QuestObjective _objectivePrefab;
        public QuestObjective ObjectivePrefab => _objectivePrefab;
    
        [Header("Difficulty Settings")]
        [SerializeField] private int _baseDailyTarget = 10;
        public int BaseDailyTarget => _baseDailyTarget;
    
        [SerializeField] private int _baseWeeklyTarget = 100;
        public int BaseWeeklyTarget => _baseWeeklyTarget;
    }
}