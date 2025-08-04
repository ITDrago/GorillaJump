using Core;

namespace Quests.Objectives
{
    public class ApplesCollectedObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnAppleCollected += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnAppleCollected -= UpdateProgress;
    }
}