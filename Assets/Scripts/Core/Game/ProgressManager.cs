using System;
using System.Collections.Generic;
using Core.Data;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Player;
using Player.Health;
using UnityEngine;

namespace Core.Game
{
    public class ProgressManager : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BlockSpawner _blockSpawner;
        [SerializeField] private PlayerHealth _playerHealth;
        
        private readonly HashSet<Block> _countedBlocks = new();

        public int BlocksPassedCount { get; private set; }
        public event Action OnBlockPassed;
        private void OnEnable()
        {
            if (_blockSpawner) _blockSpawner.OnBlockRemoved += HandleBlockRemoved;
            if (_playerHealth) _playerHealth.OnDied += HandleGameEnd;
        }

        private void OnDisable()
        {
            if (_blockSpawner) _blockSpawner.OnBlockRemoved -= HandleBlockRemoved;
            if (_playerHealth) _playerHealth.OnDied -= HandleGameEnd;
        }

        private void Update() => ProcessPassedBlocks();

        private void ProcessPassedBlocks()
        {
            if (!_blockSpawner || !_playerController) return;

            foreach (var block in _blockSpawner.SpawnedBlocks)
            {
                if (block && !_countedBlocks.Contains(block))
                {
                    if (block.transform.position.x - 1 < _playerController.transform.position.x)
                    {
                        BlocksPassedCount++;
                        _countedBlocks.Add(block);
                        GameEvents.BlockPassed();
                        OnBlockPassed?.Invoke();
                    }
                }
            }
        }
        
        private void HandleBlockRemoved(Block block)
        {
            if (block) _countedBlocks.Remove(block);
        }
        
        private void HandleGameEnd() => ScoreSaver.SaveScore(BlocksPassedCount);
    }
}