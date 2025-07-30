using System;
using System.Collections;
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
        [SerializeField] private SpawnableObject[] _specialSpawnableObjects;
        [SerializeField] private SpawnableObject _stickPrefab;
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);
        [SerializeField] private Vector2 _spawnDelay = new(2, 5);
        [SerializeField] private Vector2 _spawnInterval = new(0.8f, 1.5f);
        [SerializeField] private float _spawnHeightOffset = 10;

        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private Coroutine _spawningRoutine;
        private Block _currentSpawnBlock;
        private bool _isSpawning;

        protected override void Awake()
        {
            base.Awake();
            if (_specialSpawnableObjects is null || _specialSpawnableObjects.Length == 0 || _stickPrefab is null)
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
            StopSpawning();
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            _blockCounter++;
            if (_blockCounter < _blocksUntilNextSpawn) return;

            StopSpawning();
            StartSpawning(landedBlock);
            
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private void StartSpawning(Block landedBlock)
        {
            _currentSpawnBlock = landedBlock;
            _spawningRoutine = StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            _isSpawning = true;
            
            yield return new WaitForSeconds(Random.Range(_spawnDelay.x, _spawnDelay.y));
            
            while (_currentSpawnBlock && BlockSpawner.SpawnedBlocks.Contains(_currentSpawnBlock))
            {
                var prefabToSpawn = GetRandomObject();
                SpawnBetweenBlocks(prefabToSpawn, _currentSpawnBlock);
                
                yield return new WaitForSeconds(Random.Range(_spawnInterval.x, _spawnInterval.y));
            }
            
            _isSpawning = false;
        }

        private MovingObject GetRandomObject()
        {
            var spawnableObject = _specialSpawnableObjects[Random.Range(0, _specialSpawnableObjects.Length)];
            if (Random.value <= spawnableObject.SpawnChance)
            {
                return spawnableObject.Prefab;
            }
            return _stickPrefab.Prefab;
        }

        private void SpawnBetweenBlocks(MovingObject prefab, Block currentBlock)
        {
            var allBlocks = BlockSpawner.SpawnedBlocks;
            var nextBlock = GetNextBlock(currentBlock, allBlocks.ToList());
            if (!nextBlock) return;

            var startX = currentBlock.transform.position.x;
            var endX = nextBlock.transform.position.x;
            var spawnX = (startX + endX) / 2;
            
            var distance = Mathf.Abs(endX - startX);
            var randomOffset = Random.Range(-distance/20, distance/20);
            spawnX += randomOffset;
            
            var spawnY = MainCamera.transform.position.y + _spawnHeightOffset;

            SpawnObject(prefab, new Vector2(spawnX, spawnY));
        }

        private void StopSpawning()
        {
            if (_spawningRoutine != null)
            {
                StopCoroutine(_spawningRoutine);
                _spawningRoutine = null;
            }
            _isSpawning = false;
            _currentSpawnBlock = null;
        }

        protected override MovingObject CreateMovingObjectInstance(MovingObject prefab, Vector2 spawnPosition) => 
            Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

        private void SetNextSpawnInterval() => 
            _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
    }
}