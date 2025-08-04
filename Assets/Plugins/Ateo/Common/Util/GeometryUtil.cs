using UnityEngine;

namespace Ateo.Common.Util
{
	public static class GeometryUtil
	{
		/// <summary>
		/// Get the closest point on a finite line
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lineStart"></param>
		/// <param name="lineEnd"></param>
		/// <returns></returns>
		public static Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 lineDirection = lineEnd - lineStart;
			float lineLength = lineDirection.magnitude;
			lineDirection.Normalize();

			float projectLength = Mathf.Clamp(Vector3.Dot(point - lineStart, lineDirection), 0f, lineLength);
			return lineStart + lineDirection * projectLength;
		}

		/// <summary>
		/// Get the closest point on an infinite line
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lineStart"></param>
		/// <param name="lineDirection"></param>
		/// <returns></returns>
		public static Vector3 GetClosestPointOnInfiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineDirection)
		{
			return lineStart + Vector3.Project(point - lineStart, lineDirection);
		}
	}
}