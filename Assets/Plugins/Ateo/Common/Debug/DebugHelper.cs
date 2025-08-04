using UnityEngine;

namespace Ateo.Common
{
	public static class DebugHelper
	{
		public static void DrawBounds(Bounds b, float delay=0)
		{
			// bottom
			Vector3 p1 = new Vector3(b.min.x, b.min.y, b.min.z);
			Vector3 p2 = new Vector3(b.max.x, b.min.y, b.min.z);
			Vector3 p3 = new Vector3(b.max.x, b.min.y, b.max.z);
			Vector3 p4 = new Vector3(b.min.x, b.min.y, b.max.z);

			Debug.DrawLine(p1, p2, Color.blue, delay);
			Debug.DrawLine(p2, p3, Color.red, delay);
			Debug.DrawLine(p3, p4, Color.yellow, delay);
			Debug.DrawLine(p4, p1, Color.magenta, delay);

			// top
			Vector3 p5 = new Vector3(b.min.x, b.max.y, b.min.z);
			Vector3 p6 = new Vector3(b.max.x, b.max.y, b.min.z);
			Vector3 p7 = new Vector3(b.max.x, b.max.y, b.max.z);
			Vector3 p8 = new Vector3(b.min.x, b.max.y, b.max.z);

			Debug.DrawLine(p5, p6, Color.blue, delay);
			Debug.DrawLine(p6, p7, Color.red, delay);
			Debug.DrawLine(p7, p8, Color.yellow, delay);
			Debug.DrawLine(p8, p5, Color.magenta, delay);

			// sides
			Debug.DrawLine(p1, p5, Color.white, delay);
			Debug.DrawLine(p2, p6, Color.gray, delay);
			Debug.DrawLine(p3, p7, Color.green, delay);
			Debug.DrawLine(p4, p8, Color.cyan, delay);
		}
	}
}