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
        
        public void PlayMusic(AudioClip musicClip, bool loop = true)
        {
            if (_musicSource && musicClip)
            {
                _musicSource.clip = musicClip;
                _musicSource.loop = loop;
                _musicSource.Play();
            }
        }
    }
}