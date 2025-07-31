using System.Collections.Generic;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects
{
    public abstract class BaseMovingObjectsSpawner<T> : MonoBehaviour where T : MovingObject
    {
        [Header("References")]
        [SerializeField] protected Player.PlayerController PlayerController;
        [SerializeField] protected BlockSpawner BlockSpawner;

        [Header("Safety Checks")]
        [SerializeField] protected LayerMask SpawnBlockingLayers;
        [SerializeField] private Vector2 _clearanceBoxSize = new(2, 30);

        protected UnityEngine.Camera MainCamera;
        
        private Vector2 _lastPosition;

        protected virtual void Awake()
        {
            if (!PlayerController || !BlockSpawner)
            {
                enabled = false;
            }
        }

        protected virtual void Start() => MainCamera = UnityEngine.Camera.main;

        protected virtual void OnDestroy() { }

        protected abstract T CreateMovingObjectInstance(T prefab, Vector2 spawnPosition);

        protected void SpawnObject(T prefab, Vector2 spawnPosition)
        {
            if (!prefab || !MainCamera) return;

            var instance = CreateMovingObjectInstance(prefab, spawnPosition);
            instance.StartMovement();
        }
        
        protected bool IsPositionClear(Vector2 position)
        {
            _lastPosition = position;
            return !Physics2D.BoxCast(position, _clearanceBoxSize, 0, Vector2.down, 0.1f, SpawnBlockingLayers);
        }

        protected Block GetNextBlock(Block currentBlock, List<Block> allBlocks)
        {
            var currentIndex = allBlocks.IndexOf(currentBlock);
            if (currentIndex == -1 || currentIndex + 1 >= allBlocks.Count)
            {
                return null;
            }
            return allBlocks[currentIndex + 1];
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_lastPosition, new Vector3(_clearanceBoxSize.x, _clearanceBoxSize.y));
        }
    }
}