using UnityEngine;

namespace Core.Audio
{
    public class SceneMusicSetter : MonoBehaviour
    {
        [SerializeField] private SceneMusicProfile _sceneMusicProfile;
        [SerializeField] private bool _stopMusicInThisScene;

        private void Start()
        {
            if (!SoundManager.Instance) return;
            
            if (_stopMusicInThisScene)
            {
                SoundManager.Instance.StopMusic();
            }
            else if (_sceneMusicProfile)
            {
                SoundManager.Instance.PlayMusicPlaylist(_sceneMusicProfile);
            }
        }
    }
}