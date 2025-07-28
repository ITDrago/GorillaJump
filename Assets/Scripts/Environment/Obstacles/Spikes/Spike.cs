using Player;
using UnityEngine;

namespace Environment.Obstacles.Spikes
{
    [RequireComponent(typeof(Collider2D))]
    public class Spike : MonoBehaviour
    {
        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if (player.IsFlying) return;
                
                Debug.Log("Player was hit by a spike!");
                player.DetachFromBlock();
            }
        }
    }
}