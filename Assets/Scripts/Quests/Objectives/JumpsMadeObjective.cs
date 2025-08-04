using Core;

namespace Quests.Objectives
{
    public class JumpsMadeObjective : QuestObjective
    {
        protected override void SubscribeToEvents() => GameEvents.OnPlayerJumped += UpdateProgress;

        protected override void UnsubscribeFromEvents() => GameEvents.OnPlayerJumped -= UpdateProgress;
    }
}