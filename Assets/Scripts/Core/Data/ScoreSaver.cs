using UnityEngine;

namespace Core.Data
{
    public static class ScoreSaver
    {
        private const string HIGH_SCORE_KEY = "GorillaGame_HighScore";

        public static void SaveScore(int newScore)
        {
            var currentHighScore = LoadScore();

            if (newScore > currentHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, newScore);
                PlayerPrefs.Save();
            }
        }

        public static int LoadScore()
        {
            return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }
    }
}