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
        [SerializeField] private float _minXOffsetBeyondCameraRight = 5;

        [Header("Safety Checks")]
        [SerializeField] private Vector2 _clearanceBoxSize = new(4, 20);
        [SerializeField] private LayerMask _obstacleLayerMask;

        private UnityEngine.Camera _mainCamera;
        private int _blocksUntilNextSpawn;
        private int _blockCounter;
        private float _totalSpawnChance;
        private RingSpawnPointCalculator _spawnPointCalculator;
        private Vector2 _lastCheckedPosition;

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
                _clearanceBoxSize);

            SetNextSpawnInterval();
        }

        private void OnDestroy()
        {
            if (_blockSpawner) _blockSpawner.OnBlockSpawned -= HandleBlockSpawned;
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
            
            var proposedPosition = (previousBlock.transform.position + newBlock.transform.position) * 0.5f;
            
            _lastCheckedPosition = proposedPosition;

            var cameraWorldRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0, _mainCamera.nearClipPlane)).x;
            if (proposedPosition.x < cameraWorldRight + _minXOffsetBeyondCameraRight)
                return;

            if (!_spawnPointCalculator.IsPositionClear(proposedPosition)) return;

            var ringPrefab = GetRandomRingPrefab();
            if (!ringPrefab) return;
            
            var newRing = Instantiate(ringPrefab, proposedPosition, ringPrefab.transform.localRotation, transform);
            newRing.Initialize();
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
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_lastCheckedPosition, new Vector3(_clearanceBoxSize.x, _clearanceBoxSize.y));
        }
    }
}