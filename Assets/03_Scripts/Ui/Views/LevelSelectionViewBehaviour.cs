using System.Collections.Generic;
using Ateo.UI;
using UnityEngine;

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
            
        }

        private void UnsubscribeFromButtons()
        {
            
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