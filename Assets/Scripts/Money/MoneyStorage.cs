using System;

namespace Money
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

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return false;
            if (CurrentMoney < amount) return false;

            CurrentMoney -= amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
            return true;
        }
    }
}