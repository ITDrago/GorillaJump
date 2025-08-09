using Core.Audio;
using Core.Game;
using Money;
using Player;
using UnityEngine;

namespace Environment.Rings.RingTypes
{
    public class Ring : MonoBehaviour
    {
        [SerializeField] private int _defaultReward = 1;
        [SerializeField] private float _scaleMultiplier = 0.1f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _collectSound;
        
        private Vector3 _originalScale;
        private int _rewardAmount;
        private bool _isCollected;
        
        private ParticleSystem _collectParticleSystem;
        private Coin _coin;
        
        protected UnityEngine.Camera MainCamera { get; private set; }

        protected virtual void Awake()
        {
            MainCamera = UnityEngine.Camera.main;
            _collectParticleSystem = GetComponentInChildren<ParticleSystem>();
            _coin = GetComponentInChildren<Coin>();
            _originalScale = transform.localScale;
            _rewardAmount = _defaultReward;
        }

        public virtual void Initialize(int reward)
        {
            _rewardAmount = reward;
            UpdateScale();
        }

        private void UpdateScale()
        {
            var scaleFactor = 1 + _rewardAmount * _scaleMultiplier;
            transform.localScale = _originalScale * scaleFactor;
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCore _)) Collect();
        }
        
        private void Collect()
        {
            if (_isCollected) return;
            
            Debug.Log($"Collected ring! Reward: {_rewardAmount}");
            MoneySystem.Instance.AddMoney(_rewardAmount);
            
            _isCollected = true;
            GameEvents.RingCollected(_rewardAmount);
            
            _coin.gameObject.SetActive(false);
            
            SoundManager.Instance.PlaySfx(_collectSound);
            _collectParticleSystem.Play();
        }
    }
}