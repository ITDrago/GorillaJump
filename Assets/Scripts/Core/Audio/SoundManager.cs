using System.Collections;
using UnityEngine;

namespace Core.Audio
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart = true;
        
        [Header("Audio References")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;
        
        [SerializeField] private AudioClip[] _backgroundMusic;
        
        private int _currentBackgroundMusicIndex;

        private void Start()
        {
            _musicSource.loop = false;

            if (_playOnStart)
            {
                _musicSource.Play();
                StartCoroutine(PlayPlaylist());
            }
        }
        
        public void PlaySfx(AudioClip audioClip, float volume = 1)
        {
            if (_sfxSource && audioClip) _sfxSource.PlayOneShot(audioClip, volume);
        }
        
        public void AdjustMusicVolume(float volume) => _musicSource.volume = volume;
        
        private IEnumerator PlayPlaylist()
        {
            while (true)
            {
                if (_backgroundMusic.Length > 0)
                {
                    _musicSource.clip = _backgroundMusic[_currentBackgroundMusicIndex];
                    _musicSource.Play();

                    yield return new WaitUntil(() => !_musicSource.isPlaying);

                    _currentBackgroundMusicIndex++;

                    if (_currentBackgroundMusicIndex >= _backgroundMusic.Length) _currentBackgroundMusicIndex = 0;
                }
                else
                {
                    break;
                }
            }
        }
    }
}