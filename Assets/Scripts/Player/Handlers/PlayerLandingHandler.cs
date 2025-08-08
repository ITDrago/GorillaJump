using Environment.Blocks.BlockTypes;
using Skins;
using UnityEngine;

namespace Player.Handlers
{
    public class PlayerLandingHandler : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Awake() => _playerController = GetComponent<PlayerController>();

        private void OnEnable()
        {
            if (_playerController) _playerController.OnLanded += HandleLanding;
        }

        private void OnDisable()
        {
            if (_playerController) _playerController.OnLanded -= HandleLanding;
        }

        private void HandleLanding(Block landedBlock, Vector2 stickPoint)
        {
            switch (landedBlock)
            {
                case IceBlock iceBlock:
                    var isImmune = ActiveSkinManager.Instance.CurrentSkin?.IsImmuneToIceSlide ?? false;
                    if (isImmune) _playerController.InitiateSwing(stickPoint);
                    else _playerController.InitiateSlide(iceBlock, stickPoint);
                    break;
                
                case BreakableBlock breakableBlock:
                    var preventsBreaking = ActiveSkinManager.Instance.CurrentSkin?.PreventsBlockBreaking ?? false;
                    if (!preventsBreaking) breakableBlock.StartBreaking(_playerController);
                    _playerController.InitiateSwing(stickPoint);
                    break;

                default:
                    _playerController.InitiateSwing(stickPoint);
                    break;
            }
        }
    }
}