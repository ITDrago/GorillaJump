using UnityEngine;
using UnityEngine.Localization;

namespace UI.Menu.Localization
{
    [CreateAssetMenu(fileName = "ShopLocalization", menuName = "Shop/Shop Localization Settings")]
    public class ShopLocalizationSO : ScriptableObject
    {
        [SerializeField] private LocalizedString _freePriceText;
        public LocalizedString FreePriceText => _freePriceText;

        [SerializeField] private LocalizedString _selectButtonText;
        public LocalizedString SelectButtonText => _selectButtonText;

        [SerializeField] private LocalizedString _selectedButtonText;
        public LocalizedString SelectedButtonText => _selectedButtonText;
    }
}