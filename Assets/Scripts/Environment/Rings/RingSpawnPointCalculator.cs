using UnityEngine;

namespace Environment.Rings
{
    public class RingSpawnPointCalculator
    {
        private readonly LayerMask _obstacleLayerMask;
        private readonly Vector2 _clearanceBoxSize;

        public RingSpawnPointCalculator(LayerMask obstacleLayerMask, Vector2 clearanceBoxSize)
        {
            _obstacleLayerMask = obstacleLayerMask;
            _clearanceBoxSize = clearanceBoxSize;
        }

        public bool IsPositionClear(Vector2 position) => !Physics2D.BoxCast(position, _clearanceBoxSize, 0, Vector2.down, 0.1f, _obstacleLayerMask);
    }
}