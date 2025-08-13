using System;
using System.Collections.Generic;
using System.Linq;
using Difficulty;
using Environment.Blocks.BlockTypes;
using Environment.Obstacles.Spikes;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.Blocks
{
    [Serializable]
    public class SpikeSpawnConfig
    {
        [SerializeField] private int _spikeCount;
        [SerializeField] [Range(0, 1)] private float _spawnChance;
        
        public int SpikeCount => _spikeCount;
        public float SpawnChance => _spawnChance;
    }
    
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private Block[] _blockPrefabs;
        [SerializeField] private Block[] _specialBlockPrefabs;
        [SerializeField] private DifficultyConfig _difficultyConfig;
        [SerializeField] private PlayerController _playerController;
        
        [Header("Special Blocks Settings")]
        [SerializeField] [Range(0, 1)] private float _specialBlockProbability = 0.2f;
        [SerializeField] private int _minBlocksBetweenSpecial = 4;

        [Header("Spawn Blocks Settings")]
        [SerializeField] private int _maxBlocksCount = 7;
        
        [Header("Spike Settings")]
        [SerializeField] private bool _shouldSpawnSpike;
        [SerializeField] private GameObject _spikePrefab;
        [SerializeField] [Range(0, 1)] private float _chanceToAddSpikes = 0.5f;
        [SerializeField] private SpikeSpawnConfig[] _spikeConfigs;

        private readonly List<Block> _spawnedBlocks = new();
        private int _blocksSpawned;
        private float _nextSpawnX;
        private int _blocksSinceLastSpecial;
        
        public IReadOnlyList<Block> SpawnedBlocks => _spawnedBlocks;
        
        public event Action<Block> OnBlockSpawned;
        public event Action<Block> OnBlockRemoved;

        private void Start()
        {
            if (_playerController.StartBlock)
            {
                _spawnedBlocks.Add(_playerController.StartBlock);
                _nextSpawnX = _playerController.StartBlock.transform.position.x;
            }
            
            _blocksSinceLastSpecial = _minBlocksBetweenSpecial;
            
            SpawnNextBlocks(4);
            
            _spikeConfigs = _spikeConfigs.OrderByDescending(config => config.SpikeCount).ToArray();
        }

        private void Update()
        {
            if (ShouldSpawnNewBlock())
            {
                SpawnBlock();
            }
        }

        private bool ShouldSpawnNewBlock() => _playerController.transform.position.x > _nextSpawnX - 15;

        private void SpawnNextBlocks(int count)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnBlock();
            }
        }

        private void SpawnBlock()
        {
            var difficultyLevel = GetCurrentDifficulty();
            var spawnPosition = CalculateSpawnPosition(difficultyLevel);
            var blockPrefab = SelectBlockPrefab(difficultyLevel);

            var newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity, transform);
            _spawnedBlocks.Add(newBlock);
            
            TryPlaceSpikes(newBlock);

            _nextSpawnX = spawnPosition.x;
            _blocksSpawned++;

            if (_spawnedBlocks.Count > _maxBlocksCount)
            {
                RemoveOldBlocks();
            }
            
            OnBlockSpawned?.Invoke(newBlock);
        }

        private Vector2 CalculateSpawnPosition(DifficultyLevel difficulty)
        {
            if (_spawnedBlocks.Count == 0) return Vector2.zero;

            var lastBlock = _spawnedBlocks[^1];
            var lastX = lastBlock.transform.position.x;
            var lastWidth = lastBlock.GetComponent<Collider2D>().bounds.size.x;

            var x = lastX + lastWidth + difficulty.BlockSpacing;
            var y = Random.Range(difficulty.MinHeight, difficulty.MaxHeight);

            if (Random.value < difficulty.GapProbability)
            {
                x += difficulty.BlockSpacing * 2;
            }

            return new Vector2(x, y);
        }

        private Block SelectBlockPrefab(DifficultyLevel _)
        {
            if (_specialBlockPrefabs != null && 
                _blocksSinceLastSpecial >= _minBlocksBetweenSpecial && 
                Random.value < _specialBlockProbability)
            {
                _blocksSinceLastSpecial = 0;
                return _specialBlockPrefabs[Random.Range(0, _specialBlockPrefabs.Length)];
            }
            
            _blocksSinceLastSpecial++;
            return _blockPrefabs[Random.Range(0, _blockPrefabs.Length)];
        }

        private DifficultyLevel GetCurrentDifficulty()
        {
            foreach (var level in _difficultyConfig.Levels)
            {
                if (_blocksSpawned >= level.StartBlockIndex)
                    return level;
            }
            return _difficultyConfig.Levels[^1];
        }

        private void RemoveOldBlocks()
        {
            if (_spawnedBlocks.Count <= _maxBlocksCount) return;
            
            for (var i = 0; i < _spawnedBlocks.Count; i++)
            {
                var block = _spawnedBlocks[i];
        
                if (block && block != _playerController.StartBlock)
                {
                    OnBlockRemoved?.Invoke(block);
                    Destroy(block.gameObject);
                    _spawnedBlocks.RemoveAt(i);
                    return;
                }

                if (!block)
                {
                    _spawnedBlocks.RemoveAt(i);
                    return;
                }
            }
        }
        
        private void TryPlaceSpikes(Block block)
        {
            if (!block.TryGetComponent<SpikePlacer>(out var spikePlacer) || !_shouldSpawnSpike) return;
            
            if (Random.value > _chanceToAddSpikes) return;

            foreach (var config in _spikeConfigs)
            {
                if (Random.value <= config.SpawnChance)
                {
                    spikePlacer.PlaceSpikes(_spikePrefab, config.SpikeCount);
                    return;
                }
            }
        }
    }
}