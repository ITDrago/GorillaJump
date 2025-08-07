using Core.Game;
using Core.Money;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Banana
{
    public class FallingBanana : MovingObject
    {
        [SerializeField] private int _rewardAmount = 5;
        
        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            GameEvents.BananaCollected();
            MoneySystem.Instance.AddMoney(_rewardAmount);
            Destroy(gameObject);
        }
    }
}