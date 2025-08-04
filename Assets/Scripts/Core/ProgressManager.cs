using Player;
using UnityEngine;
using System;
using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;

namespace Core
{
    public class ProgressManager : MonoBehaviour
    {
        [Header("Charged Jump Settings")] 
        [SerializeField] private int _blocksToUnlockChargedJump = 10;

        [Header("Player References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BlockSpawner _blockSpawner;

        private Block _lastPassedBlock;
        private bool _chargedJumpUnlocked;

        public int BlocksPassedCount { get; private set; }
        public bool IsChargedJumpUnlocked => _chargedJumpUnlocked;

        public event Action OnChargedJumpUnlocked;
        public event Action OnBlockPassed;

        private void OnEnable()
        {
            if (_playerController) _playerController.OnLanded += HandlePlayerLanded;
        }

        private void OnDisable()
        {
            if (_playerController) _playerController.OnLanded -= HandlePlayerLanded;
        }

        private void Update()
        {
            ProcessPassedBlocks();
        }

        private void HandlePlayerLanded(Block landedBlock, Vector2 stickPoint)
        {
        }

        private void ProcessPassedBlocks()
        {
            if (!_blockSpawner || _blockSpawner.SpawnedBlocks == null ||
                _blockSpawner.SpawnedBlocks.Count == 0 || !_playerController) return;

            var sortedBlocks = _blockSpawner.SpawnedBlocks.OrderBy(b => b.transform.position.x).ToList();

            var startIndex = -1;
            if (_lastPassedBlock) startIndex = sortedBlocks.IndexOf(_lastPassedBlock);
            startIndex = Mathf.Max(0, startIndex + 1);


            for (var i = startIndex; i < sortedBlocks.Count; i++)
            {
                var currentBlock = sortedBlocks[i];
                var blockCollider = currentBlock.GetComponent<Collider2D>();

                if (!blockCollider)
                    continue;

                var blockPassThresholdX = currentBlock.transform.position.x;

                if (_playerController.transform.position.x > blockPassThresholdX)
                {
                    BlocksPassedCount++;
                    _lastPassedBlock = currentBlock;
                    GameEvents.BlockPassed();
                    OnBlockPassed?.Invoke();
                    CheckUnlockables();
                }
                else
                {
                    break;
                }
            }
        }

        private void CheckUnlockables()
        {
            if (!_chargedJumpUnlocked && BlocksPassedCount >= _blocksToUnlockChargedJump)
            {
                _chargedJumpUnlocked = true;
                OnChargedJumpUnlocked?.Invoke();
                Debug.Log("Charged Jump Unlocked!");
            }
        }
    }
}