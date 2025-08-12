using Core.Audio;
using Money;
using Player;
using Skins;
using UnityEngine;

namespace Environment.Collectibles.Coins
{
    public sealed class CollectableCoin : MonoBehaviour
    {
        [SerializeField] private int _rewardAmount;
        
        [Header("Audio")]
        [SerializeField] private AudioClip _collectSound;
        
        private bool _isCollected;
        private ParticleSystem _collectParticleSystem;

        private void Awake() => _collectParticleSystem = GetComponentInChildren<ParticleSystem>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCore _)) Collect();
        }
        
        private void Collect()
        {
            if (_isCollected) return;
            
            _isCollected = true;
                
            Debug.Log($"Collected coin! Reward: {_rewardAmount}");
            
            var rewardMultiplier = ActiveSkinManager.Instance.CurrentSkin?.RewardMultiplier ?? 1;
            var finalAmount = Mathf.RoundToInt(_rewardAmount * rewardMultiplier);
            
            MoneySystem.Instance.AddMoney(finalAmount);
            
            SoundManager.Instance.PlaySfx(_collectSound);
            _collectParticleSystem.Play();

            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}