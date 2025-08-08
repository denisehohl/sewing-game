using UnityEngine;

namespace Ateo.StateManagement
{
	public sealed class InGame : State<InGame>
	{
		public override IState StateParent => null;
		public override IState StateNext => null;
		public override IState StateBack => null;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Instance = new InGame();
		}
#endif
	}
}
