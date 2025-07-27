using Player;
using UnityEngine;

namespace Environment.Blocks.BlockTypes
{
    public class BreakableBlock : Block
    {
        [Header("Breakable Settings")]
        [SerializeField] private Vector2Int _secondsBeforeBreak = new(3, 5);
        
        private PlayerController _playerController;

        public async void StartBreaking(PlayerController playerController)
        {
            _playerController = playerController;
            try
            {
                await Awaitable.WaitForSecondsAsync(Random.Range(_secondsBeforeBreak.x, _secondsBeforeBreak.y + 1),
                    destroyCancellationToken);
            }
            catch
            {
                return;
            }
    
            if (_playerController&& _playerController.AttachedBlock == this)
            {
                _playerController.DetachFromBlock();
            }
    
            Destroy(gameObject);
        }
    }
}