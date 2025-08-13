using Core.Game;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Branch
{
    public class FallingBranch : MovingObject
    {
        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            if (playerCore.PlayerShieldController && playerCore.PlayerShieldController.IsShieldActive)
            {
                playerCore.PlayerShieldController.Deactivate();
                Destroy(gameObject);
                return;
            }
            
            Debug.Log("Player was hit by falling branch!");
            GameEvents.HitByBranch();
            playerCore.PlayerHealth.Die();
        }
    }
}