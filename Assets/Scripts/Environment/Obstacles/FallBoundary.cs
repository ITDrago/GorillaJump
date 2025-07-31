using Player;
using UnityEngine;

namespace Environment.Obstacles
{
    [RequireComponent(typeof(Collider2D))]
    public class FallBoundary : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth)) playerHealth.Die();
        }
    }
}