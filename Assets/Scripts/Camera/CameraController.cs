using Player;
using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private PlayerController _playerController;

        [SerializeField] private float _followSpeed = 5f;
        [SerializeField] private float _horizontalOffset = 3f;

        private float _targetX;
        private float _currentVelocity;
        private float _initialY;
        private float _lastFlyingPositionX;

        private void Start()
        {
            if (!_playerController) return;

            _initialY = transform.position.y;

            _targetX = _playerController.transform.position.x + _horizontalOffset;
            _lastFlyingPositionX = _playerController.transform.position.x;
            transform.position = new Vector3(_targetX, _initialY, transform.position.z);
        }

        private void LateUpdate()
        {
            if (!_playerController) return;

            UpdateTargetPosition();
            MoveCamera();
        }

        private void UpdateTargetPosition()
        {
            if (_playerController.IsFlying)
            {
                _lastFlyingPositionX = _playerController.transform.position.x;
                _targetX = _lastFlyingPositionX + _horizontalOffset;
            }
        }

        private void MoveCamera()
        {
            var newX = Mathf.SmoothDamp(
                transform.position.x,
                _targetX,
                ref _currentVelocity,
                _followSpeed * Time.deltaTime
            );

            transform.position = new Vector3(newX, _initialY, transform.position.z);
        }
    }
}