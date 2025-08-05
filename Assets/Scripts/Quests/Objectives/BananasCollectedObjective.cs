using Core;
using Core.Game;

namespace Quests.Objectives
{
    public class BananasCollectedObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnBananaCollected += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnBananaCollected -= UpdateProgress;
    }
}