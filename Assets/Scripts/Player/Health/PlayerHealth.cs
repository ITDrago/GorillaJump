using System;
using UnityEngine;

namespace Player.Health
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerHealth : MonoBehaviour
    {
        private PlayerController _playerController;
        private bool _isDead;
        
        public event Action OnDied;

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