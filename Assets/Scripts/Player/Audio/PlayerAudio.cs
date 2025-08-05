using Core.Audio;
using Environment.Blocks.BlockTypes;
using UnityEngine;

namespace Player.Audio
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SoundManager _soundManager;

        [Header("Audio Clips")] 
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _landSound;
        
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            _playerController.OnJumped += HandlePlayerJumped;
            _playerController.OnLanded += HandlePlayerLanded;
        }

        private void OnDisable()
        {
            _playerController.OnJumped -= HandlePlayerJumped;
            _playerController.OnLanded -= HandlePlayerLanded;
        }

        private void HandlePlayerJumped(Block jumpedFromBlock)
        {
            if (_jumpSound && _soundManager) _soundManager.PlaySfx(_jumpSound);
        }

        private void HandlePlayerLanded(Block landedOnBlock, Vector2 stickPoint)
        {
            if (_landSound && _soundManager) _soundManager.PlaySfx(_landSound);
        }
    }
}