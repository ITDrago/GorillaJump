using Core.Money;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Money
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private Text _moneyText;

        private void OnEnable()
        {
            if (!MoneySystem.Instance) return;

            MoneySystem.Instance.MoneyStorage.OnMoneyChanged += UpdateText;
            UpdateText(MoneySystem.Instance.MoneyStorage.CurrentMoney);
        }

        private void OnDisable()
        {
            if (!MoneySystem.Instance) return;
            
            MoneySystem.Instance.MoneyStorage.OnMoneyChanged -= UpdateText;
        }

        private void UpdateText(int newAmount) => _moneyText.text = newAmount.ToString();
    }
}