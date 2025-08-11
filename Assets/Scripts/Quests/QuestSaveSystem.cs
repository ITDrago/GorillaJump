using System.Collections.Generic;
using System.Linq;
using Quests.Data;
using UnityEngine;

namespace Quests
{
    public class QuestSaveSystem : MonoBehaviour
    {
        private const string QUEST_SAVE_KEY = "QuestSaveData";
        private QuestFactory _questFactory;

        public void Initialize(QuestFactory questFactory) => _questFactory = questFactory;

        public void Save(IReadOnlyList<Quest> dailyQuests, IReadOnlyList<Quest> weeklyQuests)
        {
            var saveData = new QuestSaveData
            {
                DailyQuests = dailyQuests.Select(q => new QuestProgressData(q)).ToList(),
                WeeklyQuests = weeklyQuests.Select(q => new QuestProgressData(q)).ToList()
            };

            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(QUEST_SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public (List<Quest> dailyQuests, List<Quest> weeklyQuests) Load()
        {
            var json = PlayerPrefs.GetString(QUEST_SAVE_KEY);
            if (string.IsNullOrEmpty(json))
            {
                return (new List<Quest>(), new List<Quest>());
            }

            var saveData = JsonUtility.FromJson<QuestSaveData>(json);

            var daily = saveData.DailyQuests.Select(_questFactory.CreateFromProgress).ToList();
            var weekly = saveData.WeeklyQuests.Select(_questFactory.CreateFromProgress).ToList();

            return (daily, weekly);
        }
        
        public bool HasSavedQuests() => PlayerPrefs.HasKey(QUEST_SAVE_KEY) && !string.IsNullOrEmpty(PlayerPrefs.GetString(QUEST_SAVE_KEY));

        public void ClearQuestSaveData() => PlayerPrefs.DeleteKey(QUEST_SAVE_KEY);
    }
}