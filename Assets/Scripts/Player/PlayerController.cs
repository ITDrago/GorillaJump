using System;
using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public enum State
        {
            Swinging,
            Flying,
            Landing,
            Sliding
        }

        [Header("Movement")]
        [SerializeField] private float _swingSpeed = 180;
        [SerializeField] private float _jumpForce = 10;

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
        
        public event Action<Block> OnLanded;
        public Block AttachedBlock { get; private set; }
        public bool IsFlying => _currentState == State.Flying;
        public Block StartBlock => _startBlock;

        private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

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
    
            if (_stick.LastAttachedBlock)
            {
                AttachedBlock = _stick.LastAttachedBlock;
                _localAttachmentPoint = AttachedBlock.transform.InverseTransformPoint(anchorPoint);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) HandleTap();
        }

        private void FixedUpdate()
        {
            if (_currentState is State.Swinging or State.Sliding)
                UpdateSwing();
        }

        private void HandleTap()
        {
            switch (_currentState)
            {
                case State.Swinging or State.Sliding:
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
            if (AttachedBlock) _anchorPoint = AttachedBlock.transform.TransformPoint(_localAttachmentPoint);
            
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
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody.gravityScale = 1;
            _rigidbody.linearVelocity = -transform.right * _jumpForce;
            
            AttachedBlock = null;
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
            
                OnLanded?.Invoke(AttachedBlock);
            }
        }
        
        public void DetachFromBlock()
        {
            if (_currentState != State.Swinging) return;
    
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            AttachedBlock = null;
        }
    }
}