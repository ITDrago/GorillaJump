using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Branch
{
    public class FallingBranch : MovingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player was hit by a falling branch!");
            Destroy(gameObject);
        }
    }
}