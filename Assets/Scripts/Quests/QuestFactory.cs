using System.Collections.Generic;
using System.Linq;
using Quests.Data;
using UnityEngine;

namespace Quests
{
    public class QuestFactory : MonoBehaviour
    {
        private IReadOnlyList<QuestTemplateSO> _allQuestTemplates;
        private Transform _objectivesParent;

        public void Initialize(IReadOnlyList<QuestTemplateSO> allQuestTemplates, Transform objectivesParent)
        {
            _allQuestTemplates = allQuestTemplates;
            _objectivesParent = objectivesParent;
        }

        public Quest CreateFromTemplate(QuestTemplateSO template, QuestType type)
        {
            if (!template) return null;
            return new Quest(template, type, _objectivesParent);
        }

        public Quest CreateFromProgress(QuestProgressData progressData)
        {
            var template = _allQuestTemplates.FirstOrDefault(t => t.ID == progressData.TemplateID);
            if (!template) return null;
            return new Quest(template, progressData, _objectivesParent);
        }
    }
}