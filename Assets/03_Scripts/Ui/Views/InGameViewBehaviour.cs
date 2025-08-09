using Ateo.StateManagement;
using Ateo.UI;
using UnityEngine;

namespace Moreno.SewingGame.Ui.Views
{
    public class InGameViewBehaviour : ViewBehaviour
    {
        [SerializeField]
        private ScoreDisplayView _scoreDisplayView;
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
            
            _scoreDisplayView.Init(Context.CurrentLevel,SewingMachineController.Instance.GatherScore(),false);
        }

        private void Init()
        {
            _scoreDisplayView.Init(Context.CurrentLevel,SewingMachineController.Instance.GatherScore(),false);
        }
        
        #region Event Callbacks
        

        #endregion
    }
}