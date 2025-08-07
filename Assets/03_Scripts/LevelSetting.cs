using System;
using Moreno.SewingGame.Path;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Moreno.SewingGame
{
	[CreateAssetMenu(fileName = "Level Setting", menuName = "Sewing/Create Level Setting")]
	public class LevelSetting : ScriptableObject
	{
		[SerializeField]
		private float _machineDirectionRange;
		[SerializeField]
		private float _machineDirectionChangeRange;

		[SerializeField]
		private PathData _pathData;
		[SerializeField]
		private bool _spawnPins = false;
		[SerializeField, ShowIf(nameof(_spawnPins))]
		private Vector2 _pinSpawnDistanceRange = new Vector2(10f,15f);
		[SerializeField, ShowIf(nameof(_spawnPins))]
		private bool _canPinsFlip = false;
		[SerializeField, ShowIf(nameof(_showPinRotationOption)), Range(0f,90f)]
		private float _pinRandomRotationRange;

		public float MachineDirectionRange => _machineDirectionRange;

		public float MachineDirectionChangeRange => _machineDirectionChangeRange;

		public bool SpawnPins => _spawnPins;

		public float PinSpawnDistance => Random.Range(_pinSpawnDistanceRange.x, _pinSpawnDistanceRange.y);

		public bool CanPinsFlip => _canPinsFlip;

		public float PinRandomRotationRange => _pinRandomRotationRange;

		public PathData PathData => _pathData;

		private bool _showPinRotationOption => _spawnPins && _canPinsFlip;
	}
}