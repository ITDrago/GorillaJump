using UnityEngine;

namespace Core.Audio
{
    [CreateAssetMenu(fileName = "NewSceneMusic", menuName = "Audio/Scene Music Profile")]
    public class SceneMusicProfile : ScriptableObject
    {
        public enum PlaybackOrder
        {
            Sequential,
            Shuffle
        }

        [SerializeField] private AudioClip[] _musicClips;
        [SerializeField] private PlaybackOrder _order = PlaybackOrder.Sequential;
        [SerializeField] private bool _loopPlaylist = true;

        public AudioClip[] MusicClips => _musicClips;
        public PlaybackOrder Order => _order;
        public bool LoopPlaylist => _loopPlaylist;
    }
}