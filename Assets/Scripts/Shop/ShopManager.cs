using System.Collections.Generic;
using DG.Tweening;
using Money;
using Skins.Data;
using UI.Menu.Shop;
using UnityEngine;

namespace Shop
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<SkinData> _skins;

        [Header("Scene References")]
        [SerializeField] private ShopUIManager _uiManager;

        [Header("Display Settings")]
        [SerializeField] private Vector3 _displayPosition = Vector3.zero;
        [SerializeField] private Vector3 _displayRotation = Vector3.zero;
        
        [Header("Animation Settings")]
        [SerializeField] private float _transitionDuration = 0.4f;
        [SerializeField] private float _offscreenDistance = 14;
        
        private List<GameObject> _skinInstances = new();
        private int _currentSkinIndex;
        private MoneyStorage _moneyStorage;
        
        private void Start()
        {
            _moneyStorage = MoneySystem.Instance.MoneyStorage;
            _uiManager.Configure(this);
        }

        public void Initialize()
        {
            if (_skinInstances.Count > 0) return;

            _currentSkinIndex = SkinPersistence.GetSelectedSkinID();
            SpawnAllSkins();
            UpdateCurrentSkinUI();
        }

        public void Cleanup()
        {
            foreach (var instance in _skinInstances)
            {
                if(instance) Destroy(instance);
            }
            _skinInstances.Clear();
        }

        public void NextSkin() => AnimateAndSwitch(1);

        public void PreviousSkin() => AnimateAndSwitch(-1);

        public void BuySkin()
        {
            var currentSkin = _skins[_currentSkinIndex];
            var isAcquired = currentSkin.IsFree || _moneyStorage.TrySpend(currentSkin.Price);
            
            if (isAcquired)
            {
                SkinPersistence.SavePurchasedSkin(currentSkin.ID);
                UpdateCurrentSkinUI();
            }
        }

        public void SelectCurrentSkin()
        {
            var currentSkin = _skins[_currentSkinIndex];
            SkinPersistence.SelectSkin(currentSkin.ID);
            UpdateCurrentSkinUI();
        }

        private void SpawnAllSkins()
        {
            if (_skins == null || _skins.Count == 0) return;
            
            var offscreenPosition = _displayPosition + (Vector3.right * _offscreenDistance * 2);

            for (var i = 0; i < _skins.Count; i++)
            {
                var skinPrefab = _skins[i].ShopDisplayPrefab;
                if (!skinPrefab) continue;
                
                var position = (i == _currentSkinIndex) ? _displayPosition : offscreenPosition;
                var instance = Instantiate(skinPrefab, position, Quaternion.Euler(_displayRotation));
                _skinInstances.Add(instance);
            }
        }
        
        private void AnimateAndSwitch(int direction)
        {
            if (_skinInstances.Count < 2) return;

            var oldIndex = _currentSkinIndex;
            _currentSkinIndex = (_currentSkinIndex + direction + _skins.Count) % _skins.Count;

            var oldSkinInstance = _skinInstances[oldIndex];
            var newSkinInstance = _skinInstances[_currentSkinIndex];

            var oldSkinTargetPosition = _displayPosition - (Vector3.right * _offscreenDistance * direction);
            var newSkinStartPosition = _displayPosition + (Vector3.right * _offscreenDistance * direction);
            
            oldSkinInstance.transform.DOMove(oldSkinTargetPosition, _transitionDuration).SetEase(Ease.OutSine);
            
            newSkinInstance.transform.position = newSkinStartPosition;
            newSkinInstance.transform.DOMove(_displayPosition, _transitionDuration).SetEase(Ease.OutSine);
            
            UpdateCurrentSkinUI();
        }

        private void UpdateCurrentSkinUI()
        {
            if (_currentSkinIndex >= _skins.Count) return;
            
            var currentSkinData = _skins[_currentSkinIndex];
            var isPurchased = SkinPersistence.IsSkinPurchased(currentSkinData.ID);
            var isSelected = SkinPersistence.GetSelectedSkinID() == currentSkinData.ID;

            _uiManager.UpdateSkinInfo(currentSkinData, isPurchased, isSelected);
        }
    }
}