using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Banana
{
    public class MovingBanana : MovingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player collected a banana!");
            Destroy(gameObject);
        }
    }
}