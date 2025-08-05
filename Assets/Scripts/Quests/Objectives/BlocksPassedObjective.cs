using Core;
using Core.Game;

namespace Quests.Objectives
{
    public class BlocksPassedObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnBlockPassed += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnBlockPassed -= UpdateProgress;
    }
}