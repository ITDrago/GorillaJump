using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Environment.Rings
{
    public class RingSpawnPointCalculator
    {
        private readonly LayerMask _obstacleLayerMask;
        private readonly float _clearCheckRadius;
        private readonly float _minDistanceFromBlock;

        public RingSpawnPointCalculator(LayerMask obstacleLayerMask, float clearCheckRadius, float minDistanceFromBlock)
        {
            _obstacleLayerMask = obstacleLayerMask;
            _clearCheckRadius = clearCheckRadius;
            _minDistanceFromBlock = minDistanceFromBlock;
        }

        public bool TryCalculateSpawnPosition(Block prevBlock, Block nextBlock, Vector2 proposedPosition, out Vector2 position)
        {
            position = proposedPosition;

            if (!IsSufficientlyFarFromBlocks(position, prevBlock, nextBlock))
            {
                return false;
            }

            return !IsObstructed(position);
        }

        private bool IsSufficientlyFarFromBlocks(Vector2 position, Block prevBlock, Block nextBlock)
        {
            var distanceToPrev = Vector2.Distance(position, prevBlock.transform.position);
            var distanceToNext = Vector2.Distance(position, nextBlock.transform.position);

            return distanceToPrev >= _minDistanceFromBlock && distanceToNext >= _minDistanceFromBlock;
        }

        private bool IsObstructed(Vector2 position)
        {
            return Physics2D.OverlapCircle(position, _clearCheckRadius, _obstacleLayerMask) != null;
        }
    }
}