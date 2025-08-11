using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Shop;
using Skins.Data;

namespace UI.Menu
{
    public class MenuCharacterDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<SkinData> _allSkins;

        [Header("Spawn Settings")]
        [SerializeField] private Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] private Vector3 _spawnRotation = Vector3.zero;

        private GameObject _spawnedCharacterInstance;
        private bool _isCharacterVisible = true;

        private void OnEnable() => SkinPersistence.OnSkinSelected += HandleSkinSelected;

        private void OnDisable() => SkinPersistence.OnSkinSelected -= HandleSkinSelected;

        private void Start() => SpawnSelectedSkin();

        public void SetCharacterVisible(bool isVisible)
        {
            _isCharacterVisible = isVisible;
            if (_spawnedCharacterInstance) _spawnedCharacterInstance.SetActive(_isCharacterVisible);
        }

        private void HandleSkinSelected(int skinID) => SpawnSelectedSkin();

        private void SpawnSelectedSkin()
        {
            var selectedSkinID = SkinPersistence.GetSelectedSkinID();
            var skinToSpawnData = _allSkins.FirstOrDefault(s => s.ID == selectedSkinID);

            if (!skinToSpawnData)
                return;

            var prefabToSpawn = skinToSpawnData.ShopDisplayPrefab;

            if (!prefabToSpawn)
                return;
            
            if (_spawnedCharacterInstance) Destroy(_spawnedCharacterInstance);
            
            _spawnedCharacterInstance = Instantiate(prefabToSpawn, _spawnPosition, Quaternion.Euler(_spawnRotation));
            _spawnedCharacterInstance.SetActive(_isCharacterVisible);
        }
    }
}