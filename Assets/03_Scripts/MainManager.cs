using System;
using Ateo.Common;
using Ateo.StateManagement;
using Moreno.SewingGame.Path;
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
		private StatesEnum _startState;

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
			GoToState(_startState);
		}

		protected override void OnPublish()
		{
			PathEvaluater.OnPathEndReached += OnPathEnded;
		}

		private void OnPathEnded()
		{
			GatherScore();
			StateManager.ChangeTo(StatesEnum.Result);
		}

		private void GatherScore()
		{
			var score = SewingMachineController.Instance.GatherScore();
			HighScoreManager.Instance.AddHighScore(Context.CurrentLevel,score);
		}

		protected override void OnWithdraw()
		{
			PathEvaluater.OnPathEndReached -= OnPathEnded;
			base.OnWithdraw();
			Context.CurrentLevel = null;
		}

		#endregion

		#region Public Methods

		[Button]
		public void StartLevel(LevelSetting level)
		{
			Context.CurrentLevel = level;
			Context.InTutorial = level.IsTutorial;
			SewingMachineController.Instance.PrepareLevel();
			StateManager.ChangeTo(StatesEnum.InGame);
			OnLevelStarted?.Invoke();
		}

		#endregion

		#region Private Methods

		[Button]
		private void GoToState(StatesEnum state)
		{
			StateManager.ChangeTo(state);
		}

		#endregion

		#region Event Callbacks

		#endregion


	}
}