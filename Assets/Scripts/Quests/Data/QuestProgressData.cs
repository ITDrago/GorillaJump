using System;

namespace Quests.Data
{
    [Serializable]
    public class QuestProgressData
    {
        public string TemplateID;
        public QuestType Type;
        public int CurrentProgress;
        public QuestStatus Status;
        public int Reward;
    }
}