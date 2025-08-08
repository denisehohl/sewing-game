using System.Collections.Generic;
using UnityEngine;

namespace Moreno.SewingGame.Path
{
	public static class PathHelper
	{
		public static float GetDistanceToClosestPointOnPath(this PathData data, Vector2 localTarget) =>
			GetDistanceToClosestPointOnPath(data, localTarget, out Vector2 pointOnTrack, out int start, out int end, out float t, out _);
		
		public static float GetDistanceToClosestPointOnPath(this PathData data, Vector2 localTarget, out Vector2 pointOnTrack, out float distanceOnTrack) =>
			GetDistanceToClosestPointOnPath(data, localTarget, out pointOnTrack, out _, out _, out _, out distanceOnTrack);
		
		public static float GetDistanceToClosestPointOnPath(this PathData data, Vector2 localTarget, out Vector2 pointOnTrack, out int start) =>
			GetDistanceToClosestPointOnPath(data, localTarget, out pointOnTrack, out start, out int end, out float t, out _);
		
		public static float GetDistanceToClosestPointOnPath(this PathData data, Vector2 localTarget, out Vector2 pointOnTrack, out int start, out int end, out  float t, out float distanceOnTrack)
		{
			float currentClosestSqrMag = float.MaxValue;

			var points = data.Points;
			var distances = data.Distances;

			pointOnTrack = Vector2.zero;
			start = 0;
			t = 0;
			
			for (int i = 1; i < points.Count; i++)
			{
				Vector2 a = points[i - 1];
				Vector2 b = points[i];

				Vector2 pointOnSegment = GetClosestPointOnLineSegment(a, b, localTarget, out float n);
				float sqrMagnitude = (pointOnSegment - localTarget).sqrMagnitude;

				if (sqrMagnitude < currentClosestSqrMag)
				{
					currentClosestSqrMag = sqrMagnitude;
					start = i - 1;  // Fix the segment start index
					t = n;
					pointOnTrack = pointOnSegment;
				}
			}

			end = start + 1;

			if (start >= distances.Count - 1)
			{
				distanceOnTrack = distances[distances.Count - 1];
			}
			else
			{
				distanceOnTrack = Mathf.Lerp(distances[start], distances[start + 1], t);
			}
			
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
		
		public static bool TryGetPointAtDistance(this PathData path, double distance, out Vector2 point) => 
			TryGetPointAtDistance(path, distance, out point, out int start, out int end, out float t);
		
		public static bool TryGetPointAtDistance(this PathData path, double distance, out Vector2 point, out int start, out int end, out float t)
		{
			var pointDistances = path.Distances;
			var points = path.Points;
			t = default;
			point = default;

			int max = pointDistances.Count - 1;
			end = 0;
			start = max;

			if (distance == pointDistances[max])
			{
				end = max;
				start = max - 1;
			}
			
			while (end <= start)
			{
				int mid = (end + start) / 2;
				var d = pointDistances[mid];
				
				if (d <= distance)
					end = mid + 1;
				else
					start = mid - 1;
			}

			if (start < 0 || end >= pointDistances.Count)
				return false;

			var prevDist = pointDistances[start];
			var nextDist = pointDistances[end];
			float a = 0;
			float b = (float)(nextDist - prevDist);
			float v = (float)(distance - prevDist);
			t = Mathf.InverseLerp(a, b, v);
			double p = Mathf.Lerp(prevDist, nextDist, t);
			point = Vector2.Lerp(points[start], points[end], t);
			// Debug.Log($"{p} Lies {t*100}% between [{start}]:{prevDist} and [{end}]:{nextDist}");
			return true;
		}
	}
}