using System;
using System.Collections.Generic;
using System.Linq;
using Ateo.Common;
using Ateo.StateManagement;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Moreno.SewingGame.Ui
{
    public class UserInterfaceManager : ComponentPublishBehaviour<UserInterfaceManager>
    {
        [SerializeField]
        private UIView _main;
        [SerializeField]
        private UIView _levelSelect;
        [SerializeField]
        private UIView _inGame;
        [SerializeField]
        private UIView _pause;
        [SerializeField]
        private UIView _result;

        private List<UIView> _activeViews = new List<UIView>();
        private List<UIView> _showViews = new List<UIView>();
        private List<UIView> _showViewsImmediate = new List<UIView>();
        private List<UIView> _hideViewsImmediate = new List<UIView>();
        
        protected override void OnPublish()
        {
            StateManager.OnStateChanged += OnStateChanged;
        }

        protected override void OnStart()
        {
            UpdateViews();
        }

        protected override void OnWithdraw()
        {
            StateManager.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(StatesEnum state, StatesEnum previous)
        {
            UpdateViews();
        }

        private void UpdateViews()
        {
            var state = StateManager.CurrentEnum;
            switch (state)
            {
                case StatesEnum.None: 
                    break;
                case StatesEnum.InGame:
                    _showViews.Add(_inGame);
                    break;
                case StatesEnum.LevelSelect:
                    _showViews.Add(_levelSelect);
                    break;
                case StatesEnum.Main:
                    _showViews.Add(_main);
                    break;
                case StatesEnum.Result:
                    _showViews.Add(_result);
                    break;
                case StatesEnum.Paused:
                    _showViews.Add(_pause);
                    break;
            }
            
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