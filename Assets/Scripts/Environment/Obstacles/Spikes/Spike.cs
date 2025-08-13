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
            if (other.TryGetComponent(out PlayerCore playerCore))
            {
                if (playerCore.PlayerController.IsFlying) return;
                
                if (playerCore.PlayerShieldController && playerCore.PlayerShieldController.IsShieldActive)
                {
                    playerCore.PlayerShieldController.Deactivate();
                    return;
                }
                
                playerCore.PlayerHealth.Die();
            }
        }
    }
}