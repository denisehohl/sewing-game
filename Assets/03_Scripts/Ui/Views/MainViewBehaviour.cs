using Ateo.UI;
using UnityEngine;

namespace Moreno.SewingGame.Ui.Views
{
    public class MainViewBehaviour : ViewBehaviour
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

        private void Init()
        {
            
        }
    }
}