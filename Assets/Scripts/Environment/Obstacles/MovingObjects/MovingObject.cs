using Core.Audio;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class MovingObject : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 15f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _interactionSound;
        
        private Vector3 _movementDirection = Vector3.down;
        private bool _isMoving;
        
        private SoundManager _soundManager;

        public virtual void Start() => _soundManager = (SoundManager)FindFirstObjectByType(typeof(SoundManager));

        private void Update()
        {
            if (_isMoving)
            {
                transform.Translate(_movementDirection.normalized * (_movementSpeed * Time.deltaTime));
            }
        }

        public void StartMovement() => _isMoving = true;

        protected void SetMovementDirection(Vector3 direction) => _movementDirection = direction;

        protected abstract void OnPlayerEnter(PlayerCore playerCore);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCore playerCore))
            {
                OnPlayerEnter(playerCore);
                _soundManager.PlaySfx(_interactionSound);
            }
        }

        private void OnBecameInvisible() => Destroy(gameObject);
    }
}