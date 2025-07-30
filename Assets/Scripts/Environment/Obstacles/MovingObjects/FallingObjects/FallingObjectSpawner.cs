using System;
using System.Collections.Generic;
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
        [SerializeField] private SpawnType _spawnType;

        public enum SpawnType { Single, Continuous }

        public MovingObject Prefab => _prefab;
        public float SpawnChance => _spawnChance;
        public SpawnType ObjectType => _spawnType;
    }

    public class FallingObjectSpawner : BaseMovingObjectsSpawner<MovingObject>
    {
        [Header("Spawn Settings")]
        [SerializeField] private SpawnableObject[] _spawnableObjects;
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);
        [SerializeField] private Vector2Int _spawnDelay = new(2, 5);
        [SerializeField] private Vector2 _continuousSpawnInterval = new(0.5f, 1.5f);
        [SerializeField] private float _spawnHeightOffset = 10;

        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private Coroutine _continuousSpawningRoutine;
        private Block _currentSpawnBlock;
        
        private List<SpawnableObject> _singleObjects = new();
        private List<SpawnableObject> _continuousObjects = new();

        protected override void Awake()
        {
            base.Awake();
            if (_spawnableObjects is not { Length: not 0 })
            {
                enabled = false;
                return;
            }

            CategorizeSpawnableObjects();
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
            StopContinuousSpawning();
        }

        private void CategorizeSpawnableObjects()
        {
            foreach (var spawnable in _spawnableObjects)
            {
                if (spawnable.ObjectType == SpawnableObject.SpawnType.Single)
                    _singleObjects.Add(spawnable);
                else
                    _continuousObjects.Add(spawnable);
            }
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            _blockCounter++;
            if (_blockCounter < _blocksUntilNextSpawn) return;

            StopContinuousSpawning();
            
            if (TrySpawnContinuousObjects(landedBlock)) {}
            if (TrySpawnSingleObject(landedBlock)) {}
            
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private bool TrySpawnContinuousObjects(Block landedBlock)
        {
            if (_continuousObjects.Count == 0) return false;

            var selected = SelectRandomObject(_continuousObjects);
            if (selected == null) return false;

            _currentSpawnBlock = landedBlock;
            _continuousSpawningRoutine = StartCoroutine(ContinuousSpawning(selected.Prefab));
            return true;
        }

        private bool TrySpawnSingleObject(Block landedBlock)
        {
            if (_singleObjects.Count == 0) return false;

            var selected = SelectRandomObject(_singleObjects);
            if (selected == null) return false;

            SpawnSingleObject(selected.Prefab, landedBlock);
            return true;
        }

        private SpawnableObject SelectRandomObject(List<SpawnableObject> spawnables)
        {
            var totalChance = spawnables.Sum(s => s.SpawnChance);
            if (totalChance <= 0) return null;

            var randomValue = Random.Range(0, totalChance);
            float currentSum = 0;

            foreach (var spawnable in spawnables)
            {
                currentSum += spawnable.SpawnChance;
                if (currentSum >= randomValue)
                    return spawnable;
            }

            return null;
        }

        private System.Collections.IEnumerator ContinuousSpawning(MovingObject prefab)
        {
            while (_currentSpawnBlock && BlockSpawner.SpawnedBlocks.Contains(_currentSpawnBlock))
            {
                yield return new WaitForSeconds(Random.Range(_continuousSpawnInterval.x, _continuousSpawnInterval.y));
                SpawnBetweenBlocks(prefab, _currentSpawnBlock);
            }
        }

        private async void SpawnSingleObject(MovingObject prefab, Block currentBlock)
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
            
            SpawnBetweenBlocks(prefab, currentBlock);
        }

        private void SpawnBetweenBlocks(MovingObject prefab, Block currentBlock)
        {
            var allBlocks = BlockSpawner.SpawnedBlocks;
            var nextBlock = GetNextBlock(currentBlock, allBlocks.ToList());
            if (!nextBlock) return;

            var startX = currentBlock.transform.position.x;
            var endX = nextBlock.transform.position.x;
            var spawnX = (startX + endX) / 2f;
            var spawnY = MainCamera.transform.position.y + _spawnHeightOffset;

            SpawnObject(prefab, new Vector2(spawnX, spawnY));
        }

        private void StopContinuousSpawning()
        {
            if (_continuousSpawningRoutine != null)
            {
                StopCoroutine(_continuousSpawningRoutine);
                _continuousSpawningRoutine = null;
            }
            _currentSpawnBlock = null;
        }

        protected override MovingObject CreateMovingObjectInstance(MovingObject prefab, Vector2 spawnPosition) => 
            Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

        private void SetNextSpawnInterval() => 
            _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
    }
}