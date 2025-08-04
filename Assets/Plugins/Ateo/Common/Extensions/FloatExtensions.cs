using UnityEngine;

namespace Ateo.Extensions
{
	public static class FloatExtensions
	{
		public static float Eased(this float f)
		{
			float e = Mathf.Clamp01(f);
			e *= Mathf.PI;
			e -= Mathf.PI / 2;
			e = Mathf.Sin(e);
			e *= 0.5f;
			e += 0.5f;
			return e;
		}
		
		public static float Hypotenuse(this float f, float otherSide)
		{
			return Mathf.Sqrt(f * f + otherSide * otherSide);
		}
	}
}