using System.Collections.Generic;
using Difficulty;
using Environment.Blocks.BlockTypes;
using Player;
using UnityEngine;

namespace Environment.Blocks
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private Block[] _blockPrefabs;
        [SerializeField] private Block[] _specialBlockPrefabs;
        [SerializeField] private DifficultyConfig _difficultyConfig;
        [SerializeField] private PlayerController _playerController;
        
        [Header("Special Blocks Settings")]
        [SerializeField] [Range(0f, 1f)] private float _specialBlockProbability = 0.2f;
        [SerializeField] private int _minBlocksBetweenSpecial = 4;

        [Header("Spawn Blocks Settings")]
        [SerializeField] private int _maxBlocksCount = 7;

        private List<Block> _spawnedBlocks = new();
        private int _blocksSpawned;
        private float _nextSpawnX;
        private int _blocksSinceLastSpecial;

        private void Start()
        {
            if (_playerController.StartBlock != null)
            {
                _spawnedBlocks.Add(_playerController.StartBlock);
                _nextSpawnX = _playerController.StartBlock.transform.position.x;
            }
            
            _blocksSinceLastSpecial = _minBlocksBetweenSpecial;
            
            SpawnNextBlocks(4);
        }

        private void Update()
        {
            if (ShouldSpawnNewBlock()) SpawnBlock();
        }

        private bool ShouldSpawnNewBlock() => _playerController.transform.position.x > _nextSpawnX - 15f;

        private void SpawnNextBlocks(int count)
        {
            for (var i = 0; i < count; i++) SpawnBlock();
        }

        private void SpawnBlock()
        {
            var difficultyLevel = GetCurrentDifficulty();
            var spawnPosition = CalculateSpawnPosition(difficultyLevel);
            var blockPrefab = SelectBlockPrefab(difficultyLevel);

            var newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity, transform);
            _spawnedBlocks.Add(newBlock);

            _nextSpawnX = spawnPosition.x;
            _blocksSpawned++;

            if (_spawnedBlocks.Count > _maxBlocksCount) RemoveOldBlocks();
        }

        private Vector2 CalculateSpawnPosition(DifficultyLevel difficulty)
        {
            if (_spawnedBlocks.Count == 0)
                return new Vector2(0, 0);

            var lastBlock = _spawnedBlocks[^1];
            var lastX = lastBlock.transform.position.x;
            var lastWidth = lastBlock.GetComponent<Collider2D>().bounds.size.x;

            var x = lastX + lastWidth + difficulty.BlockSpacing;
            var y = Random.Range(difficulty.MinHeight, difficulty.MaxHeight);

            if (Random.value < difficulty.GapProbability)
                x += difficulty.BlockSpacing * 2;

            return new Vector2(x, y);
        }

        private Block SelectBlockPrefab(DifficultyLevel difficulty)
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
                if (_blocksSpawned >= level.StartBlockIndex)
                    return level;
            return _difficultyConfig.Levels[^1];
        }

        private void RemoveOldBlocks()
        {
            if (_spawnedBlocks.Count > _maxBlocksCount)
            {
                for (var i = 0; i < _spawnedBlocks.Count; i++)
                {
                    var block = _spawnedBlocks[i];
            
                    if (block && block != _playerController.StartBlock)
                    {
                        Destroy(block.gameObject);
                        _spawnedBlocks.RemoveAt(i);
                        break;
                    }
                    else if (!block)
                    {
                        _spawnedBlocks.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}