using System;
using Ateo.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class MainManager : ComponentPublishBehaviour<MainManager>
	{
		#region private Serialized Variables

		[SerializeField]
		private SettingsData _currentSettings;
		[SerializeField]
		private LevelSetting _levelToStart;

		#endregion

		#region private Variables

		#endregion

		#region Properties

		public SettingsData CurrentSettings => _currentSettings;

		#endregion

		#region Delegates & Events

		public static event Action OnLevelStarted;

		#endregion

		#region Monobehaviour Callbacks

		protected override void OnStart()
		{
			StartLevel(_levelToStart);
		}

		protected override void OnWithdraw()
		{
			base.OnWithdraw();
			Context.CurrentLevel = null;
		}

		#endregion

		#region Public Methods

		[Button]
		public void StartLevel(LevelSetting level)
		{
			Context.CurrentLevel = level;
			SewingMachineController.Instance.PrepareLevel();
			OnLevelStarted?.Invoke();
		}

		#endregion

		#region Private Methods

		#endregion

		#region Event Callbacks

		#endregion


	}
}