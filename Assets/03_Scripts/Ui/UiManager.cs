using System;
using System.Collections.Generic;
using System.Linq;
using Ateo.Common;
using Ateo.StateManagement;
using Ateo.ViewManagement;
using UnityEngine;

namespace Moreno.SewingGame.Ui
{
    public class UiManager : ComponentPublishBehaviour<UiManager>
    {
        private UIView _main;
        private UIView _LevelSelect;
        private UIView _inGame;
        private UIView _result;

        private List<UIView> _activeViews;
        private List<UIView> _showViews;
        private List<UIView> _hideViews;
        private List<UIView> _showViewsImmediate;
        private List<UIView> _hideViewsImmediate;
        
        protected override void OnPublish()
        {
            StateManager.OnStateChanged += OnStateChanged;
        }

        protected override void OnWithdraw()
        {
            StateManager.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(StatesEnum state, StatesEnum previous)
        {
            switch (state)
            {
                case StatesEnum.None: 
                    break;
                case StatesEnum.InGame:
                    break;
                case StatesEnum.LevelSelect:
                    break;
                case StatesEnum.Main:
                    break;
                case StatesEnum.Result:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void UpdateViews()
        {
            foreach (UIView hideView in _hideViewsImmediate)
            {
                hideView.InstantHide(true);
                // Quick Fix so that game object is disabled in this frame (UiView disables the object after 3 frames)
                hideView.gameObject.SetActive(false); 
            }

            //get difference and hide the active views which need to be disabled
            foreach (UIView hideView in _activeViews.Where(x => !_showViews.Contains(x)))
            {
                hideView.Hide();
            }

            _activeViews.Clear();

            foreach (UIView showView in _showViewsImmediate)
            {
                showView.InstantShow(true);
                _activeViews.Add(showView);
            }

            foreach (UIView showView in _showViews)
            {
                showView.Show();
                _activeViews.Add(showView);
            }

            _showViews.Clear();
            _showViewsImmediate.Clear();
            _hideViewsImmediate.Clear();
        }
    }
}