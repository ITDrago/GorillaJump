using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Branch
{
    public class FallingBranch : MovingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            if (player.TryGetComponent(out PlayerHealth playerHealth))
            {
                Debug.Log("Player was hit by falling branch!");
                playerHealth.Die();
            }
        }
    }
}