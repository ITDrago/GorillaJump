using UnityEngine;

namespace Environment.Rings.RingTypes
{
    public class MovingRing : Ring
    {
        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 2;
        [SerializeField] private float _movementDistance = 2;
        [SerializeField] private float _startDelay = 0.5f;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isMovingToTarget;
        private float _currentDelay;

        private void Start()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + Vector3.up * _movementDistance;
            _currentDelay = _startDelay;
            _isMovingToTarget = true;
        }

        private void Update()
        {
            if (_currentDelay > 0)
            {
                _currentDelay -= Time.deltaTime;
                return;
            }

            Move();
        }

        private void Move()
        {
            var currentTarget = _isMovingToTarget ? _targetPosition : _startPosition;
            transform.position =
                Vector3.MoveTowards(transform.position, currentTarget, _movementSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
            {
                _isMovingToTarget = !_isMovingToTarget;
                _currentDelay = _startDelay;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            var gizmoStart = Application.isPlaying ? _startPosition : transform.position;
            var gizmoEnd = gizmoStart + Vector3.up * _movementDistance;

            Gizmos.DrawWireSphere(gizmoStart, 0.5f);
            Gizmos.DrawWireSphere(gizmoEnd, 0.5f);
            Gizmos.DrawLine(gizmoStart, gizmoEnd);
        }
#endif
    }
}