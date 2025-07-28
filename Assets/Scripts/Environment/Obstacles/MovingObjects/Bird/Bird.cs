using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class Bird : MovingObject
    {
        [Header("Bird Settings")]
        [SerializeField] private float _pushForce = 20f;
        [SerializeField] private Vector3 _flyDirection = Vector3.up;

        private void Start()
        {
            SetMovementDirection(_flyDirection);
        }

        protected override void OnPlayerEnter(PlayerController player)
        {
            if (!player.IsFlying) return;
            
            if (player.TryGetComponent<Rigidbody2D>(out var playerRigidbody))
            {
                playerRigidbody.linearVelocity = Vector2.zero;
                playerRigidbody.AddForce((transform.up + transform.right) * _pushForce, ForceMode2D.Impulse);
                Destroy(gameObject);
            }
        }
    }
}