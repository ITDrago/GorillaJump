using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Player.Handlers
{
    public class PlayerLandingHandler : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            if (_playerController) _playerController.OnLanded += HandleLanding;
        }

        private void OnDestroy()
        {
            if (_playerController) _playerController.OnLanded -= HandleLanding;
        }

        private void HandleLanding(Block landedBlock)
        {
            switch (landedBlock)
            {
                case BreakableBlock breakableBlock:
                    breakableBlock.StartBreaking(_playerController);
                    break;
            }
        }
    }
}