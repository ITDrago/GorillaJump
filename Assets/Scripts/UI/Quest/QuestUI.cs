using Quests;
using UnityEngine;

namespace UI.Quest
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private Transform _dailyQuestsContainer;
        [SerializeField] private Transform _weeklyQuestsContainer;
        [SerializeField] private GameObject _questUIPrefab;

        private void OnEnable()
        {
            if (QuestManager.Instance)
            {
                QuestManager.Instance.OnQuestDataUpdated += DisplayQuests;
                DisplayQuests();
            }
        }

        private void OnDisable()
        {
            if (QuestManager.Instance)
            {
                QuestManager.Instance.OnQuestDataUpdated -= DisplayQuests;
            }
        }

        public void DisplayQuests()
        {
            ClearContainer(_dailyQuestsContainer);
            ClearContainer(_weeklyQuestsContainer);

            if (!QuestManager.Instance) return;

            foreach (var quest in QuestManager.Instance.DailyQuests) InstantiateQuestView(quest, _dailyQuestsContainer);

            foreach (var quest in QuestManager.Instance.WeeklyQuests) InstantiateQuestView(quest, _weeklyQuestsContainer);
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container) Destroy(child.gameObject);
        }

        private void InstantiateQuestView(Quests.Data.Quest quest, Transform parent)
        {
            var questGo = Instantiate(_questUIPrefab, parent);
            var view = questGo.GetComponent<QuestView>();
            if (view)
            {
                view.Setup(quest);
            }
        }
    }
}