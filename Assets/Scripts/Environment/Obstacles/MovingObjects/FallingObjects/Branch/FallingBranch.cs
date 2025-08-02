using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Branch
{
    public class FallingBranch : MovingObject
    {
        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            Debug.Log("Player was hit by falling branch!");
            playerCore.PlayerHealth.Die();
        }
    }
}