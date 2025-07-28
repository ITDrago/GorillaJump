using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Banana
{
    public class FallingBanana : MovingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player collected a banana!");
            Destroy(gameObject);
        }
    }
}