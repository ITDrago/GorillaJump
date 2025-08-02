using UnityEngine;

namespace Player
{
    public class PlayerCore : MonoBehaviour
    {
        public PlayerController PlayerController { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }

        private void Awake()
        {
            PlayerController = GetComponentInParent<PlayerController>();
            PlayerHealth = GetComponentInParent<PlayerHealth>();
        }
    }
}