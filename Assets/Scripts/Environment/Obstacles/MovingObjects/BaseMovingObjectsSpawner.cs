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
        [SerializeField] private float _clearanceRadius = 6;

        protected UnityEngine.Camera MainCamera;

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
            return Physics2D.OverlapCircle(position, _clearanceRadius, SpawnBlockingLayers) == null;
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
    }
}