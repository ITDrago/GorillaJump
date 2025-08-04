using System;
using System.Collections.Generic;

namespace Quests.Data
{
    [Serializable]
    public class QuestSaveData
    {
        public List<QuestProgressData> DailyQuests = new();
        public List<QuestProgressData> WeeklyQuests = new();
    }
}