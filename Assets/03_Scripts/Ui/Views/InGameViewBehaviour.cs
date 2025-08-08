using Ateo.StateManagement;
using Ateo.UI;
using UnityEngine;

namespace Moreno.SewingGame.Ui.Views
{
    public class InGameViewBehaviour : ViewBehaviour
    {
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
            
        }

        private void UnsubscribeFromButtons()
        {
            
        }
        
        private void Update()
        {
            if(StateManager.CurrentEnum != StatesEnum.InGame) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StateManager.ChangeTo(StatesEnum.Paused);
            }
        }

        private void Init()
        {
            
        }
        
        #region Event Callbacks
        

        #endregion
    }
}