using System;

namespace Core.Game
{
    public static class GameEvents
    {
        public static event Action OnBlockPassed;
        public static void BlockPassed() => OnBlockPassed?.Invoke();

        public static event Action OnBananaCollected;
        public static void BananaCollected() => OnBananaCollected?.Invoke();

        public static event Action OnPlayerJumped;
        public static void PlayerJumped() => OnPlayerJumped?.Invoke();
    
        public static event Action OnAppleCollected;
        public static void AppleCollected() => OnAppleCollected?.Invoke();

        public static event Action OnHitByBranch;
        public static void HitByBranch() => OnHitByBranch?.Invoke();

        public static event Action<int> OnRingCollected;
        public static void RingCollected(int reward) => OnRingCollected?.Invoke(reward);

        public static event Action OnBirdInteraction;
        public static void BirdInteraction() => OnBirdInteraction?.Invoke();
    }
}