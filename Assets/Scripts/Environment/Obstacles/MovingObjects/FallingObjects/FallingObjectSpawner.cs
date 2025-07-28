using System;
using System.Linq;
using Environment.Blocks.BlockTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Obstacles.MovingObjects.FallingObjects
{
    [Serializable]
    public class SpawnableObject
    {
        [SerializeField] private MovingObject _prefab;
        [SerializeField] [Range(0, 1)] private float _spawnChance = 0.5f;

        public MovingObject Prefab => _prefab;
        public float SpawnChance => _spawnChance;
    }

    public class FallingObjectSpawner : BaseMovingObjectsSpawner<MovingObject>
    {
        [Header("Spawn Settings")]
        [SerializeField] private SpawnableObject[] _spawnableObjects;
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);
        [SerializeField] private Vector2Int _spawnDelay = new(2, 5);
        [SerializeField] private float _spawnHeightOffset = 10;

        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private MovingObject _selectedPrefabForSpawn;

        protected override void Awake()
        {
            base.Awake();
            if (_spawnableObjects is not { Length: not 0 })
            {
                enabled = false;
                return;
            }
            PlayerController.OnLanded += HandlePlayerLanded;
        }

        protected override void Start()
        {
            base.Start();
            SetNextSpawnInterval();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (PlayerController) PlayerController.OnLanded -= HandlePlayerLanded;
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            _blockCounter++;
            if (_blockCounter < _blocksUntilNextSpawn) return;

            TrySelectRandomObject(landedBlock);
            
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private void TrySelectRandomObject(Block currentBlock)
        {
            _selectedPrefabForSpawn = null; 

            foreach (var spawnable in _spawnableObjects)
            {
                if (Random.value <= spawnable.SpawnChance)
                {
                    _selectedPrefabForSpawn = spawnable.Prefab; 
                    SpawnFallingObject(_selectedPrefabForSpawn, currentBlock); 
                    return; 
                }
            }
        }
        
        private async void SpawnFallingObject(MovingObject prefab, Block currentBlock)
        {
            if (!prefab || !currentBlock) return;

            try
            {
                await Awaitable.WaitForSecondsAsync(Random.Range(_spawnDelay.x, _spawnDelay.y), destroyCancellationToken);
            }
            catch (OperationCanceledException) 
            {
                return;
            }
                
            var allBlocks = BlockSpawner.SpawnedBlocks;
            var nextBlock = GetNextBlock(currentBlock, allBlocks.ToList());

            if (!nextBlock) return;

            var startX = currentBlock.transform.position.x;
            var endX = nextBlock.transform.position.x;
            var spawnX = (startX + endX) / 2f;
            var spawnY = MainCamera.transform.position.y + _spawnHeightOffset;

            var spawnPosition = new Vector2(spawnX, spawnY);
            
            SpawnObject(_selectedPrefabForSpawn, spawnPosition); 
        }

        protected override MovingObject CreateMovingObjectInstance(MovingObject prefab, Vector2 spawnPosition) => Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

        private void SetNextSpawnInterval() => _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
    }
}