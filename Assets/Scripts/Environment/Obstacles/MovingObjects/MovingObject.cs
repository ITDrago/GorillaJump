using Core.Audio;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class MovingObject : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 15;

        [Header("Audio")] [SerializeField] private AudioClip _interactionSound;

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

        public void SetMovementSpeed(float newSpeed) => _movementSpeed = newSpeed;

        protected void SetMovementDirection(Vector3 direction) => _movementDirection = direction;

        protected abstract void OnPlayerEnter(PlayerCore playerCore);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCore playerCore))
            {
                OnPlayerEnter(playerCore);
                SoundManager.Instance.PlaySfx(_interactionSound);
            }
        }

        private void OnBecameInvisible() => Destroy(gameObject);
    }
}