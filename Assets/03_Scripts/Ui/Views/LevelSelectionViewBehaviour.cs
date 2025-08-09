using System.Collections.Generic;
using Ateo.StateManagement;
using Ateo.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Moreno.SewingGame.Ui.Views
{
    public class LevelSelectionViewBehaviour : ViewBehaviour
    {
        [SerializeField]
        private LevelSetting[] _avaliableLevels;

        [SerializeField]
        private LevelSelectEntry _levelSelectEntryPrefab;

        [SerializeField]
        private Transform _listParent;
        [SerializeField]
        private Button _backButton;

        private List<LevelSelectEntry> _instances = new List<LevelSelectEntry>();

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
            _backButton.onClick.AddListener(() => StateManager.ChangeTo(StatesEnum.Main));
        }

        private void UnsubscribeFromButtons()
        {
            _backButton.onClick.RemoveAllListeners();
        }

        private void Init()
        {
            PopulateLevelDisplays();
        }

        private void PopulateLevelDisplays()
        {
            for (int i = 0; i < _avaliableLevels.Length; i++)
            {
                LevelSetting level = _avaliableLevels[i];
                var ui = GetOrCreateUiInstance(i);
                ui.Populate(level);
            }
        }

        private LevelSelectEntry GetOrCreateUiInstance(int index)
        {
            if (_instances.Count > index) return _instances[index];
            var newInstance = Instantiate(_levelSelectEntryPrefab, _listParent);
            _instances.Add(newInstance);
            return newInstance;

        }
        
        #region Event Callbacks
        

        #endregion
    }
}