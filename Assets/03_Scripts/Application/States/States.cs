//-----------------------------------------------------------------------
// This file is AUTO-GENERATED.
// Changes for this script by hand might be lost when auto-generation is run.
// Generated date: 2025/08/08 13:13:25
//-----------------------------------------------------------------------

namespace Ateo.StateManagement
{
	public static class States
	{
		public static readonly IState InGame = StateManagement.InGame.Instance;
		public static readonly IState LevelSelect = StateManagement.LevelSelect.Instance;
		public static readonly IState Main = StateManagement.Main.Instance;
		public static readonly IState Paused = StateManagement.Paused.Instance;
		public static readonly IState Result = StateManagement.Result.Instance;
	}

	public enum StatesEnum
	{
		None,
		InGame,
		LevelSelect,
		Main,
		Paused,
		Result,
	}
}
