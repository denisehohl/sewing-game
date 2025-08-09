using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class ScoreDisplayView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _timeDisplay;
        [SerializeField]
        private TMP_Text _accuracyDisplay;
        [SerializeField]
        private TMP_Text _cleanlinessDisplay;

        private Image[] _starDisplays;

        private string PERCENTAGE_FORMAT = "##0.#";

        public void Init(LevelSetting level, LevelScore score, bool withTimeText = true)
        {
            bool hasScore = score != null;
            
            _timeDisplay.gameObject.SetActive(true);
            _accuracyDisplay.gameObject.SetActive(hasScore);
            _cleanlinessDisplay.gameObject.SetActive(hasScore);

            if (!hasScore)
            {
                _timeDisplay.text = "no Score";
                return;
            }
            
            float acuracy = level.GetAccuracyPercentage(score.Inacuracy);
            float clean = level.GetCleanPercentage(score.DamageTaken);

            if (withTimeText)
            {
                _timeDisplay.text = $"Time: {FloatSecondsToTimeString(score.Time)}";
            }
            else
            {
                _timeDisplay.text = $"{FloatSecondsToTimeString(score.Time)}";
            }
            _accuracyDisplay.text = $"Accuracy: {acuracy.ToString(PERCENTAGE_FORMAT)}%";
            _cleanlinessDisplay.text = $"Cleanliness: {clean.ToString(PERCENTAGE_FORMAT)}%";
        }
        
        public static string FloatSecondsToTimeString(float totalSeconds)
        {
            int hours = (int)(totalSeconds / 3600);
            int minutes = (int)((totalSeconds % 3600) / 60);
            float seconds = totalSeconds % 60;

            if (hours > 0)
            {
                return string.Format("{0}:{1:D2}:{2:00.0}", hours, minutes, seconds);
            }
            else
            {
                return string.Format("{0:D2}:{1:00.0}", minutes, seconds);
            }
        }
    }
}