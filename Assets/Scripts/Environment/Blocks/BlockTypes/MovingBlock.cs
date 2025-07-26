using System;
using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    public class MovingBlock : Block
    {
        public enum MovementType
        {
            Horizontal,
            Vertical
        }

        [Header("Movement Settings")] 
        [SerializeField] private MovementType _movementType = MovementType.Horizontal;

        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _movementDistance = 3f;
        [SerializeField] private float _startDelay = 0.5f;

        [Header("Randomization")] 
        [SerializeField] private bool _randomizeDirection = true;
        [SerializeField] private bool _randomizeSpeed = true;
        [SerializeField] private Vector2 _speedRange = new(1f, 3f);
        [SerializeField] private Vector2 _distanceRange = new(2f, 4f);

        private Rigidbody2D _rigidbody;
        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private Vector2 _movementDirection;
        private bool _isMovingForward = true;
        private float _currentDelay;

        public event Action OnMovementStarted;
        public event Action OnMovementCompleted;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody2D>();
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        private void Start()
        {
            InitializeMovement();
            StartMovement();
        }

        private void InitializeMovement()
        {
            _startPosition = transform.position;

            if (_randomizeDirection) _movementType = (MovementType)UnityEngine.Random.Range(0, 2);

            if (_randomizeSpeed)
            {
                _movementSpeed = UnityEngine.Random.Range(_speedRange.x, _speedRange.y);
                _movementDistance = UnityEngine.Random.Range(_distanceRange.x, _distanceRange.y);
            }
            
            switch (_movementType)
            {
                case MovementType.Horizontal:
                    _movementDirection = Vector2.right;
                    _targetPosition = _startPosition + Vector2.right * _movementDistance;
                    break;
                case MovementType.Vertical:
                    _movementDirection = Vector2.up;
                    _targetPosition = _startPosition + Vector2.up * _movementDistance;
                    break;
            }
        }

        private void StartMovement()
        {
            _currentDelay = _startDelay;
            OnMovementStarted?.Invoke();
        }

        private void FixedUpdate()
        {
            if (_currentDelay > 0)
            {
                _currentDelay -= Time.fixedDeltaTime;
                return;
            }

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            var target = _isMovingForward ? _targetPosition : _startPosition;
            var newPosition = Vector2.MoveTowards(
                _rigidbody.position,
                target,
                _movementSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MovePosition(newPosition);

            if (Vector2.Distance(newPosition, target) < 0.01f)
            {
                _isMovingForward = !_isMovingForward;
                OnMovementCompleted?.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                var start = transform.position;
                var end = start + (_movementType == MovementType.Horizontal
                    ? Vector3.right * _movementDistance
                    : Vector3.up * _movementDistance);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawWireSphere(end, 0.2f);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_startPosition, 0.2f);
                Gizmos.DrawWireSphere(_targetPosition, 0.2f);
                Gizmos.DrawLine(_startPosition, _targetPosition);
            }
        }
#endif
    }
}