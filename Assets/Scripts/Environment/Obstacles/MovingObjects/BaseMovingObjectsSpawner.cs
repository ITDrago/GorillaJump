using System.Collections.Generic;
using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Environment.Obstacles.MovingObjects
{
    public abstract class BaseMovingObjectsSpawner<T> : MonoBehaviour where T : MovingObject
    {
        [Header("References")]
        [SerializeField] protected Player.PlayerController PlayerController;
        [SerializeField] protected Environment.Blocks.BlockSpawner BlockSpawner;

        protected UnityEngine.Camera MainCamera;

        protected virtual void Awake()
        {
            if (!PlayerController || !BlockSpawner) enabled = false;
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

        protected Block GetNextBlock(Block currentBlock, List<Block> allBlocks)
        {
            var currentIndex = allBlocks.IndexOf(currentBlock);
            if (currentIndex == -1 || currentIndex + 1 >= allBlocks.Count) return null;
            return allBlocks[currentIndex + 1];
        }
    }
}