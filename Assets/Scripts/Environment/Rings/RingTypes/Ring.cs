using Core;
using Core.Game;
using Player;
using UnityEngine;

namespace Environment.Rings.RingTypes
{
    public class Ring : MonoBehaviour
    {
        [SerializeField] private int _defaultReward = 1;
        [SerializeField] private float _scaleMultiplier = 0.1f;
        
        private Vector3 _originalScale;
        private int _rewardAmount;
        protected UnityEngine.Camera MainCamera { get; private set; }

        protected virtual void Awake()
        {
            _originalScale = transform.localScale;
            MainCamera = UnityEngine.Camera.main;
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
            if (other.TryGetComponent(out PlayerCore playerCore)) Collect();
        }
        
        private void Collect()
        {
            Debug.Log($"Collected ring! Reward: {_rewardAmount}");
            GameEvents.RingCollected(_rewardAmount);
        }
    }
}