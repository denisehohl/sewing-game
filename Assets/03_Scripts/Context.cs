using System;

namespace Moreno.SewingGame
{
	public static class Context
	{
		private static LevelSetting _currentLevel;
		private static bool _inTutorial;

		public static event Action<bool> OnTutorialChanged;

		public static LevelSetting CurrentLevel
		{
			get => _currentLevel;
			set => _currentLevel = value;
		}

		public static bool InTutorial
		{
			get => _inTutorial;
			set
			{
				if(value == _inTutorial) return;
				_inTutorial = value;
				OnTutorialChanged?.Invoke(value);
			}
		}
	}
}