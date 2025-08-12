using UnityEngine;
using UnityEngine.Localization;

namespace Skins.Data
{
    [CreateAssetMenu(fileName = "NewSkin", menuName = "Gorilla/Skin Data")]
    public class SkinData : ScriptableObject
    {
        [Header("Common Info")]
        [SerializeField] private int _id;
        [SerializeField] private LocalizedString _name;
        [SerializeField] private int _price;
        [SerializeField] private bool _isFree;
        [SerializeField] private LocalizedString _buffDescription;
        
        [Header("Abilities")]
        [SerializeField] private float _rewardMultiplier = 1;
        [SerializeField] private float _effectDurationMultiplier = 1;
        [SerializeField] private bool _isImmuneToIceSlide;
        [SerializeField] private bool _preventsBlockBreaking;
        [SerializeField] private float _stickSpawnChanceMultiplier = 1;

        [Header("Customization")]
        [SerializeField] private Gradient _trailGradient;
        
        [Header("Game Objects")]
        [SerializeField] private GameObject _shopDisplayPrefab;
        [SerializeField] private GameObject _inGamePlayerPrefab;

        public int ID => _id;
        public LocalizedString Name => _name;
        public int Price => _price;
        public bool IsFree => _isFree;
        public LocalizedString BuffDescription => _buffDescription;
        public float RewardMultiplier => _rewardMultiplier;
        public float EffectDurationMultiplier => _effectDurationMultiplier;
        public bool IsImmuneToIceSlide => _isImmuneToIceSlide;
        public bool PreventsBlockBreaking => _preventsBlockBreaking;
        public float StickSpawnChanceMultiplier => _stickSpawnChanceMultiplier;
        public Gradient TrailGradient => _trailGradient;
        public GameObject ShopDisplayPrefab => _shopDisplayPrefab;
        public GameObject InGamePlayerPrefab => _inGamePlayerPrefab;
    }
}