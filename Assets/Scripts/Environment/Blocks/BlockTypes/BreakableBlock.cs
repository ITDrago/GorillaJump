using Player;
using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    public class BreakableBlock : Block
    {
        [Header("Breakable Settings")]
        [SerializeField] private int _maxSecondsBeforeBreak;
        [SerializeField] private int _minSecondsBeforeBreak;
        
        private PlayerController _playerController;

        public async void StartBreaking(PlayerController playerController)
        {
            _playerController = playerController;
            await Awaitable.WaitForSecondsAsync(Random.Range(_minSecondsBeforeBreak, _maxSecondsBeforeBreak + 1), destroyCancellationToken);
    
            if (_playerController&& _playerController.AttachedBlock == this)
            {
                _playerController.DetachFromBlock();
            }
    
            Destroy(gameObject);
        }
    }
}