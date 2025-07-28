using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class MovingObject : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 15f;
        
        private Vector3 _movementDirection = Vector3.down;
        private bool _isMoving;

        private void Update()
        {
            if (_isMoving)
            {
                transform.Translate(_movementDirection.normalized * (_movementSpeed * Time.deltaTime));
            }
        }

        public void StartMovement() => _isMoving = true;

        protected void SetMovementDirection(Vector3 direction) => _movementDirection = direction;

        protected abstract void OnPlayerEnter(PlayerController player);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                OnPlayerEnter(player);
            }
        }

        private void OnBecameInvisible() => Destroy(gameObject);
    }
}