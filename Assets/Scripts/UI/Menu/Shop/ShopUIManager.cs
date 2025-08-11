using Shop;
using Skins.Data;
using UI.Menu.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

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

        [Header("Localization")]
        [SerializeField] private ShopLocalizationSO _localizationSettings;

        private void OnDisable()
        {
            _buyButton.onClick.RemoveAllListeners();
            _selectButton.onClick.RemoveAllListeners();
        }

        public void Configure(ShopManager shopManager)
        {
            _buyButton.onClick.AddListener(shopManager.BuySkin);
            _selectButton.onClick.AddListener(shopManager.SelectCurrentSkin);
        }

        public void UpdateSkinInfo(SkinData skin, bool isPurchased, bool isSelected)
        {
            UpdateLocalizedText(_skinNameText, skin.Name);
            UpdateLocalizedText(_buffDescriptionText, skin.BuffDescription);

            if (skin.IsFree)
            {
                UpdateLocalizedText(_priceText, _localizationSettings.FreePriceText);
            }
            else
            {
                _priceText.text = skin.Price.ToString();
            }

            if (isPurchased)
            {
                _buyButton.gameObject.SetActive(false);
                _selectButton.gameObject.SetActive(true);
                _selectButton.interactable = !isSelected;
                
                var buttonTextString = isSelected ? _localizationSettings.SelectedButtonText : _localizationSettings.SelectButtonText;
                UpdateLocalizedText(_selectButtonText, buttonTextString);
            }
            else
            {
                _buyButton.gameObject.SetActive(true);
                _selectButton.gameObject.SetActive(false);
                var isAffordable = global::Money.MoneySystem.Instance.MoneyStorage.CurrentMoney >= skin.Price;
                _buyButton.interactable = skin.IsFree || isAffordable;
            }
        }
        
        private void UpdateLocalizedText(Text targetText, LocalizedString localizedString)
        {
            void Handler(string value)
            {
                if(targetText) targetText.text = value;
                localizedString.StringChanged -= Handler;
            }
            localizedString.StringChanged += Handler;
            localizedString.RefreshString();
        }
    }
}