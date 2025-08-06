using UnityEngine;
using UnityEngine.UI;

namespace Core.Audio
{
    public class ButtonSfx : MonoBehaviour
    {
        [SerializeField] private AudioClip _clickSound;
        
        private SoundManager _soundManager;
        private Button _button;
        private void Start()
        {
            _soundManager = (SoundManager)FindFirstObjectByType(typeof(SoundManager));
            _button = GetComponent<Button>();
            if(_button) _button.onClick.AddListener(PlayClickSfx);
        }
        
        private void OnDestroy()
        {
            if (_button) _button.onClick.RemoveListener(PlayClickSfx);
        }

        private void PlayClickSfx() => _soundManager.PlaySfx(_clickSound, 0.8f);
    }
}
