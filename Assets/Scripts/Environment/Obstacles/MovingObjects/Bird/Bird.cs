using Core.Game;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class Bird : MovingObject
    {
        [Header("Flight Settings")]
        [SerializeField] private float _minFlightDuration = 0.8f;
        [SerializeField] private float _maxFlightDuration = 2;
        [SerializeField] private float _minHorizontalDistance = 2;
        [SerializeField] private float _maxHorizontalDistance = 20;
        [SerializeField] private float _minVerticalVelocity = 5;

        private Transform _targetBlockTransform;

        private void Start() => SetMovementDirection(Vector3.up);

        public void Initialize(Transform target) => _targetBlockTransform = target;

        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            if (!playerCore.PlayerController.IsFlying || !_targetBlockTransform) return;
            if (!playerCore.PlayerController.TryGetComponent(out Rigidbody2D playerRigidbody)) return;
            if (!_targetBlockTransform.TryGetComponent(out Collider2D targetCollider)) return;

            var startPoint = playerRigidbody.position;
            var bounds = targetCollider.bounds;
            var endPoint = new Vector2(bounds.center.x, bounds.max.y);

            var horizontalDistance = endPoint.x - startPoint.x;
            if (horizontalDistance <= 0) return;

            var heightDifference = endPoint.y - startPoint.y;
            var gravity = Physics2D.gravity.y * playerRigidbody.gravityScale;

            var flightDuration = CalculateFlightDuration(horizontalDistance);
            var initialYVelocity = CalculateVerticalVelocity(heightDifference, gravity, flightDuration);
            initialYVelocity = Mathf.Max(initialYVelocity, _minVerticalVelocity);

            var initialXVelocity = horizontalDistance / flightDuration;
            playerRigidbody.linearVelocity = new Vector2(initialXVelocity - 1, initialYVelocity);

            GameEvents.BirdInteraction();
            Destroy(gameObject);
        }

        private float CalculateFlightDuration(float distance)
        {
            var normalizedDistance = Mathf.InverseLerp(_minHorizontalDistance, _maxHorizontalDistance, distance);
            return Mathf.Lerp(_minFlightDuration, _maxFlightDuration, normalizedDistance);
        }

        private float CalculateVerticalVelocity(float heightDiff, float gravity, float duration) 
            => (heightDiff - 0.5f * gravity * duration * duration) / duration;
    }
}