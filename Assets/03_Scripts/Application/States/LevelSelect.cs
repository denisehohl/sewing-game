using UnityEngine;

namespace Ateo.StateManagement
{
	public sealed class LevelSelect : State<LevelSelect>
	{
		public override IState StateParent => null;
		public override IState StateNext => null;
		public override IState StateBack => null;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Instance = new LevelSelect();
		}
#endif
	}
}
