using UnityEngine;

namespace Core.Audio
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        public void PlaySfx(AudioClip audioClip, float volume = 1)
        {
            if (_sfxSource && audioClip) _sfxSource.PlayOneShot(audioClip, volume);
        }

        public void PlayMusic() => _musicSource.Play();
        public void StopMusic() => _musicSource.Stop();
        public void AdjustMusicVolume(float volume) => _musicSource.volume = volume;
    }
}