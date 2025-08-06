using System.Collections.Generic;
using UnityEngine;

namespace Moreno.SewingGame.Path
{
	[CreateAssetMenu(fileName = "Path", menuName = "Sewing/Create Path", order = 0)]
	public class PathData : ScriptableObject
	{
		[SerializeField]
		private List<Vector2> _points;

		public List<Vector2> Points
		{
			get => _points;
			set => _points = value;
		}
	}
}