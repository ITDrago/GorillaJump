using System;
using Environment.Blocks.BlockTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Core.Game;
using UI.Game;

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

        [Header("Charged Jump")]
        [SerializeField] private Vector2 _chargedJumpForceRange = new(11, 14);

        [Header("References")]
        [SerializeField] private Stick _stick;
        [SerializeField] private Transform _gorillaBody;
        [SerializeField] private ChargedJumpUI _chargedJumpUI;

        [Header("Start Properties")]
        [SerializeField] private Block _startBlock;
        [SerializeField] private float _startAngle = 180;

        private State _currentState = State.Swinging;
        private State _previousState;
        private Rigidbody2D _rigidbody;
        private float _currentAngle;
        private bool _isChargedJump;
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
                RespawnAt(_startBlock);
            }
            if (_chargedJumpUI) _chargedJumpUI.Hide();
        }

        private void OnDisable()
        {
            _inputSystem.Player.Tap.performed -= HandleTapPerformed;
            _inputSystem.Player.Tap.canceled -= HandleTapCanceled;
            _inputSystem.Disable();
        }

        private void FixedUpdate() => HandleAttachedState();

        private void HandleAttachedState()
        {
            switch (_currentState)
            {
                case State.Swinging:
                    ExecuteSwing();
                    break;
                case State.Sliding:
                    ExecuteSlide();
                    ExecuteSwing();
                    break;
            }
        }
        
        private void ExecuteSwing()
        {
            if (!_isChargedJump) _gorillaBody.Rotate(0, 0, _swingSpeed * Time.fixedDeltaTime);
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

            _anchorPoint += iceBlock.SlideDirection * (_slideSpeed * Time.fixedDeltaTime);
        }
        
        private void Update()
        {
            if (_isChargedJump && _chargedJumpUI)
            {
                _chargedJumpUI.UpdateBar();
            }
        }

        private void HandleTapPerformed(InputAction.CallbackContext context)
        {
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
                case State.Swinging or State.Sliding:
                    Jump(_jumpForce);
                    break;
                case State.Flying:
                    TryLand();
                    break;
            }
            
            if (_isChargedJump) ExecuteChargedJump();
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

        private void RespawnAt(Block respawnBlock)
        {
            StartSwinging(respawnBlock.StickAnchor.position, _startAngle);
            AttachedBlock = respawnBlock;
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
        
        private void UpdateSwingPosition()
        {
            if (AttachedBlock && _currentState != State.Sliding)
                _anchorPoint = AttachedBlock.transform.TransformPoint(_localAttachmentPoint);

            var direction = new Vector2(
                Mathf.Sin(_currentAngle * Mathf.Deg2Rad),
                Mathf.Cos(_currentAngle * Mathf.Deg2Rad)
            );

            transform.position = _anchorPoint + direction * _stick.StickLength;
            _stick.UpdateStickPosition(_anchorPoint, direction);
        }

        private void Jump(float force)
        {
            GameEvents.PlayerJumped();
            OnJumped?.Invoke(AttachedBlock);
            
            _currentState = State.Flying;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody.gravityScale = 1;
            _rigidbody.linearVelocity = _gorillaBody.right * force;
            AttachedBlock = null;
        }

        private void StartChargeJump()
        {
            if (_chargedJumpUI)
            {
                _isChargedJump = true;
                _chargedJumpUI.Show(transform);
                _chargedJumpUI.StartCharge();
            }
        }

        private void ExecuteChargedJump()
        {
            _isChargedJump = false;
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