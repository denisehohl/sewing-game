namespace Ateo.Extensions
{
	public static class IntegerExtensions
	{
		public static int Increment(this int i, int min, int max, int value)
		{
			i += value;

			if (i < min)
			{
				i = max;
			}
			else if (i > max)
			{
				i = min;
			}

			return i;
		}

		public static int FromBytesToMegabytes(this int bytes)
		{
			return bytes / 1024 / 1024;
		}
	}
}