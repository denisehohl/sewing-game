using Ateo.StateManagement;
using Ateo.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class MainViewBehaviour : ViewBehaviour
    {
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private Button _quitButton;
        
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
            _startButton.onClick.AddListener(StartGame);
            _quitButton.onClick.AddListener(QuitGame);
        }


        private void UnsubscribeFromButtons()
        {
            _startButton.onClick.RemoveListener(StartGame);
            _quitButton.onClick.RemoveListener(QuitGame);
        }

        private void Init()
        {
            
        }

        #region Event Callbacks
        
        private void StartGame()
        {
            StateManager.ChangeTo(StatesEnum.LevelSelect);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}