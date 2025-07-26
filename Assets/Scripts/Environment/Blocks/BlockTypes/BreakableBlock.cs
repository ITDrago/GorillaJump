using Player;
using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    public class BreakableBlock : Block
    {
        [Header("Breakable Settings")]
        [SerializeField] private int _secondsBeforeBreak;
        
        private PlayerController _playerController;

        public async void StartBreaking(PlayerController playerController)
        {
            _playerController = playerController;
            await Awaitable.WaitForSecondsAsync(_secondsBeforeBreak, destroyCancellationToken);
    
            if (_playerController&& _playerController.AttachedBlock == this)
            {
                _playerController.DetachFromBlock();
            }
    
            Destroy(gameObject);
        }
    }
}