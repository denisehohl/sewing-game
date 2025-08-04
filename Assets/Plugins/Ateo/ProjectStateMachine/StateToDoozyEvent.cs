#if (DOOZY_3 || DOOZY_4) && STATEMACHINE

using Ateo.Common;
using UnityEngine;
#if DOOZY_3
using Doozy.Engine;
#elif DOOZY_4
using Doozy.Runtime.Signals;
#endif

namespace Ateo.StateManagement
{
	/// <summary>
	/// This class translates ApplicationState change events to Doozy-GameEventMessages
	/// </summary>
	public class StateToDoozyEvent : ComponentPublishBehaviour<StateToDoozyEvent>
	{
#if DOOZY_4
		[SerializeField]
		private string _streamCategory;
#endif
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
#if DOOZY_3
			GameEventMessage.SendEvent(state.ToString());
#elif DOOZY_4
			// TODO
			Signal.Send(_streamCategory, state.ToString());
#endif
		}
	}
}
#endif