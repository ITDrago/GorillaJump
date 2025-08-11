using System.Linq;
using Environment.Blocks.BlockTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class BirdSpawner : BaseMovingObjectsSpawner<Bird>
    {
        [Header("Spawn Settings")]
        [SerializeField] [Range(0, 1)] private float _spawnChanceInGap = 0.02f;
        [SerializeField] private float _minGapSizeForSpawn = 5;
        [SerializeField] private float _spawnHeightOffset = 8;
        [SerializeField] private Bird _birdPrefab;

        protected override void Awake()
        {
            base.Awake();
            if (!_birdPrefab)
            {
                enabled = false;
                return;
            }
            PlayerController.OnJumped += HandlePlayerJumped;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (PlayerController) PlayerController.OnJumped -= HandlePlayerJumped;
        }

        private void HandlePlayerJumped(Block jumpedFromBlock)
        {
            if (Random.value > _spawnChanceInGap) return;

            var allBlocks = BlockSpawner.SpawnedBlocks.ToList();
            var nextBlock = GetNextBlock(jumpedFromBlock, allBlocks);

            if (!nextBlock) return;

            var gapDistance = Vector3.Distance(jumpedFromBlock.transform.position, nextBlock.transform.position);

            if (gapDistance >= _minGapSizeForSpawn)
            {
                SpawnBird(jumpedFromBlock, nextBlock);
            }
        }

        private void SpawnBird(Block fromBlock, Block toBlock)
        {
            var spawnX = (fromBlock.transform.position.x + toBlock.transform.position.x) / 2f;
            var spawnY = MainCamera.transform.position.y - _spawnHeightOffset;
            var spawnPosition = new Vector2(spawnX, spawnY);
            
            if (IsPositionClear(spawnPosition))
            {
                SpawnObject(_birdPrefab, spawnPosition);
            }
        }

        protected override Bird CreateMovingObjectInstance(Bird prefab, Vector2 spawnPosition) => 
            Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
    }
}