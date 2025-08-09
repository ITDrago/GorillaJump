using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio References")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioClip _sfxVolumeTestClip;
        
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SfxVolume";

        private Coroutine _musicCoroutine;
        private SceneMusicProfile _currentProfile;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadVolumeSettings();
        }
        
        public void PlayMusicPlaylist(SceneMusicProfile profile)
        {
            if (!profile || profile.MusicClips.Length == 0)
            {
                StopMusic();
                return;
            }

            if (_currentProfile == profile && _musicCoroutine != null)
            {
                return;
            }

            _currentProfile = profile;
            StopMusic();
            _musicCoroutine = StartCoroutine(PlaylistCoroutine(profile));
        }

        public void StopMusic()
        {
            if (_musicCoroutine != null)
            {
                StopCoroutine(_musicCoroutine);
                _musicCoroutine = null;
            }
            _musicSource.Stop();
            _musicSource.clip = null;
            _currentProfile = null;
        }
        
        private IEnumerator PlaylistCoroutine(SceneMusicProfile profile)
        {
            var clips = new List<AudioClip>(profile.MusicClips);
            var clipIndex = 0;

            while (true)
            {
                if (profile.Order == SceneMusicProfile.PlaybackOrder.Shuffle)
                {
                    Shuffle(clips);
                }

                while (clipIndex < clips.Count)
                {
                    var currentClip = clips[clipIndex];
                    _musicSource.clip = currentClip;
                    _musicSource.loop = false;
                    _musicSource.Play();
                
                    yield return new WaitUntil(() => !_musicSource.isPlaying);

                    clipIndex++;
                }
                
                if (profile.LoopPlaylist)
                {
                    clipIndex = 0;
                }
                else
                {
                    break;
                }
            }
            
            _musicCoroutine = null;
            _currentProfile = null;
        }

        private void Shuffle<T>(List<T> list)
        {
            var random = new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = volume;
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxSource.volume = volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
            PlayerPrefs.Save();
        }

        public void PlaySfx(AudioClip audioClip, float volumeScale = 1)
        {
            if (_sfxSource && audioClip)
            {
                _sfxSource.PlayOneShot(audioClip, volumeScale);
            }
        }
        
        public void PlaySfxForUITest() => PlaySfx(_sfxVolumeTestClip);

        private void LoadVolumeSettings()
        {
            var musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            var sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            
            _musicSource.volume = musicVolume;
            _sfxSource.volume = sfxVolume;
        }
        
        public float GetMusicVolume() => _musicSource.volume;

        public float GetSfxVolume() => _sfxSource.volume;
    }
}