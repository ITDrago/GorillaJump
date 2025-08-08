using Core.Audio;
using Environment.Blocks.BlockTypes;
using Player.Health;
using UnityEngine;

namespace Player.Audio
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SoundManager _soundManager;

        [Header("Audio Clips")] 
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _landSound;
        [SerializeField] private AudioClip _deathSound;
        
        private PlayerController _playerController;
        private PlayerHealth _playerHealth;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerHealth = _playerController.GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            _playerController.OnJumped += HandlePlayerJumped;
            _playerController.OnLanded += HandlePlayerLanded;
            _playerHealth.OnDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            _playerController.OnJumped -= HandlePlayerJumped;
            _playerController.OnLanded -= HandlePlayerLanded;
            _playerHealth.OnDied -= HandlePlayerDied;
        }

        private void HandlePlayerJumped(Block jumpedFromBlock)
        {
            if (_jumpSound && _soundManager) _soundManager.PlaySfx(_jumpSound);
        }

        private void HandlePlayerLanded(Block landedOnBlock, Vector2 stickPoint)
        {
            if (_landSound && _soundManager) _soundManager.PlaySfx(_landSound);
        }

        private void HandlePlayerDied()
        {
            if (_deathSound && _soundManager)
            {
                _soundManager.AdjustMusicVolume(0.2f);
                _soundManager.PlaySfx(_deathSound);
            }
        }
    }
}