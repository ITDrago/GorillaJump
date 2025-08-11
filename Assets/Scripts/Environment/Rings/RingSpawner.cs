using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Environment.Rings.RingTypes;
using UnityEngine;

namespace Environment.Rings
{
    [System.Serializable]
    public class RingType
    {
        public Ring Prefab;
        [Range(0, 1)] public float SpawnChance;
    }

    public class RingSpawner : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private BlockSpawner _blockSpawner;

        [Header("Ring Types")] 
        [SerializeField] private RingType[] _ringTypes;

        [Header("Spawn Timing")] 
        [SerializeField] private Vector2Int _blocksBetweenSpawnsRange = new(3, 5);

        [Header("Positioning Settings")] 
        [SerializeField] private float _minDistanceFromBlock = 3;
        [SerializeField] private float _minXOffsetBeyondCameraRight = 5;
        [SerializeField] private Vector2 _spawnYViewportRange = new(0.2f, 0.8f);

        [Header("Safety Checks")]
        [SerializeField] private float _clearCheckRadius = 3;
        [SerializeField] private LayerMask _obstacleLayerMask;

        private UnityEngine.Camera _mainCamera;
        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private float _totalSpawnChance;
        private RingSpawnPointCalculator _spawnPointCalculator;

        private void Awake()
        {
            if (!_blockSpawner || _ringTypes.Length == 0)
            {
                enabled = false;
                return;
            }

            _blockSpawner.OnBlockSpawned += HandleBlockSpawned;
            _totalSpawnChance = _ringTypes.Sum(rt => rt.SpawnChance);
        }

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
            _spawnPointCalculator = new RingSpawnPointCalculator(
                _obstacleLayerMask,
                _clearCheckRadius,
                _minDistanceFromBlock);

            SetNextSpawnInterval();
        }

        private void OnDestroy()
        {
            if (_blockSpawner != null) _blockSpawner.OnBlockSpawned -= HandleBlockSpawned;
        }

        private void HandleBlockSpawned(Block newBlock)
        {
            _blockCounter++;
            if (_blockCounter < _blocksUntilNextSpawn) return;

            TrySpawnRing(newBlock);
            _blockCounter = 0;
            SetNextSpawnInterval();
        }

        private void TrySpawnRing(Block newBlock)
        {
            if (!TryGetPreviousBlock(newBlock, out var previousBlock)) return;

            var midpointX = (previousBlock.transform.position.x + newBlock.transform.position.x) * 0.5f;
            var cameraWorldRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0, _mainCamera.nearClipPlane)).x;

            if (midpointX < cameraWorldRight + _minXOffsetBeyondCameraRight)
                return;

            var spawnY = Random.Range(
                _mainCamera.ViewportToWorldPoint(new Vector3(0, _spawnYViewportRange.x, _mainCamera.nearClipPlane)).y,
                _mainCamera.ViewportToWorldPoint(new Vector3(0, _spawnYViewportRange.y, _mainCamera.nearClipPlane)).y
            );

            var proposedPosition = new Vector2(midpointX, spawnY);

            if (!_spawnPointCalculator.TryCalculateSpawnPosition(previousBlock, newBlock, proposedPosition,
                    out var spawnPosition)) return;

            var ringPrefab = GetRandomRingPrefab();
            if (!ringPrefab) return;

            var newRing = Instantiate(ringPrefab, spawnPosition,ringPrefab.transform.localRotation, transform);
            newRing.Initialize(newRing.GetRewardAmount);
        }

        private bool TryGetPreviousBlock(Block currentBlock, out Block previousBlock)
        {
            previousBlock = null;
            var blocks = _blockSpawner.SpawnedBlocks;
            var currentIndex = blocks.ToList().IndexOf(currentBlock);

            if (currentIndex > 0)
            {
                previousBlock = blocks[currentIndex - 1];
                return true;
            }

            return false;
        }

        private Ring GetRandomRingPrefab()
        {
            if (_totalSpawnChance <= 0) return _ringTypes.FirstOrDefault()?.Prefab;

            var randomValue = Random.Range(0, _totalSpawnChance);
            var cumulativeChance = 0f;

            foreach (var ringType in _ringTypes)
            {
                cumulativeChance += ringType.SpawnChance;
                if (randomValue <= cumulativeChance) return ringType.Prefab;
            }

            return _ringTypes.LastOrDefault()?.Prefab;
        }

        private void SetNextSpawnInterval() => _blocksUntilNextSpawn = Random.Range(_blocksBetweenSpawnsRange.x, _blocksBetweenSpawnsRange.y + 1);
    }
}