#if TIMELINE
using System;
using UnityEngine.Playables;

namespace Ateo.StateManagement.Playables
{
	[Serializable]
	public class StateManagerBehaviour : PlayableBehaviour
	{
		public StatesEnum State;
	}
}
#endif