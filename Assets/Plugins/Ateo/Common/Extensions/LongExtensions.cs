namespace Ateo.Extensions
{
	public static class LongExtensions
	{
		public static long Increment(this long i, long min, long max, long value)
		{
			i += value;

			if(i < min)
			{
				i = max;
			}
			else if(i > max)
			{
				i = min;
			}

			return i;
		}
		
		public static long FromBytesToMegabytes(this long bytes)
		{
			return bytes / 1024 / 1024;
		}
	}
}