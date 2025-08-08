using System;

namespace Moreno.SewingGame
{
    [Serializable]
    public class LevelScore
    {
        public float Time;
        public float DamageTaken;
        public float Inacuracy;

        public float CumulativeScore(LevelSetting level)
        {
            float accuracy = level.GetAccuracyPercentage(Inacuracy);
            float clean = level.GetCleanPercentage(DamageTaken);
            return Time * clean * accuracy;
        }
    }
}