using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Player
{
    public class Stick : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private float _stickRadius = 0.17f;
        [SerializeField] private float _maxStickDistance = 1.25f;
        [SerializeField] private float _stickLength = 1.5f;
        
        [SerializeField] private LayerMask _blockLayer;

        public Block LastAttachedBlock { get; private set; }
        public float StickLength => _stickLength;

        public bool TryStickToBlock(out Vector2 stickPoint)
        {
            stickPoint = Vector2.zero;
            LastAttachedBlock = null;

            var hit = Physics2D.OverlapCircle(transform.position, _stickRadius, _blockLayer);
            if (!hit) return false;

            stickPoint = hit.ClosestPoint(transform.position);
            LastAttachedBlock = hit.GetComponent<Block>();

            if (!LastAttachedBlock || Vector2.Distance(transform.position, stickPoint) > _maxStickDistance)
                return false;

            return true;
        }

        public void UpdateStickPosition(Vector2 anchorPoint, Vector2 direction)
        {
            transform.position = anchorPoint;
            transform.right = direction;
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