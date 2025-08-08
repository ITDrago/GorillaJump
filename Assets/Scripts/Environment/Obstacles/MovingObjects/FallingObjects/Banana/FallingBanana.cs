using Core.Game;
using Money;
using Player;
using Skins;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Banana
{
    public class FallingBanana : MovingObject
    {
        [SerializeField] private int _rewardAmount = 5;
        
        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            GameEvents.BananaCollected();
            
            var rewardMultiplier = ActiveSkinManager.Instance.CurrentSkin?.RewardMultiplier ?? 1;
            var finalAmount = Mathf.RoundToInt(_rewardAmount * rewardMultiplier);
            
            MoneySystem.Instance.AddMoney(finalAmount);
            Destroy(gameObject);
        }
    }
}