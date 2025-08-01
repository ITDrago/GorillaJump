using System;
using Environment.Blocks.BlockTypes;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            Swinging,
            Flying,
            Sliding,
            ChargingJump
        }

        [Header("Movement")]
        [SerializeField] private float _swingSpeed = 180;
        [SerializeField] private float _jumpForce = 10;
        [SerializeField] private float _slideSpeed = 0.5f;

        [Header("Charged Jump")]
        [SerializeField] private Vector2 _chargedJumpForceRange = new(12, 25);

        [Header("References")]
        [SerializeField] private Stick _stick;
        [SerializeField] private Transform _hand;
        [SerializeField] private ChargedJumpUI _chargedJumpUI;
        [SerializeField] private ProgressManager _progressManager;

        [Header("Start Properties")]
        [SerializeField] private Block _startBlock;
        [SerializeField] private float _startAngle = 180;

        private State _currentState = State.Swinging;
        private Rigidbody2D _rigidbody;
        private float _currentAngle;
        private Vector2 _anchorPoint;
        private Vector3 _localAttachmentPoint;
        private InputSystem _inputSystem;

        public Block AttachedBlock { get; private set; }
        public bool IsFlying => _currentState == State.Flying;
        public Block StartBlock => _startBlock;
        
        public event Action<Block, Vector2> OnLanded;
        public event Action<Block> OnJumped;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _inputSystem = new InputSystem();
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
            _inputSystem.Player.Tap.performed += HandleTapPerformed;
            _inputSystem.Player.Tap.canceled += HandleTapCanceled;
        }

        private void Start()
        {
            if (_startBlock)
            {
                StartSwinging(_startBlock.StickAnchor.position, _startAngle);
                AttachedBlock = _startBlock;
            }
            if (_chargedJumpUI) _chargedJumpUI.Hide();
        }

        private void OnDisable()
        {
            _inputSystem.Player.Tap.performed -= HandleTapPerformed;
            _inputSystem.Player.Tap.canceled -= HandleTapCanceled;
            _inputSystem.Disable();
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
                case State.ChargingJump:
                    UpdateSwingPosition();
                    break;
            }
        }
        
        private void Update()
        {
            if (_currentState == State.ChargingJump && _chargedJumpUI)
            {
                _chargedJumpUI.UpdateBar();
            }
        }

        private void HandleTapPerformed(InputAction.CallbackContext context)
        {
            if (!_progressManager || !_progressManager.IsChargedJumpUnlocked) return;
            
            switch (_currentState)
            {
                case State.Swinging or State.Sliding:
                    StartChargeJump();
                    break;
            }
        }

        private void HandleTapCanceled(InputAction.CallbackContext context)
        {
            switch (_currentState)
            {
                case State.ChargingJump:
                    ExecuteChargedJump();
                    break;
                case State.Swinging or State.Sliding:
                    Jump(_jumpForce);
                    break;
                case State.Flying:
                    TryLand();
                    break;
            }
        }

        private void TryLand()
        {
            if (_stick.TryStickToBlock(out var stickPoint))
            {
                var landedBlock = _stick.LastAttachedBlock;
                OnLanded?.Invoke(landedBlock, stickPoint);
            }
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

        private void Jump(float force)
        {
            OnJumped?.Invoke(AttachedBlock);
            
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody.gravityScale = 1;
            _rigidbody.linearVelocity = -transform.right * force;
            AttachedBlock = null;
        }

        private void StartChargeJump()
        {
            _currentState = State.ChargingJump;
            if (_chargedJumpUI)
            {
                _chargedJumpUI.Show(transform);
                _chargedJumpUI.StartCharge();
            }
        }

        private void ExecuteChargedJump()
        {
            var finalForce = _jumpForce;
            if (_chargedJumpUI)
            {
                _chargedJumpUI.StopCharge();
                var chargeValue = _chargedJumpUI.GetCurrentChargeValue();
                finalForce = Mathf.Lerp(_chargedJumpForceRange.x, _chargedJumpForceRange.y, chargeValue);
                _chargedJumpUI.Hide();
            }
            Jump(finalForce);
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