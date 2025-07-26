using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Obstacles.Branch
{
    public class BranchSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BlockSpawner _blockSpawner;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private FallingBranch _branchPrefab;

        [Header("Spawn Settings")]
        [SerializeField] [Range(0, 1)] private float _branchSpawnChance = 0.5f;
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);
        [SerializeField] private Vector2Int _spawnDelay = new(1, 3);
        [SerializeField] private float _spawnHeightOffset = 10;

        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private UnityEngine.Camera _mainCamera;

        private void Awake()
        {
            if (!_blockSpawner || !_playerController)
            {
                enabled = false;
                return;
            }
            _playerController.OnLanded += HandlePlayerLanded;
        }

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
            SetNextSpawnInterval();
        }

        private void OnDestroy()
        {
            if (_playerController)
            {
                _playerController.OnLanded -= HandlePlayerLanded;
            }
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
            _blockCounter++;

            if (_blockCounter < _blocksUntilNextSpawn) return;
            
            if (Random.value <= _branchSpawnChance)
            {
                SpawnBranchBetweenBlocks(landedBlock);
            }
            
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private async void SpawnBranchBetweenBlocks(Block currentBlock)
        {
            if (!_branchPrefab || !_mainCamera || !currentBlock) return;

            try
            {
                await Awaitable.WaitForSecondsAsync(Random.Range(_spawnDelay.x, _spawnDelay.y),
                    destroyCancellationToken);
            }
            catch
            {
                //Cancel
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

            var branchInstance = Instantiate(_branchPrefab, spawnPosition, Quaternion.identity, transform);
            branchInstance.StartFalling();
        }
        
        private void SetNextSpawnInterval()
        {
            _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
        }
    }
}