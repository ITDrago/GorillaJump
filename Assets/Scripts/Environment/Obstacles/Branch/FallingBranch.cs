using Player;
using UnityEngine;

namespace Environment.Obstacles.Branch
{
    [RequireComponent(typeof(Collider2D))]
    public class FallingBranch : MonoBehaviour
    {
        [SerializeField] private float _fallSpeed = 20;

        private bool _isFalling;

        private void Update()
        {
            if (_isFalling) transform.Translate(Vector3.down * (_fallSpeed * Time.deltaTime));
        }
        
        public void StartFalling() => _isFalling = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                Debug.Log("Player was hit by a falling branch!");
                Destroy(gameObject);
            }
        }

        private void OnBecameInvisible() => Destroy(gameObject);
    }
}