using Core.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Audio
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private void Start()
        {
            if (!SoundManager.Instance)
            {
                gameObject.SetActive(false);
                return;
            }

            _musicVolumeSlider.value = SoundManager.Instance.GetMusicVolume();
            _sfxVolumeSlider.value = SoundManager.Instance.GetSfxVolume();

            _musicVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
            _sfxVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume);
        }

        private void OnDestroy()
        {
            if (SoundManager.Instance)
            {
                _musicVolumeSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetMusicVolume);
                _sfxVolumeSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSfxVolume);
            }
        }
    }
}