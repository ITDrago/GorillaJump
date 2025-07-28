using Player;
using UnityEngine;

namespace Environment.Obstacles.FallingObjects.Branch
{
    public class FallingBranch : FallingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player was hit by a falling branch!");
            Destroy(gameObject);
        }
    }
}