using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class LevelSelectEntry : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private ScoreDisplayView _scoreDisplay;
        [SerializeField]
        private Button _startLevelButton;

        public void Populate(LevelSetting level)
        {
            _text.text = level.LevelName;
            _scoreDisplay.Init(level,HighScoreManager.Instance.TryGetHighscore(level));
            _startLevelButton.onClick.RemoveAllListeners();
            _startLevelButton.onClick.AddListener(() =>MainManager.Instance.StartLevel(level));
        }
    }
}