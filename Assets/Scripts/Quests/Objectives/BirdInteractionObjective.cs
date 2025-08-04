using Core;

namespace Quests.Objectives
{
    public class BirdInteractionObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnBirdInteraction += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnBirdInteraction -= UpdateProgress;
    }
}