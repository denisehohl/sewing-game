using System;
using Ateo.StateManagement;
using Ateo.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class PauseMenuViewBehaviour : ViewBehaviour
    {
        [SerializeField] 
        private Button _resumeButton;
        [SerializeField] 
        private Button _exitButton;

        protected override void OnShowFinished()
        {
            SubscribeToButtons();
        }

        protected override void OnHideStart()
        {
            UnsubscribeFromButtons();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnResume();
            }
        }

        private void SubscribeToButtons()
        {
            _resumeButton.onClick.AddListener(OnResume);
            _exitButton.onClick.AddListener(OnExit);
        }


        private void UnsubscribeFromButtons()
        {
            _resumeButton.onClick.RemoveListener(OnResume);
            _exitButton.onClick.RemoveListener(OnExit);
        }

        #region Event Callbacks

        private void OnResume()
        {
            StateManager.ChangeTo(StatesEnum.InGame);
        }

        private void OnExit()
        {
            SewingMachineController.Instance.ResetMachine();
            StateManager.ChangeTo(StatesEnum.Main);
        }

        #endregion
    }
}