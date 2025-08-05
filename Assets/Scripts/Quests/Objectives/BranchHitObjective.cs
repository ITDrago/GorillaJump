using Core;
using Core.Game;

namespace Quests.Objectives
{
    public class BranchHitObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnHitByBranch += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnHitByBranch -= UpdateProgress;
    }
}