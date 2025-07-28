using Player;
using UnityEngine;

namespace Environment.Obstacles.FallingObjects.Banana
{
    public class FallingBanana : FallingObject
    {
        protected override void OnPlayerEnter(PlayerController player)
        {
            Debug.Log("Player collected a banana!");
            Destroy(gameObject);
        }
    }
}