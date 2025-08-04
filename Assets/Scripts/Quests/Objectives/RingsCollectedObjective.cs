using Core;

namespace Quests.Objectives
{
    public class RingsCollectedObjective : QuestObjective
    {
        private void HandleRingCollected(int reward) => UpdateProgress();

        protected override void SubscribeToEvents() => GameEvents.OnRingCollected += HandleRingCollected;

        protected override void UnsubscribeFromEvents() => GameEvents.OnRingCollected -= HandleRingCollected;
    }
}