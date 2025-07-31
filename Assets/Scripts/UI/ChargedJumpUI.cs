using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChargedJumpUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Slider _chargeSlider;
        [SerializeField] private GameObject _container;

        [Header("Settings")]
        [SerializeField] private float _chargeSpeed = 2; 
        
        private float _currentCharge;
        private bool _isCharging;
        private int _direction = 1;

        private void Awake()
        {
            if (!_chargeSlider) enabled = false;
        }

        public void UpdateBar()
        {
            if (!_isCharging) return;
            
            _currentCharge += Time.deltaTime * _chargeSpeed * _direction;
            
            if (_currentCharge >= 1f)
            {
                _currentCharge = 1f;
                _direction = -1;
            }
            else if (_currentCharge <= 0f)
            {
                _currentCharge = 0f;
                _direction = 1;
            }

            _chargeSlider.value = _currentCharge;
        }
        
        public void Show() => _container.SetActive(true);

        public void Hide() => _container.SetActive(false);

        public void StartCharge()
        {
            _currentCharge = 0f;
            _direction = 1;
            _isCharging = true;
        }

        public void StopCharge() => _isCharging = false;

        public float GetCurrentChargeValue() => _chargeSlider.value;
    }
}