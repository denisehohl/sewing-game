using UnityEngine;

namespace Ateo.StateManagement
{
	public sealed class Main : State<Main>
	{
		public override IState StateParent => null;
		public override IState StateNext => null;
		public override IState StateBack => null;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Instance = new Main();
		}
#endif
	}
}
