#if TIMELINE
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Ateo.StateManagement.Playables
{
	public class StateManagerMixerBehaviour : PlayableBehaviour
	{
		public static event Action<StatesEnum> OnRequestState;

		#region Reset Statics

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			OnRequestState = null;
		}

		#endregion

		// NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			int inputCount = playable.GetInputCount();

			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				ScriptPlayable<StateManagerBehaviour> inputPlayable = (ScriptPlayable<StateManagerBehaviour>)playable.GetInput(i);
				StateManagerBehaviour input = inputPlayable.GetBehaviour();

				if (Mathf.RoundToInt(inputWeight) == 1)
				{
					if (StateManager.CurrentEnum != input.State)
					{
						OnRequestState?.Invoke(input.State);
					}
				}
			}
		}
	}
}
#endif