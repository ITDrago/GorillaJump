using Core;
using Player;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class Bird : MovingObject
    {
        [SerializeField] private float _pushForce = 10;
        [SerializeField] private Vector3 _flyDirection = Vector3.up;

        private void Start() => SetMovementDirection(_flyDirection);

        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            if (!playerCore.PlayerController.IsFlying) return;

            var playerRigidbody = playerCore.PlayerController.GetComponent<Rigidbody2D>();
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.AddForce((transform.up + transform.right) * _pushForce, ForceMode2D.Impulse);
            
            GameEvents.BirdInteraction();
            Destroy(gameObject);
        }
    }
}