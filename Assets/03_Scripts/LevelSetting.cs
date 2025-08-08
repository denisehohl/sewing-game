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
		private bool _isTutorial;

		[SerializeField, BoxGroup("Scoring")]
		private float _perfectAccuracyRange = 5;
		[SerializeField, BoxGroup("Scoring")]
		private float _imperfectAccuracyRange = 50;

		[SerializeField, BoxGroup("Scoring")]
		private float _perfectDamageTaken = 0;
		[SerializeField, BoxGroup("Scoring")]
		private float _imperfectDamageTaken = 20;
		
		[SerializeField]
		private float _machineDirectionRange;
		[SerializeField]
		private float _machineDirectionChangeRange;

		[SerializeField]
		private PathData _pathData;

		[SerializeField]
		private AnimationCurve _accuracyScorePerDistance;
		
		[SerializeField]
		private bool _spawnPins = false;
		[SerializeField, ShowIf(nameof(_spawnPins))]
		private Vector2 _pinSpawnDistanceRange = new Vector2(10f,15f);
		[SerializeField, ShowIf(nameof(_spawnPins))]
		private bool _canPinsFlip = false;
		[SerializeField, ShowIf(nameof(_showPinRotationOption)), Range(0f,90f)]
		private float _pinRandomRotationRange;
		[SerializeField, Range(0f,10f)]
		private float _lineWidth = 0.01f;

		[SerializeField]
		private Vector2 _threadingRandomRange;
		[SerializeField]
		private float _threadingSmoothTime;
		[SerializeField]
		private float _threadingMaxSmoothSpeed;

		public bool IsTutorial => _isTutorial;

		public string LevelName => name;
		public float MachineDirectionRange => _machineDirectionRange;

		public float MachineDirectionChangeRange => _machineDirectionChangeRange;

		public bool SpawnPins => _spawnPins;

		public float PinSpawnDistance => Random.Range(_pinSpawnDistanceRange.x, _pinSpawnDistanceRange.y);

		public bool CanPinsFlip => _canPinsFlip;

		public float PinRandomRotationRange => _pinRandomRotationRange;
		public float LineWidth => _lineWidth;

		public PathData PathData => _pathData;

		private bool _showPinRotationOption => _spawnPins && _canPinsFlip;

		public Vector2 ThreadingRandomRange => _threadingRandomRange;

		public float ThreadingSmoothTime => _threadingSmoothTime;

		public float ThreadingMaxSmoothSpeed => _threadingMaxSmoothSpeed;

		public float GetAccuracyScoreForDistanceToPath(float distance)
		{
			return _accuracyScorePerDistance.Evaluate(distance);
		}
		
		public float GetAccuracyPercentage(float accuracy)
		{
			return Mathf.Clamp01(Mathf.InverseLerp(_imperfectAccuracyRange, _perfectAccuracyRange, accuracy)) * 100f;
		}
		
		public float GetCleanPercentage(float accuracy)
		{
			return  Mathf.Clamp01(Mathf.InverseLerp(_imperfectDamageTaken, _perfectDamageTaken, accuracy)) * 100f;
		}
	}
}