using System.Collections.Generic;
using System.Linq;
using Ateo.Common;
using UnityEngine;

namespace Moreno.SewingGame
{
    public class HighScoreManager : ComponentPublishBehaviour<HighScoreManager>
    {
        private Dictionary<LevelSetting, List<LevelScore>> _data = new Dictionary<LevelSetting, List<LevelScore>>();

        public LevelScore TryGetHighscore(LevelSetting level)
        {
            if (level == null) return null;
            return _data.TryGetValue(level, out var list) ? list.OrderBy(x => x.CumulativeScore(level)).First() : null;
        }

        public void AddHighScore(LevelSetting level, LevelScore score)
        {
            if (_data.ContainsKey(level))
            {
                _data[level].Add(score);
            }
            else
            {
                var table = new List<LevelScore>()
                {
                    score
                };
                _data.Add(level, table);
            }
        }
    }
}