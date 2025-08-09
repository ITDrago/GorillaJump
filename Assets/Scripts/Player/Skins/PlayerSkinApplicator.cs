using System.Collections.Generic;
using System.Linq;
using Shop;
using Skins;
using Skins.Data;
using UnityEngine;

namespace Player.Skins
{
    public class PlayerSkinApplicator : MonoBehaviour
    {
        [System.Serializable]
        public struct SkinMapping
        {
            public int ID;
            public GameObject SkinObject;
        }

        [SerializeField] private List<SkinData> _allSkins;
        [SerializeField] private List<SkinMapping> _skinMappings;

        private void Awake()
        {
            var selectedSkinID = SkinPersistence.GetSelectedSkinID();
            ApplySkin(selectedSkinID);
        }

        private void ApplySkin(int skinID)
        {
            var skinData = _allSkins.FirstOrDefault(s => s.ID == skinID);
            if (!skinData) return;
            
            if (ActiveSkinManager.Instance) ActiveSkinManager.Instance.SetActiveSkin(skinData);

            foreach (var mapping in _skinMappings)
            {
                if (!mapping.SkinObject) continue;

                var isActive = mapping.ID == skinID;
                mapping.SkinObject.SetActive(isActive);

                if (isActive)
                {
                    var trail = (TrailRenderer)FindFirstObjectByType(typeof(TrailRenderer));
                    if (trail && skinData.TrailGradient != null)
                    {
                        trail.colorGradient = skinData.TrailGradient;
                    }
                }
            }
        }
    }
}