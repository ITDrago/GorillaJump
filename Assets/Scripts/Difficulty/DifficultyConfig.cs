using UnityEngine;

namespace Difficulty
{
    [CreateAssetMenu(menuName = "Game/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        public DifficultyLevel[] Levels;
    }

    [System.Serializable]
    public class DifficultyLevel
    {
        public int StartBlockIndex;
        [Range(1, 10)] public float BlockSpacing = 2;
        [Range(-5, 0)] public float MinHeight = 0;
        [Range(0, 5)] public float MaxHeight = 3;
        [Range(0, 1)] public float GapProbability = 0.1f;
    }
}