using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private string _conditionBoolName = "isFlying";
        
        private PlayerController _playerController;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();

            if (_playerController)
            {
                _playerController.OnJumped += SwitchIsFlyingTrue;
                _playerController.OnLanded += SwitchIsFlyingFalse;
            }
        }

        private void OnDisable()
        {
            if (_playerController)
            {
                _playerController.OnJumped -= SwitchIsFlyingTrue;
                _playerController.OnLanded -= SwitchIsFlyingFalse;
            }
        }
        
        private void SwitchIsFlying(bool condition) => _playerAnimator.SetBool(_conditionBoolName, condition);
        
        private void SwitchIsFlyingTrue(Block block) => SwitchIsFlying(true);
        private void SwitchIsFlyingFalse(Block block, Vector2 position) => SwitchIsFlying(false);
    }
}