using System;
using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Player;

namespace Environment.Obstacles
{
    using UnityEngine;

    [Serializable]
    public class SpawnableObject
    {
        [SerializeField] private FallingObject _prefab;
        [SerializeField] [Range(0, 1)] private float _spawnChance = 0.5f;

        public FallingObject Prefab => _prefab;
        public float SpawnChance => _spawnChance;
    }

    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BlockSpawner _blockSpawner;
        [SerializeField] private PlayerController _playerController;
        
        [Header("Spawn Settings")]
        [SerializeField] private SpawnableObject[] _spawnableObjects;
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);
        [SerializeField] private Vector2Int _spawnDelay = new(2, 5);
        [SerializeField] private float _spawnHeightOffset = 10;

        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private Camera _mainCamera;

        private void Awake()
        {
            if (!_blockSpawner || !_playerController || _spawnableObjects.Length == 0)
            {
                enabled = false;
                return;
            }
            _playerController.OnLanded += HandlePlayerLanded;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            SetNextSpawnInterval();
        }

        private void OnDestroy()
        {
            if (_playerController) _playerController.OnLanded -= HandlePlayerLanded;
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            _blockCounter++;
            if (_blockCounter < _blocksUntilNextSpawn) return;
            
            TrySpawnRandomObject(landedBlock);
            
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private void TrySpawnRandomObject(Block currentBlock)
        {
            foreach (var spawnable in _spawnableObjects)
            {
                if (Random.value <= spawnable.SpawnChance)
                {
                    SpawnObject(spawnable.Prefab, currentBlock);
                    return; 
                }
            }
        }
        
        private async void SpawnObject(FallingObject prefab, Block currentBlock)
        {
            if (!prefab || !_mainCamera || !currentBlock) return;

            try
            {
                await Awaitable.WaitForSecondsAsync(Random.Range(_spawnDelay.x, _spawnDelay.y), destroyCancellationToken);
            }
            catch
            {
                return;
            }
                
            var allBlocks = _blockSpawner.SpawnedBlocks;
            var currentIndex = allBlocks.ToList().IndexOf(currentBlock);

            if (currentIndex == -1 || currentIndex + 1 >= allBlocks.Count) return;

            var nextBlock = allBlocks[currentIndex + 1];

            var startX = currentBlock.transform.position.x;
            var endX = nextBlock.transform.position.x;
            var spawnX = (startX + endX) / 2f;
            var spawnY = _mainCamera.transform.position.y + _spawnHeightOffset;

            var spawnPosition = new Vector2(spawnX, spawnY);

            var instance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            instance.StartMovement();
        }
        
        private void SetNextSpawnInterval()
        {
            _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
        }
    }
}