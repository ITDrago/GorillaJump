using Environment;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            Swinging,
            Flying,
            Landing
        }

        [Header("Movement")] [SerializeField] private float _swingSpeed = 180f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _stickLength = 2f;

        [Header("References")] [SerializeField]
        private Stick _stick;

        [SerializeField] private Transform _hand;

        [Header("Start Properties")] [SerializeField]
        private Block _startBlock;

        [SerializeField] private float _startAngle = 180f;

        private State _currentState = State.Swinging;
        private float _currentAngle;
        private Rigidbody2D _rigidbody;
        private Vector2 _anchorPoint;

        public bool IsFlying => _currentState == State.Flying;
        public Block StartBlock => _startBlock;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (_startBlock) StartSwinging(_startBlock.StickAnchor.position);
        }

        private void StartSwinging(Vector2 anchorPoint, float startAngle = -999f)
        {
            _anchorPoint = anchorPoint;
            _currentState = State.Swinging;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _currentAngle = startAngle > -500f ? startAngle : _startAngle;
            UpdateSwingPosition();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) HandleTap();
        }

        private void FixedUpdate()
        {
            if (_currentState == State.Swinging)
                UpdateSwing();
        }

        private void HandleTap()
        {
            switch (_currentState)
            {
                case State.Swinging:
                    Jump();
                    break;
                case State.Flying: TryLand(); break;
            }
        }

        private void UpdateSwing()
        {
            _currentAngle += -_swingSpeed * Time.fixedDeltaTime;
            UpdateSwingPosition();
        }

        private void UpdateSwingPosition()
        {
            var direction = new Vector2(
                Mathf.Sin(_currentAngle * Mathf.Deg2Rad),
                Mathf.Cos(_currentAngle * Mathf.Deg2Rad)
            );

            transform.position = _anchorPoint + direction * _stickLength;

            transform.right = direction;

            _stick.transform.position = _anchorPoint;
            _stick.transform.right = direction;

            if (_hand)
            {
                _hand.position = transform.position;
                _hand.up = -direction;
            }
        }

        private void Jump()
        {
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody.linearVelocity = -transform.right * _jumpForce;
        }

        private void TryLand()
        {
            if (_stick.TryStickToBlock(out var stickPoint))
            {
                _currentState = State.Landing;
                _rigidbody.interpolation = RigidbodyInterpolation2D.None;
                _anchorPoint = stickPoint;

                Vector2 direction = (transform.position - (Vector3)_anchorPoint).normalized;
                var landingAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

                StartSwinging(_anchorPoint, landingAngle);
            }
        }
    }
}