using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    [RequireComponent(typeof(Collider2D))]
    public class IceBlock : Block
    {
        [SerializeField] private Vector2 _slideDirection = Vector2.right;

        private Collider2D _iceCollider;

        public Vector2 SlideDirection => _slideDirection.normalized;

        private void Awake() => _iceCollider = GetComponent<Collider2D>();

        public bool IsStickOnSurface(Vector2 stickPoint)
        {
            if (!_iceCollider) return false;
            
            return _iceCollider.OverlapPoint(stickPoint);
        }
    }
}