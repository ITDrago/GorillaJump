using Player;
using UnityEngine;

namespace Environment.Obstacles.FallingObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class FallingObject : MonoBehaviour
    {
        [SerializeField] private Vector3 _movementDirection = Vector3.down;
        [SerializeField] private float _fallSpeed = 15f;

        private bool _isMoving;

        private void Update()
        {
            if (_isMoving)
            {
                transform.Translate(_movementDirection.normalized * (_fallSpeed * Time.deltaTime));
            }
        }

        public void StartMovement() => _isMoving = true;

        protected abstract void OnPlayerEnter(PlayerController player);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player)) OnPlayerEnter(player);
        }

        private void OnBecameInvisible() => Destroy(gameObject);
    }
}