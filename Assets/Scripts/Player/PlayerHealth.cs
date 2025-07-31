using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerHealth : MonoBehaviour
    {
        public event Action OnDied;

        private PlayerController _playerController;
        private bool _isDead;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void Die()
        {
            if (_isDead) return;

            _isDead = true;
            OnDied?.Invoke();
            
            _playerController.DetachFromBlock();
        }
    }
}