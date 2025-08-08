using Ateo.StateManagement;
using Ateo.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class ResultViewBehaviour : ViewBehaviour
    {
        [SerializeField]
        private TMP_Text _levelNameText;
        [SerializeField]
        private ScoreDisplayView _scoreDisplay;
        [SerializeField]
        private Button _continueButton;
        [SerializeField]
        private Button _retryButton;
        
        protected override void OnShowStart()
        {
            Init();
        }

        protected override void OnShowFinished()
        {
            SubscribeToButtons();
        }

        protected override void OnHideStart()
        {
            UnsubscribeFromButtons();
        }

        private void SubscribeToButtons()
        {
            _continueButton.onClick.AddListener(OnContinue);
            _retryButton.onClick.AddListener(OnRetry);
        }

        private void UnsubscribeFromButtons()
        {
            _continueButton.onClick.RemoveListener(OnContinue);
            _retryButton.onClick.RemoveListener(OnRetry);
        }

        private void Init()
        {
            var level = Context.CurrentLevel;
            if(level == null) return;
            var score = HighScoreManager.Instance.TryGetHighscore(level);
            _scoreDisplay.Init(level, score);
            _levelNameText.text = level.name;
        }
        
        #region Event Callbacks
        
        private void OnRetry()
        {
            MainManager.Instance.StartLevel(Context.CurrentLevel);
        }

        private void OnContinue()
        {
            StateManager.ChangeTo(StatesEnum.LevelSelect);
        }

        #endregion
    }
}