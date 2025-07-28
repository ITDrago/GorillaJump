using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Branch
{
    public class MovingBranch : MovingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player was hit by a falling branch!");
            Destroy(gameObject);
        }
    }
}