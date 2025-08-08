using UnityEngine;

namespace Ateo.StateManagement
{
	public sealed class Paused : State<Paused>
	{
		public override IState StateParent => States.InGame;
		public override IState StateNext => null;
		public override IState StateBack => null;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Instance = new Paused();
		}
#endif
	}
}
