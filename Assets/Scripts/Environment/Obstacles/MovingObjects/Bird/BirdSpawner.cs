using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Obstacles.MovingObjects.Bird
{
    public class BirdSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BlockSpawner _blockSpawner;
        [SerializeField] private Bird _birdPrefab;

        [Header("Spawn Settings")]
        [SerializeField] [Range(0, 1)] private float _spawnChanceInGap = 0.75f;
        [SerializeField] private float _minGapSizeForSpawn = 8f;
        [SerializeField] private float _spawnHeightOffset = 12f;
        
        private UnityEngine.Camera _mainCamera;

        private void Awake()
        {
            if (!_playerController || !_blockSpawner || !_birdPrefab)
            {
                enabled = false;
                return;
            }
            
            _playerController.OnJumped += HandlePlayerJumped;
        }

        private void Start() => _mainCamera = UnityEngine.Camera.main;

        private void OnDestroy()
        {
            if (_playerController)
            {
                _playerController.OnJumped -= HandlePlayerJumped;
            }
        }

        private void HandlePlayerJumped(Block jumpedFromBlock)
        {
            if (Random.value > _spawnChanceInGap) return;

            var allBlocks = _blockSpawner.SpawnedBlocks.ToList();
            var currentIndex = allBlocks.IndexOf(jumpedFromBlock);

            if (currentIndex == -1 || currentIndex + 1 >= allBlocks.Count) return;

            var nextBlock = allBlocks[currentIndex + 1];
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
            var spawnY = _mainCamera.transform.position.y - _spawnHeightOffset;
            var spawnPosition = new Vector2(spawnX, spawnY);

            var instance = Instantiate(_birdPrefab, spawnPosition, Quaternion.identity, transform);
            instance.StartMovement();
        }
    }
}