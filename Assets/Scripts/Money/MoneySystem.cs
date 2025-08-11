using UnityEngine;

namespace Money
{
    public class MoneySystem : MonoBehaviour
    {
        public static MoneySystem Instance { get; private set; }

        public MoneyStorage MoneyStorage { get; private set; }
        
        private const string MONEY_SAVE_KEY = "PlayerTotalMoney";

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Load();
        }

        public void AddMoney(int amount)
        {
            MoneyStorage.Add(amount);
        }

        private void Save(int amount)
        {
            PlayerPrefs.SetInt(MONEY_SAVE_KEY, amount);
        }

        private void Load()
        {
            var savedMoney = PlayerPrefs.GetInt(MONEY_SAVE_KEY, 0);
            MoneyStorage = new MoneyStorage(savedMoney);
            MoneyStorage.OnMoneyChanged += Save;
        }
    }
}