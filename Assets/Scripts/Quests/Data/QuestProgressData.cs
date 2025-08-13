using System;

namespace Quests.Data
{
    [Serializable]
    public class QuestProgressData
    {
        public string TemplateID;
        public QuestType Type;
        public int CurrentProgress;
        public int Reward;

        public QuestProgressData() {}
        
        public QuestProgressData(Quest quest)
        {
            TemplateID = quest.TemplateID;
            Type = quest.Type;
            CurrentProgress = quest.CurrentProgress;
            Reward = quest.Reward;
        }
    }
}