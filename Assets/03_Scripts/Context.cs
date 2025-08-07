namespace Moreno.SewingGame
{
	public class Context
	{
		private static LevelSetting _currentLevel;

		public static LevelSetting CurrentLevel
		{
			get => _currentLevel;
			set => _currentLevel = value;
		}
	}
}