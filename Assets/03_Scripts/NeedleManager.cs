using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Moreno.SewingGame.Audio;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Moreno.SewingGame
{
	public class NeedleManager : MonoBehaviour
	{
		#region private Serialized Variables
		
		[SerializeField, Required]
		[BoxGroup("Needle")]
		private GameObject _intactNeedle;
		[SerializeField, Required]
		[BoxGroup("Needle")]
		private Interactable _brokenNeedle;

		[SerializeField, Required]
		private List<GameObject> _objectsToDisableWhenAnimating;

		[SerializeField, Required]
		private CinemachineCamera _closeupCam;
		
		[SerializeField]
		private float _animationDistanceOffset = 0.1f;
		[SerializeField]
		private float _animationJumpPower = -0.1f;
		[SerializeField]
		private float _animationTime = 1f;

		[SerializeField]
		private Transform _holeTarget;
		[SerializeField]
		private Transform _threadTarget;
		[SerializeField]
		private Transform _threadVisualTarget;
		[SerializeField]
		private Vector2 _maxThreadYRange;
		[SerializeField]
		private float _threadYAcceptableDistance = 0.1f;
		[SerializeField]
		private float _threadYInputSpeed = 0.2f;
		
		
		[SerializeField]
		private EventReference _threadWhirl;
		[SerializeField]
		private EventReference _threadPressed;

		#endregion

		#region private Variables

		private Vector3 _targetLocalPos;
		private Vector3 _threadVisualTargetLocalPos;
		private bool _inThreadingMinigame;
		private float _playerYThreadPos;
		private float _currentYThreadPos;
		private EventInstance _threadInstance;
		private PARAMETER_ID _intensityId;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void Start()
		{
			StoreStartLocalPosition();
			_closeupCam.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			MainManager.OnLevelStarted += OnLevelStart;
		}

		private void OnDisable()
		{
			MainManager.OnLevelStarted -= OnLevelStart;
		}

		#endregion

		#region Public Methods
		
		public void SetNeedleBrokenVisual(bool isBroken)
		{
			_brokenNeedle.transform.DOKill();
			_intactNeedle.transform.DOKill();
			SetObjectsInactiveWhileAnimation(false);
			_brokenNeedle.gameObject.SetActive(isBroken);
			_brokenNeedle.transform.localPosition = _targetLocalPos;
			_intactNeedle.transform.localPosition = _targetLocalPos;
			_intactNeedle.SetActive(!isBroken);
		}

		public IEnumerator StartReplaceTask(Action callback = null)
		{
			bool needleReplaced = !_brokenNeedle.gameObject.activeSelf;
			_brokenNeedle.OnClicked += ()=> needleReplaced = true;
			_closeupCam.gameObject.SetActive(true);

			while (!needleReplaced)
			{
				yield return null;
			}
			
			_brokenNeedle.ClearListeners();
			SwapNewNeedleIn();

			yield return StartThreadTask();
			
			_closeupCam.gameObject.SetActive(false);
			callback?.Invoke();
		}

		#endregion

		#region Private Methods

		private IEnumerator StartThreadTask()
		{
			yield return new WaitForSeconds(2f);
			InitThreadMinigame();
			while (_inThreadingMinigame)
			{
				UpdateThreadMinigame();
				yield return null;
			}

			yield return new WaitForSeconds(1f);
			_threadTarget.gameObject.SetActive(false);
		}

		private void SwapNewNeedleIn()
		{
			float offsetStrength = _animationDistanceOffset;
			_brokenNeedle.gameObject.SetActive(true);
			_intactNeedle.gameObject.SetActive(true);
			SetObjectsInactiveWhileAnimation(true);
			_brokenNeedle.transform.DOLocalJump( _targetLocalPos + Vector3.left * offsetStrength, _animationJumpPower, 1, _animationTime)
				.OnComplete(()=>_brokenNeedle.gameObject.SetActive(false));
			_intactNeedle.transform.localPosition = _targetLocalPos + Vector3.right * offsetStrength;
			_intactNeedle.transform.DOLocalJump(_targetLocalPos, _animationJumpPower, 1, _animationTime)
				.SetDelay(_animationTime * 0.5f)
				.OnComplete(()=> SetObjectsInactiveWhileAnimation(false));
		}

		private void StoreStartLocalPosition()
		{
			_targetLocalPos = _intactNeedle.transform.localPosition;
			_threadVisualTargetLocalPos = _threadVisualTarget.localPosition;
		}

		private void SetObjectsInactiveWhileAnimation(bool isAnimating)
		{
			foreach (GameObject obj in _objectsToDisableWhenAnimating)
			{
				obj.SetActive(!isAnimating);
			}
		}

		private float _threadYVelocity;
		private void UpdateThreadMinigame()
		{
			var level = Context.CurrentLevel;
			_playerYThreadPos += Input.mousePositionDelta.y * _threadYInputSpeed;
			_playerYThreadPos = Mathf.Clamp(_playerYThreadPos, _maxThreadYRange.x, _maxThreadYRange.y);
			var randomNoise = Random.Range(level.ThreadingRandomRange.x, level.ThreadingRandomRange.y);
			_currentYThreadPos = Mathf.SmoothDamp(
				_currentYThreadPos, 
				Mathf.Clamp(_playerYThreadPos + randomNoise,_maxThreadYRange.x, _maxThreadYRange.y),
				ref _threadYVelocity, 
				level.ThreadingSmoothTime,
				level.ThreadingMaxSmoothSpeed);

			SetThreadYPos(_currentYThreadPos);
			
			float targetY = _holeTarget.position.y;
			float threadY = _threadTarget.position.y;
			float distance = Math.Abs(targetY - threadY);
			
			SetAudioIntensityByDistance(distance);
			
			if (Input.GetMouseButtonDown(0))
			{
				_threadVisualTarget.DOLocalMoveZ(_threadVisualTarget.localPosition.z - 0.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
				RuntimeManager.PlayOneShot(_threadPressed);
				if (Math.Abs(targetY - threadY) < _threadYAcceptableDistance)
				{
					OnThreadMinigameSuccess();
				}
			}
		}

		private void OnThreadMinigameSuccess()
		{
			ReleaseThreadSound();
			_threadVisualTarget.DOLocalMoveZ(_threadVisualTarget.localPosition.z - 0.2f, 1f).SetLoops(2, LoopType.Yoyo);
			_inThreadingMinigame = false;
		}

		private void InitThreadMinigame()
		{
			_inThreadingMinigame = true;
			_threadTarget.gameObject.SetActive(true);
			_threadVisualTarget.localPosition = _threadVisualTargetLocalPos;
			StartThreadLoopSound();
		}

		private void SetAudioIntensityByDistance(float distance)
		{
			float intensity = distance /  Mathf.Max(Mathf.Abs(_maxThreadYRange.x), Mathf.Abs(_maxThreadYRange.y));
			_threadInstance.setParameterByID(_intensityId, Mathf.Lerp(1, 0, intensity));
		}

		private void SetThreadYPos(float y)
		{
			Vector3 offset = new Vector3(0, y, 0);
			_threadTarget.position = _holeTarget.position + offset;
		}

		private void StartThreadLoopSound()
		{
			if(_threadInstance.isValid()) return;
			_threadInstance = RuntimeManager.CreateInstance(_threadWhirl);
			_intensityId = _threadInstance.GetParameterId("Intensity");
			_threadInstance.StartAttached(_threadTarget);
		}

		private void ReleaseThreadSound()
		{
			_threadInstance.stop(STOP_MODE.IMMEDIATE);
			_threadInstance.release();
		}

		#endregion

		#region Event Callbacks
		
		private void OnLevelStart()
		{
			_threadTarget.gameObject.SetActive(false);
		}

		#endregion

		
	}
}