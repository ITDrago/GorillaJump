using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private Transform _stickAnchor;

        public Transform StickAnchor => _stickAnchor;
        
        public virtual Vector2 GetLandingPoint(Vector2 playerPosition)
        {
            return _stickAnchor.position;
        }
    }
}