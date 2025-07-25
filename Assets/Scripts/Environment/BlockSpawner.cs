using System.Collections.Generic;
using Difficulty;
using Player;
using UnityEngine;

namespace Environment
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private Block[] _blockPrefabs;
        [SerializeField] private DifficultyConfig _difficultyConfig;
        [SerializeField] private PlayerController _playerController;

        private List<Block> _spawnedBlocks = new();
        private int _blocksSpawned;
        private float _nextSpawnX;

        private void Start()
        {
            if (_playerController.StartBlock != null)
            {
                _spawnedBlocks.Add(_playerController.StartBlock);
                _nextSpawnX = _playerController.StartBlock.transform.position.x;
            }

            SpawnNextBlocks(4);
        }

        private void Update()
        {
            if (ShouldSpawnNewBlock()) SpawnBlock();
        }

        private bool ShouldSpawnNewBlock()
        {
            return _playerController.transform.position.x > _nextSpawnX - 15f;
        }

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

            if (_spawnedBlocks.Count > 10) RemoveOldBlocks();
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
            if (_spawnedBlocks.Count > 10)
                for (var i = 0; i < _spawnedBlocks.Count; i++)
                    if (_spawnedBlocks[i] != _playerController.StartBlock)
                    {
                        Destroy(_spawnedBlocks[i].gameObject);
                        _spawnedBlocks.RemoveAt(i);
                        break;
                    }
        }
    }
}