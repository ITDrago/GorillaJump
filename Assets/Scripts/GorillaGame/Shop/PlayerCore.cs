using UnityEngine;

namespace GorillaGame.Shop
{
    [CreateAssetMenu(fileName = "NewSkin", menuName = "Gorilla/Skin Data")]
    public class SkinData : ScriptableObject
    {
        [Header("Common Info")]
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private int _price;
        [SerializeField] private bool _isFree;
        [SerializeField] private string _buffDescription;
        
        [Header("Abilities")]
        [SerializeField] private float _rewardMultiplier = 1f;
        [SerializeField] private float _effectDurationMultiplier = 1f;
        [SerializeField] private float _chargedJumpRequirementMultiplier = 1f;
        [SerializeField] private bool _isImmuneToIceSlide;

        [Header("Customization")]
        [SerializeField] private Gradient _trailGradient;
        
        [Header("Game Objects")]
        [SerializeField] private GameObject _shopDisplayPrefab;
        [SerializeField] private GameObject _inGamePlayerPrefab;

        public int ID => _id;
        public string Name => _name;
        public int Price => _price;
        public bool IsFree => _isFree;
        public string BuffDescription => _buffDescription;
        public float RewardMultiplier => _rewardMultiplier;
        public float EffectDurationMultiplier => _effectDurationMultiplier;
        public float ChargedJumpRequirementMultiplier => _chargedJumpRequirementMultiplier;
        public bool IsImmuneToIceSlide => _isImmuneToIceSlide;
        public Gradient TrailGradient => _trailGradient;
        public GameObject ShopDisplayPrefab => _shopDisplayPrefab;
        public GameObject InGamePlayerPrefab => _inGamePlayerPrefab;
    }
}