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
        [Range(3f, 10f)] public float BlockSpacing = 5f;
        [Range(-5f, 0f)] public float MinHeight = -2f;
        [Range(0f, 5f)] public float MaxHeight = 2f;
        [Range(0f, 1f)] public float GapProbability = 0.1f;
    }
}