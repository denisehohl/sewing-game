using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moreno.SewingGame.Path
{
	public class PathEvaluater : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField, Required]
		private LineRenderer _pathVisualizer;
		[SerializeField, Required]
		private LineRenderer _playerPathVisualizer;

		#endregion

		#region private Variables

		private PathData _currentPath;
		private float _accumulatedDistanceOffset;
		private float _accuracyTrend;

		#endregion

		#region Properties

		public float AccumulatedDistanceOffset => _accumulatedDistanceOffset;

		public float AccuracyTrend
		{
			get => _accuracyTrend;
			set
			{
				var clamped = Mathf.Clamp(value, 0, 1f);
				if(clamped == _accuracyTrend) return;
				_accuracyTrend = clamped;
				OnAccuracyChanged?.Invoke(clamped);
			}
		}

		#endregion

		#region Delegates & Events

		public static event Action<float> OnAccuracyChanged; 

		#endregion

		#region Monobehaviour Callbacks

		private void OnEnable()
		{
			DamageManager.OnDamageTaken += OnDamageTaken;
		}

		private void OnDisable()
		{
			DamageManager.OnDamageTaken -= OnDamageTaken;
		}

		private void OnDestroy()
		{
			OnAccuracyChanged = null;
		}

		private void OnDamageTaken(float damage, float intensity)
		{
			AccuracyTrend -= intensity;
		}

		#endregion

		#region Public Methods

		public void PrepareLevel()
		{
			ResetValues();
			_currentPath = Context.CurrentLevel.PathData;
			UpdatePathVisual(_currentPath);
		}

		public void ResetValues()
		{
			_currentPath = null;
			_accumulatedDistanceOffset = 0;
			_accuracyTrend = 0;
			ResetPlayerPathVisualizer();
		}

		public void CheckWorldPointPathAccuracy(Vector3 positionToCheck)
		{
			if(_currentPath == null) return;
			Vector2 localPoint = TranslateWorldToPathPoint(positionToCheck);
			float distance = _currentPath.Points.GetDistanceToClosestPointOnPath(localPoint, out var pointOnTrack);
			
			AddPointToPlayerPath(localPoint);

			AccuracyTrend += Context.CurrentLevel.GetAccuracyScoreForDistanceToPath(distance);
			
			_accumulatedDistanceOffset += distance;
		}

		public bool TryGetWorldPositionFromPathDistance(float distance, out Vector3 worldPoint)
		{
			if (_currentPath.TryGetPointAtDistance(distance, out var pathPoint))
			{
				worldPoint = TranslatePathPointToWorldPosition(pathPoint);
				return true;
			}
			worldPoint = Vector3.zero;
			return false;
		}

		#endregion

		#region Private Methods

		private void AddPointToPlayerPath(Vector2 localPoint)
		{
			int count = _playerPathVisualizer.positionCount++;
			_playerPathVisualizer.SetPosition(count,localPoint);
		}

		private void ResetPlayerPathVisualizer()
		{
			_playerPathVisualizer.positionCount = 1;
			AddPointToPlayerPath(Vector2.zero);
		}

		private Vector2 TranslateWorldToPathPoint(Vector3 worldPosition)
		{
			Vector3 point = _pathVisualizer.transform.InverseTransformPoint(worldPosition);
			return new Vector2(point.x, point.y);
		}

		private Vector3 TranslatePathPointToWorldPosition(Vector2 pathPoint)
		{
			return _pathVisualizer.transform.TransformPoint(pathPoint);
		}

		private void UpdatePathVisual(PathData data)
		{
			_pathVisualizer.gameObject.SetActive(data != null);
			if (data == null) return;
			var points = data.Points;
			var count = points.Count;
			var convertedList = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				var point = points[i];
				convertedList[i] = new Vector3(point.x, point.y, 0);
			}

			_pathVisualizer.positionCount = count;
			_pathVisualizer.SetPositions(convertedList);
			_pathVisualizer.startWidth = Context.CurrentLevel.LineWidth;
		}

		#endregion

		#region Helper Functions

		[Button]
		private void StorePathAsPathData(PathData target)
		{
			var pathBuffer = new Vector3[_pathVisualizer.positionCount];
			_pathVisualizer.GetPositions(pathBuffer);
			var finalList = new List<Vector2>();

			foreach (Vector3 v in pathBuffer)
			{
				finalList.Add(new Vector2(v.x,v.y));
			}

			target.Points = finalList;
			
			target.CalculateDistances();

#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(target);
			UnityEditor.AssetDatabase.SaveAssets();
#endif
		}

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}