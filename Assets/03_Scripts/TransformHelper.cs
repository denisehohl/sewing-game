using UnityEngine;

namespace Moreno.SewingGame
{
	public static class TransformHelper
	{
		/// <summary>
		/// Rotates a target transform around a fixed point using a custom pivot (grab point).
		/// </summary>
		/// <param name="target">The transform to rotate.</param>
		/// <param name="pivot">The grab point (local to the target transform).</param>
		/// <param name="anchor">The fixed world-space point to rotate around.</param>
		/// <param name="axis">The world-space axis to rotate around.</param>
		/// <param name="angleDegrees">The rotation angle in degrees.</param>
		public static void RotateAroundCustomPivot(this Transform target, Vector3 pivot, Vector3 anchor, Vector3 axis, float angleDegrees)
		{
			// Convert local grab point (pivot) to world space
			Vector3 pivotWorldPos = target.TransformPoint(pivot);

			// Calculate offset between anchor and pivot
			Vector3 pivotToAnchor = pivotWorldPos - anchor;

			// Rotate the offset around the anchor
			Quaternion rotation = Quaternion.AngleAxis(angleDegrees, axis);
			Vector3 rotatedOffset = rotation * pivotToAnchor;

			// Compute the new position of the pivot
			Vector3 newPivotWorldPos = anchor + rotatedOffset;

			// Calculate how much the target needs to move so that the pivot stays aligned
			Vector3 delta = newPivotWorldPos - pivotWorldPos;

			// Move and rotate the target
			target.position += delta;
			target.RotateAround(pivotWorldPos, axis, angleDegrees);
		}
		
	}
}