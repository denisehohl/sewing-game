using System.Collections.Generic;
using UnityEngine;

namespace Moreno.SewingGame.Path
{
	public static class PathHelper
	{
		public static float GetDistanceToClosestPointOnPath(this List<Vector2> points, Vector2 localTarget) =>
			GetDistanceToClosestPointOnPath(points, localTarget, out Vector2 pointOnTrack, out int start, out int end, out float t);
		
		public static float GetDistanceToClosestPointOnPath(this List<Vector2> points, Vector2 localTarget, out Vector2 pointOnTrack) =>
			GetDistanceToClosestPointOnPath(points, localTarget, out pointOnTrack, out _, out _, out _);
		
		public static float GetDistanceToClosestPointOnPath(this List<Vector2> points, Vector2 localTarget, out Vector2 pointOnTrack, out int start) =>
			GetDistanceToClosestPointOnPath(points, localTarget, out pointOnTrack, out start, out int end, out float t);
		
		public static float GetDistanceToClosestPointOnPath(this List<Vector2> points, Vector2 localTarget, out Vector2 pointOnTrack, out int start, out int end, out  float t)
		{
			float currentClosestSqrMag = float.MaxValue;

			pointOnTrack = Vector2.zero;
			Vector2 b = points[0];
			start = 0;
			t = 0;
			
			for (int i = 1; i < points.Count; i++)
			{
				Vector2 a = points[i];

				Vector2 pointOnSegment = GetClosestPointOnLineSegment(a, b, localTarget,out float n);
				float sqrMagnitude = (pointOnSegment - localTarget).sqrMagnitude;
				if (sqrMagnitude < currentClosestSqrMag)
				{
					currentClosestSqrMag = sqrMagnitude;
					start = i;
					t = (float) n;
					pointOnTrack = pointOnSegment;
				}
				b = a;
			}

			end = start + 1;
			
			return Vector2.Distance(localTarget, pointOnTrack);
		}
		
		private static Vector2 GetClosestPointOnLineSegment(Vector2 a, Vector2 b, Vector2 target, out float normalisedDistance)
		{
			Vector2 aToB = b - a;
			Vector2 aToTarget = target - a;

			float segmentSqrLength = aToB.sqrMagnitude;
			if (segmentSqrLength == 0)
			{
				normalisedDistance = 0;
				return a;
			}

			float dotProduct = Vector2.Dot(aToTarget, aToB);
			normalisedDistance = dotProduct / segmentSqrLength;

			switch (normalisedDistance)
			{
				case < 0:
					normalisedDistance = 0;
					return a;
				case > 1:
					normalisedDistance = 1;
					return b;
				default:
					return a + aToB * normalisedDistance;
			}
		}
	}
}