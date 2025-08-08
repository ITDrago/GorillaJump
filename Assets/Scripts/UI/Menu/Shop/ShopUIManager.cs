using Shop;
using Skins.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Shop
{
    public class ShopUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text _skinNameText;
        [SerializeField] private Text _buffDescriptionText;
        [SerializeField] private Text _priceText;
        
        [Header("Buttons")]
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Text _selectButtonText;

        public void Configure(ShopManager shopManager)
        {
            _buyButton.onClick.AddListener(shopManager.BuySkin);
            _selectButton.onClick.AddListener(shopManager.SelectCurrentSkin);
        }

        public void UpdateSkinInfo(SkinData skin, bool isPurchased, bool isSelected)
        {
            _skinNameText.text = skin.Name;
            _buffDescriptionText.text = skin.BuffDescription;

            if (skin.IsFree) _priceText.text = "Free";
            else _priceText.text = skin.Price.ToString();

            if (isPurchased)
            {
                _buyButton.gameObject.SetActive(false);
                _selectButton.gameObject.SetActive(true);
                
                _selectButton.interactable = !isSelected;
                _selectButtonText.text = isSelected ? "Selected" : "Select";
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _selectButton.gameObject.SetActive(false);
                var isAffordable = global::Money.MoneySystem.Instance.MoneyStorage.CurrentMoney >= skin.Price;
                _buyButton.interactable = skin.IsFree || isAffordable;
            }
        }
    }
}