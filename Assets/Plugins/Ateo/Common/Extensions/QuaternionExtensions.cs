using UnityEngine;

namespace Ateo.Extensions
{
	public static class QuaternionExtensions
	{
		public static Quaternion GetNormalized(this Quaternion q)
		{
			float f = 1f / Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
			return new Quaternion(q.x * f, q.y * f, q.z * f, q.w * f);
		}

		public static Quaternion Add(this Quaternion first, Quaternion second)
		{
			first.w += second.w;
			first.x += second.x;
			first.y += second.y;
			first.z += second.z;
			return first;
		}

		public static Quaternion Scale(this Quaternion rotation, float multiplier)
		{
			rotation.w *= multiplier;
			rotation.x *= multiplier;
			rotation.y *= multiplier;
			rotation.z *= multiplier;
			return rotation;
		}

		public static float Magnitude(this Quaternion rotation)
		{
			return Mathf.Sqrt((Quaternion.Dot(rotation, rotation)));
		}
	}
}