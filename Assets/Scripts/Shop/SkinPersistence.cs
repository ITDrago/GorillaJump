using System;
using System.Linq;
using UnityEngine;

namespace Shop
{
    public static class SkinPersistence
    {
        public static event Action<int> OnSkinSelected;
        
        private const string PURCHASED_SKINS_KEY = "PurchasedSkins";
        private const string SELECTED_SKIN_KEY = "SelectedSkinID";

        public static void SavePurchasedSkin(int skinID)
        {
            var purchased = GetPurchasedSkins();
            if (purchased.Contains(skinID)) return;

            var newPurchased = string.Join(",", purchased) + "," + skinID;
            PlayerPrefs.SetString(PURCHASED_SKINS_KEY, newPurchased);
        }

        public static int[] GetPurchasedSkins()
        {
            var savedData = PlayerPrefs.GetString(PURCHASED_SKINS_KEY);
            if (string.IsNullOrEmpty(savedData)) return new int[] { 0 };

            return savedData.Split(',').Select(int.Parse).ToArray();
        }

        public static bool IsSkinPurchased(int skinID) => GetPurchasedSkins().Contains(skinID);

        public static void SelectSkin(int skinID)
        {
            PlayerPrefs.SetInt(SELECTED_SKIN_KEY, skinID);
            OnSkinSelected?.Invoke(skinID);
        }

        public static int GetSelectedSkinID() => PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);
    }
}