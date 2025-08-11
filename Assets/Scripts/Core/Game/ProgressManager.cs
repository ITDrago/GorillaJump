using System;
using System.Linq;
using Environment.Blocks;
using Environment.Blocks.BlockTypes;
using Player;
using Skins;
using UnityEngine;

namespace Core.Game
{
    public class ProgressManager : MonoBehaviour
    {
        [Header("Charged Jump Settings")]
        [SerializeField] private int _blocksToUnlockChargedJump = 40;

        [Header("Player References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BlockSpawner _blockSpawner;

        private bool _chargedJumpUnlocked;
        private int _finalBlocksToUnlock;

        public int BlocksPassedCount { get; private set; }
        public bool IsChargedJumpUnlocked => _chargedJumpUnlocked;

        public event Action OnChargedJumpUnlocked;
        public event Action OnBlockPassed;
        
        private void Start() => CalculateFinalUnlockRequirement();

        private void Update() => ProcessPassedBlocks();
        
        private void CalculateFinalUnlockRequirement()
        {
            var multiplier = 1f;
            if (ActiveSkinManager.Instance?.CurrentSkin)
            {
                multiplier = ActiveSkinManager.Instance.CurrentSkin.ChargedJumpRequirementMultiplier;
            }
            
            _finalBlocksToUnlock = Mathf.CeilToInt(_blocksToUnlockChargedJump * multiplier);
        }

        private void ProcessPassedBlocks()
        {
            if (!_blockSpawner || !_blockSpawner.SpawnedBlocks.Any() || !_playerController) return;

            var passedBlocksCountNow = _blockSpawner.SpawnedBlocks
                .Count(b => b && b.transform.position.x < _playerController.transform.position.x);

            if (passedBlocksCountNow > BlocksPassedCount)
            {
                BlocksPassedCount = passedBlocksCountNow;
                GameEvents.BlockPassed();
                OnBlockPassed?.Invoke();
                CheckUnlockables();
            }
        }

        private void CheckUnlockables()
        {
            if (!_chargedJumpUnlocked && BlocksPassedCount >= _finalBlocksToUnlock)
            {
                _chargedJumpUnlocked = true;
                OnChargedJumpUnlocked?.Invoke();
                Debug.Log("Charged Jump Unlocked!");
            }
        }
    }
}