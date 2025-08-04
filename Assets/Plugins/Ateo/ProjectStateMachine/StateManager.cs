using UnityEngine;

namespace Ateo.StateManagement
{
	/// <summary>
	/// Manages the high-level state of the application
	/// </summary>
	public static class StateManager
	{
#if STATEMACHINE
		public static IState Current { get; private set; }
		public static IState Previous { get; private set; }

		public static StatesEnum CurrentEnum { get; private set; }
		public static StatesEnum PreviousEnum { get; private set; }

		private static int FrameCount{ get; set; }
		public static bool HasChangedThisFrame => FrameCount == Time.frameCount;

		public delegate void EventHandler();

		public delegate void EventHandlerStatesEnum(StatesEnum state, StatesEnum previous);


		public static event EventHandler OnNext;
		public static event EventHandler OnBack;
		public static event EventHandlerStatesEnum OnStateChanged;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			FrameCount = -1;
			Current = null;
			Previous = null;
			CurrentEnum = StatesEnum.None;
			PreviousEnum = StatesEnum.None;
			OnNext = null;
			OnBack = null;
			OnStateChanged = null;
		}
#endif

		public static void ChangeTo(StatesEnum stateEnum)
		{
			ChangeTo(StateHelper.GetState(stateEnum));
		}

		public static void ChangeTo(IState state)
		{
			if (state == null || Current == state)
				return;

			// Save

			Previous = Current;
			Current = state;

			PreviousEnum = StateHelper.GetStatesEnum(Previous);
			CurrentEnum = StateHelper.GetStatesEnum(Current);

			FrameCount = Time.frameCount;

			if (Previous != null)
			{
				if (!Previous.CanComplete)
				{
					DebugDev.Log($"ApplicationState: Completing of State {Previous.Name} prevented");
					return;
				}

				// Check if old and new parent are different

				if (Previous.StateParent != null && Previous.StateParent != state.StateParent)
				{
					if (!Previous.StateParent.CanComplete)
					{
						DebugDev.Log($"ApplicationState: Completing of State {Previous.Name} prevented by parent {Previous.StateParent.Name}");
						return;
					}

					// Complete old state

					DebugDev.Log($"ApplicationState: Complete State {Previous.Name}");
					Previous.Complete();

					// Complete old parent state

					DebugDev.Log($"ApplicationState: Complete Parent State {Previous.StateParent.Name}");
					Previous.StateParent.Complete();
				}
				else
				{
					// Complete old state

					DebugDev.Log($"ApplicationState: Complete State {Previous.Name}");
					Previous.Complete();
				}
			}

			// Start  parent state of new state

			if (Current.StateParent != null)
			{
				if (Previous == null || Previous.StateParent != Current.StateParent)
				{
					DebugDev.Log($"ApplicationState: Start Parent State {state.StateParent.Name}");
					Current.StateParent.Start();
				}
			}

			// Start new state

			DebugDev.Log($"ApplicationState: Start State {Current.Name}");
			Current.Start();

			// Notify parent state that sub state has changed

			Current.StateParent?.NotifySubStateChanged();

			// Invoke event

			DebugDev.Log(Previous != null
				? $"ApplicationState: Changed state from {Previous.Name} to {Current.Name}"
				: $"ApplicationState: Changed state to {Current.Name}");

			OnStateChanged?.Invoke(CurrentEnum, PreviousEnum);
		}

		public static void Next()
		{
			IState nextState = Current?.StateNext;

			if (nextState != null)
			{
				OnNext?.Invoke();
				ChangeTo(nextState);
			}
		}

		public static void Back()
		{
			IState backState = Current?.StateBack;

			if (backState != null)
			{
				OnBack?.Invoke();
				ChangeTo(backState);
			}
		}
#endif
	}
}