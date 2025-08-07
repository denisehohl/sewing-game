using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moreno.SewingGame.Path
{
	[CreateAssetMenu(fileName = "Path", menuName = "Sewing/Create Path", order = 0)]
	public class PathData : ScriptableObject
	{
		[SerializeField]
		private List<Vector2> _points = new List<Vector2>();

		[SerializeField]
		private List<float> _distances = new List<float>();

		public List<Vector2> Points
		{
			get => _points;
			set => _points = value;
		}

		public List<float> Distances => _distances;

		public float PathLength => _distances[^1];

		[Button]
		public void CalculateDistances()
		{
			float distance = 0;
			Vector2 start = _points[0];
			_distances.Clear();
			_distances.Add(0);
			for (int i = 1; i < _points.Count; i++)
			{
				var end = _points[i];
				distance += Vector2.Distance(start, end);
				_distances.Add(distance);
				start = end;
			}
			
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}
	}
}