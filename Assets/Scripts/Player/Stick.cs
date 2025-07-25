using UnityEngine;

namespace Player
{
    public class Stick : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float _stickRadius = 0.2f;
        [SerializeField] private LayerMask _blockLayer;
        [SerializeField] private float _maxStickDistance = 0.5f;

        public bool TryStickToBlock(out Vector2 stickPoint)
        {
            stickPoint = Vector2.zero;

            var hit = Physics2D.OverlapCircle(transform.position, _stickRadius, _blockLayer);
            if (!hit) return false;

            stickPoint = hit.ClosestPoint(transform.position);

            if (Vector2.Distance(transform.position, stickPoint) > _maxStickDistance)
                return false;

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stickRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _maxStickDistance);
        }
    }
}