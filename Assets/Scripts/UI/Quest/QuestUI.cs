using Quests;
using UnityEngine;

namespace UI.Quest
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private Transform _dailyQuestsContainer;
        [SerializeField] private Transform _weeklyQuestsContainer;
        [SerializeField] private GameObject _questUIPrefab;

        public void DisplayQuests()
        {
            foreach (Transform child in _dailyQuestsContainer) Destroy(child.gameObject);
            foreach (Transform child in _weeklyQuestsContainer) Destroy(child.gameObject);

            if (!QuestManager.Instance)
                return;

            foreach (var quest in QuestManager.Instance.DailyQuests) InstantiateQuestView(quest, _dailyQuestsContainer);

            foreach (var quest in QuestManager.Instance.WeeklyQuests) InstantiateQuestView(quest, _weeklyQuestsContainer);
        }

        private void InstantiateQuestView(Quests.Data.Quest quest, Transform parent)
        {
            var questGo = Instantiate(_questUIPrefab, parent);
            var view = questGo.GetComponent<QuestView>();
        
            if (view) view.Setup(quest);
        }
    }
}