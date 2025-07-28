using System;
using Environment.Blocks.BlockTypes;
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
            Sliding
        }

        [Header("Movement")]
        [SerializeField] private float _swingSpeed = 180;
        [SerializeField] private float _jumpForce = 10;
        [SerializeField] private float _slideSpeed = 0.5f;

        [Header("References")]
        [SerializeField] private Stick _stick;
        [SerializeField] private Transform _hand;

        [Header("Start Properties")]
        [SerializeField] private Block _startBlock;
        [SerializeField] private float _startAngle = 180;

        private State _currentState = State.Swinging;
        private Rigidbody2D _rigidbody;
        private float _currentAngle;
        private Vector2 _anchorPoint;
        private Vector3 _localAttachmentPoint;

        public Block AttachedBlock { get; private set; }
        public bool IsFlying => _currentState == State.Flying;
        public Block StartBlock => _startBlock;
        
        public event Action<Block, Vector2> OnLanded;
        public event Action<Block> OnJumped;

        private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

        private void Start()
        {
            if (_startBlock)
            {
                StartSwinging(_startBlock.StickAnchor.position, _startAngle);
                AttachedBlock = _startBlock;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) HandleTap();
        }

        private void FixedUpdate()
        {
            switch (_currentState)
            {
                case State.Swinging:
                    ExecuteSwing();
                    break;
                case State.Sliding:
                    ExecuteSlide();
                    break;
            }
        }

        private void HandleTap()
        {
            switch (_currentState)
            {
                case State.Swinging or State.Sliding:
                    Jump();
                    break;
                case State.Flying:
                    TryLand();
                    break;
            }
        }

        private void TryLand()
        {
            if (_stick.TryStickToBlock(out var stickPoint)) OnLanded?.Invoke(_stick.LastAttachedBlock, stickPoint);
        }

        public void InitiateSwing(Vector2 stickPoint)
        {
            Vector2 direction = (transform.position - (Vector3)stickPoint).normalized;
            var landingAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            StartSwinging(stickPoint, landingAngle);
        }

        public void InitiateSlide(IceBlock block, Vector2 stickPoint)
        {
            _currentState = State.Sliding;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;
            AttachedBlock = block;
            _anchorPoint = stickPoint;

            Vector2 direction = (transform.position - (Vector3)stickPoint).normalized;
            _currentAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            UpdateSwingPosition();
        }

        private void StartSwinging(Vector2 anchorPoint, float startAngle)
        {
            _currentState = State.Swinging;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.interpolation = RigidbodyInterpolation2D.None;
            _anchorPoint = anchorPoint;
            _currentAngle = startAngle;

            if (_stick.LastAttachedBlock)
            {
                AttachedBlock = _stick.LastAttachedBlock;
                _localAttachmentPoint = AttachedBlock.transform.InverseTransformPoint(anchorPoint);
            }
            else if (AttachedBlock)
            {
                 _localAttachmentPoint = AttachedBlock.transform.InverseTransformPoint(anchorPoint);
            }

            UpdateSwingPosition();
        }

        private void ExecuteSwing()
        {
            _currentAngle += -_swingSpeed * Time.fixedDeltaTime;
            UpdateSwingPosition();
        }

        private void ExecuteSlide()
        {
            var iceBlock = AttachedBlock as IceBlock;
            if (!iceBlock || !iceBlock.IsStickOnSurface(_anchorPoint))
            {
                DetachFromBlock();
                return;
            }

            _currentAngle += -_swingSpeed * Time.fixedDeltaTime;
            _anchorPoint += iceBlock.SlideDirection * (_slideSpeed * Time.fixedDeltaTime);
            UpdateSwingPosition();
        }

        private void UpdateSwingPosition()
        {
            if (AttachedBlock && _currentState != State.Sliding)
                _anchorPoint = AttachedBlock.transform.TransformPoint(_localAttachmentPoint);

            var direction = new Vector2(
                Mathf.Sin(_currentAngle * Mathf.Deg2Rad),
                Mathf.Cos(_currentAngle * Mathf.Deg2Rad)
            );

            transform.position = _anchorPoint + direction * _stick.StickLength;
            transform.right = direction;
            _stick.UpdateStickPosition(_anchorPoint, direction);

            if (_hand)
            {
                _hand.position = transform.position;
                _hand.up = -direction;
            }
        }

        private void Jump()
        {
            OnJumped?.Invoke(AttachedBlock);
            
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody.gravityScale = 1;
            _rigidbody.linearVelocity = -transform.right * _jumpForce;
            AttachedBlock = null;
        }

        public void DetachFromBlock()
        {
            if (_currentState == State.Flying) return;

            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            AttachedBlock = null;
        }
    }
}