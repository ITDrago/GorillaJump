using System;

namespace Quests.Data
{
    [Serializable]
    public class QuestProgressData
    {
        public string TemplateID;
        public int CurrentProgress;
        public QuestStatus Status;
    }
}