using System;
using System.Collections.Generic;
using Moreno.SewingGame.Path;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Moreno.SewingGame
{
	public class PinManager : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField, Required]
		private Pin _pinPrefab;

		[SerializeField, Required]
		private Transform _pinParent;

		[SerializeField, Required]
		private PathEvaluater _pathEvaluater;

		#endregion

		#region private Variables

		private List<Pin> _instances = new List<Pin>();
		private LevelSetting _currentLevel;

		#endregion

		#region Public Methods

		public void PrepareLevel()
		{
			_currentLevel = Context.CurrentLevel;
			RemoveAllPins();
			if (_currentLevel == null) return;
			if (!_currentLevel.SpawnPins) return;

			for (float distance = _currentLevel.PinSpawnDistance;
			     distance < _currentLevel.PathData.PathLength;
			     distance += _currentLevel.PinSpawnDistance)
			{
				AddPinAtPathDistance(distance);
			}
		}

		public void RemoveAllPins()
		{
			for (int i = _instances.Count - 1; i >= 0; i--)
			{
				var instance = _instances[i];
				if (instance != null)
				{
					Destroy(instance.gameObject);
				}
			}

			_instances.Clear();
		}

		#endregion

		#region Private Methods

		private void AddPinAtPathDistance(float distance)
		{
			if (_pathEvaluater.TryGetWorldPositionFromPathDistance(distance, out var worldPosition, out Vector3 pathDirection))
			{
				InstantiatePin(_currentLevel, worldPosition, pathDirection);
			}
		}

		private void InstantiatePin(LevelSetting setting, Vector3 worldPosition, Vector3 pathDirection)
		{
			var instance = Instantiate(_pinPrefab, _pinParent);
			var t = instance.transform;
			t.position = worldPosition + new Vector3(0, 0.01f, 0);

			if (setting.LineWidth >= 1f)
			{
				Vector3 positionOffset = Quaternion.AngleAxis(90f, Vector3.up) * pathDirection;
				positionOffset *= Random.Range(0f, setting.LineWidth * 2f);
				positionOffset *= Random.Range(0, 2) == 1 ? 1f : -1f;
				t.position += positionOffset;
			}

			var randomYRotation = Random.Range(-1f, 1f) * setting.PinRandomRotationRange;
			if (setting.CanPinsFlip)
			{
				randomYRotation += Random.Range(0, 2) == 1 ? 0 : 180f;
			}

			// Rotation towards/along the path direction. 
			var r = Quaternion.LookRotation(pathDirection);
			// Rotate by 90° and add the random y rotation.
			r *= Quaternion.Euler(0, 90 + randomYRotation, 0);
			// Apply the final rotation.
			t.rotation = r;

			_instances.Add(instance);
		}

		#endregion

		#region Event Callbacks

		#endregion
	}
}