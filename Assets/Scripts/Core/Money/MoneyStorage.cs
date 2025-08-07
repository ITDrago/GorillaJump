using System;

namespace Core.Money
{
    public class MoneyStorage
    {
        public event Action<int> OnMoneyChanged;

        public int CurrentMoney { get; private set; }

        public MoneyStorage(int startingAmount)
        {
            CurrentMoney = startingAmount;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;

            CurrentMoney += amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
        }
    }
}