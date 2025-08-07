using System;
using System.Collections;
using System.Collections.Generic;
using Ateo.Animation;
using Ateo.Common;
using DG.Tweening;
using FMODUnity;
using Moreno.SewingGame.Audio;
using Moreno.SewingGame.Path;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Moreno.SewingGame
{
	public class SewingMachineController : ComponentPublishBehaviour<SewingMachineController>
	{
		#region private Serialized Variables

		[BoxGroup("Audio")]
		[SerializeField, Required]
		private SewingMachineEventInstance _sewingMachineAudio;
		[SerializeField]
		[BoxGroup("Gameplay")]
		private float _fabricMaxSpeed = 1f;
		[SerializeField]
		[BoxGroup("Gameplay")]
		private float _rotationSpeed = 1f;
		[SerializeField, Required]
		[BoxGroup("Gameplay")]
		private AnimationCurve _accelerationCurve = new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1,1)});
		[SerializeField, Required]
		[BoxGroup("Gameplay")]
		private float _speedSmoothTime;
		[SerializeField, Required]
		[BoxGroup("Gameplay")]
		private float _speedSmoothTimeMaxSpeed = 100f;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private Transform _fabricParent;
		[BoxGroup("Gameplay References")]
		[SerializeField, Required]
		private Transform _fabricRotator;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private Transform _rotationCenter;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private Hurtable _needle;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private ParticleSystem _brokenParticleSystem;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private GameObject _todoMessage;
		[SerializeField, Required]
		[BoxGroup("Gameplay References")]
		private PinManager _pinManager;
		
		[SerializeField, Required]
		[BoxGroup("Foot")]
		private AnimationBehaviour _footDownAnimationBehaviour;
		[SerializeField, Required]
		[BoxGroup("Foot")]
		private AnimationBehaviour _footUpAnimationBehaviour;
		[SerializeField, Required]
		[BoxGroup("Foot")]
		private PlayOneShot _footDownSound;
		[SerializeField, Required]
		[BoxGroup("Foot")]
		private PlayOneShot _footUpSound;
		
		[SerializeField, Required]
		[BoxGroup("Needle")]
		private Transform _needleTransform;
		[SerializeField]
		[BoxGroup("Needle")]
		private Vector2 _needleTransformMinMaxY;
		[SerializeField]
		[BoxGroup("Needle")]
		private float _needleSpeed = 30f;
		[SerializeField]
		[BoxGroup("Needle")]
		private float _minNeedleAnimationTimeStep = 0.5f;

		[SerializeField, Required]
		[BoxGroup("Thread")]
		private Transform _threadHolderTransform;
		[FormerlySerializedAs("_threadHolderAnimationoffset"),SerializeField]
		[BoxGroup("Thread")]
		private float _threadHolderAnimationOffset = 0.1f;
		[SerializeField]
		[BoxGroup("Thread")]
		private Vector2 _threadHolderTransformMinMaxY;

		[SerializeField]
		[BoxGroup("Path")]
		private PathEvaluater _pathEvaluator;
		
		
		#endregion

		#region private Variables

		private float _speedVelocity;
		private float _currentSpeed;
		private float _targetSpeed;
		private int _currentKeysPressed;
		private float _needleAnimationTime = 0;
		private float _currentRotation = 0;
		private bool _footDown;
		private float _previousNormalizedNeedleAnimationTime;
		private bool _broken = false;
		private float _lastNeedleStitchTime;
		private Vector3 _fabricStartPosition;
		
		private List<KeyCode> _possiblePowerKeys = new List<KeyCode>() {KeyCode.Q, KeyCode.W, KeyCode.E};

		private List<KeyCode> _pressedKeys = new List<KeyCode>();

		#endregion

		#region Properties

		public float CurrentSpeed
		{
			get
			{
				if (!_footDown)
				{
					return 0;
				}
				return _currentSpeed;
			}
		}

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		protected override void OnStart()
		{
			_todoMessage.SetActive(false);
			_fabricStartPosition = _fabricParent.position;
		}

		private void Update()
		{
			CheckPlayerInput();
		}

		#endregion

		#region Public Methods

		public void PrepareLevel()
		{
			ResetMachine();
			_pathEvaluator.PrepareLevel();
			_pinManager.PrepareLevel();
		}

		public void BreakMachine()
		{
			StopMachine();
			_broken = true;
			_brokenParticleSystem.Play(true);
			_todoMessage.SetActive(true);
			_pathEvaluator.AccuracyTrend = 0;
			
			MoveNeedleToOutPoint();
			StartCoroutine(Routine());
			return;

			IEnumerator Routine()
			{
				yield return new WaitForSeconds(5);
				_broken = false;
				_brokenParticleSystem.Stop(true,ParticleSystemStopBehavior.StopEmitting);
				_todoMessage.SetActive(false);
			}
		}

		public void ResetMachine()
		{
			SetFootState(false);
			_fabricParent.position = _fabricStartPosition;
			_fabricParent.rotation = Quaternion.identity;
			_pathEvaluator.ResetValues();
		}

		public void StopMachine()
		{
			_sewingMachineAudio.StopSound();
		}

		public void AddAcceleration(float value)
		{
			EvaluateCurrentSpeed(value > 0);
			UpdateAudioSpeed();
		}
		
		public void MoveNeedleToOutPoint()
		{
			StartCoroutine(Routine());
			return;
			
			IEnumerator Routine()
			{
				while (_previousNormalizedNeedleAnimationTime < 0.99f)
				{
					_needleAnimationTime += 0.1f;
					AnimateNeedle();
					AnimateThreadHolder();
					yield return null;
				}
			}
		}

		#endregion

		#region Private Methods

		private bool GetPowerKeys()
		{
			int previousKeyCount = _pressedKeys.Count;
			foreach (KeyCode key in _possiblePowerKeys)
			{
				if (Input.GetKeyDown(key))
				{
					if(!_pressedKeys.Contains(key))
						_pressedKeys.Add(key);
				}

				if (Input.GetKeyUp(key))
				{
					if(_pressedKeys.Contains(key))
						_pressedKeys.Remove(key);
				}
			}

			int currentKeyCount = _pressedKeys.Count;
			if (_broken) currentKeyCount = 0;

			if (previousKeyCount != currentKeyCount)
			{
				//count change
				if (currentKeyCount == 0)
				{
					_sewingMachineAudio.StopSound();
				}

				if (previousKeyCount == 0)
				{
					_sewingMachineAudio.StartSound();
				}
			}

			if (_broken) return false;
			_currentKeysPressed = currentKeyCount;
			return currentKeyCount > 0;
		}

		private void CheckPlayerInput()
		{
			bool gasDown = GetPowerKeys();

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				ToggleFootState();
			}

			EvaluateCurrentSpeed(gasDown);
			if (gasDown)
			{
				_needleAnimationTime += Mathf.Abs(_needleSpeed * _currentSpeed) * Time.deltaTime;
			}
			
			MoveFabricForward();
			AnimateNeedle();
			AnimateThreadHolder();
			UpdateAudioSpeed();

			if(!MouseWorldPointer.Instance.PressedDownOnFabric) return;
			float rotationDirection = MouseWorldPointer.Instance.TryDetectMouseDragDirection();
			RotateFabric(rotationDirection);
		}

		private void RotateFabric(float direction)
		{
			float speed = _footDown ? _currentSpeed : -1;
			_fabricRotator.Rotate(Vector3.up,direction*speed*_rotationSpeed);
		}

		private void MoveFabricForward()
		{
			if (_currentKeysPressed <= 0) return;
			_fabricParent.position += _rotationCenter.forward * (CurrentSpeed * _fabricMaxSpeed * Time.deltaTime);
		}

		private void EvaluateCurrentSpeed(bool gasDown)
		{
			_targetSpeed = gasDown 
				? -_accelerationCurve.Evaluate(_currentKeysPressed) 
				: 0;
			_currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _speedVelocity, _speedSmoothTime, _speedSmoothTimeMaxSpeed);
		}

		private float GetNormalizedSinValue(float time)
		{
			float sinHeldTime = Mathf.Sin(time);
			return (sinHeldTime * 0.5f) + 0.5f;
		}

		private void AnimateNeedle()
		{
			AnimateBetweenPoints(_needleAnimationTime,_needleTransform,_needleTransformMinMaxY, out var normalized);
			if (Mathf.Abs(_previousNormalizedNeedleAnimationTime - normalized) > 0.001f)
			{
				if (normalized <= 0.2f)
				{
					if (_needleAnimationTime - _minNeedleAnimationTimeStep > _lastNeedleStitchTime)
					{
						_lastNeedleStitchTime = _needleAnimationTime;
						if (_footDown)
						{
							OnNeedleStitch();
						}
					}
				}
			}
			_previousNormalizedNeedleAnimationTime = normalized;
		}

		private void AnimateThreadHolder()
		{
			float time = _needleAnimationTime + _threadHolderAnimationOffset;
			AnimateBetweenPoints(time, _threadHolderTransform, _threadHolderTransformMinMaxY,out _);
		}

		private void AnimateBetweenPoints(float time, Transform target, Vector2 localYBounds, out float normalizedTime)
		{
			normalizedTime = GetNormalizedSinValue(time);
			SetAnimationBetweenBounds(normalizedTime, target, localYBounds);
		}

		private void SetAnimationBetweenBounds(float lerp,Transform target, Vector2 localYBounds)
		{
			var vector3 = target.localPosition;
			float y = Mathf.Lerp(localYBounds.x, localYBounds.y, lerp);
			vector3.y = y;
			target.localPosition = vector3;
		}

		private void ToggleFootState()
		{
			_footDown = !_footDown;
			UpdateFootAnimation();
		}

		private void SetFootState(bool value, bool immediate = false)
		{
			_footDown = value;
			UpdateFootAnimation(immediate);
		}

		private void UpdateFootAnimation(bool immediate = false)
		{
			if (immediate)
			{
				if (_footDown)
				{
					_footDownAnimationBehaviour.ExecuteAnimationImmediate();
				}
				else
				{
					_footUpAnimationBehaviour.ExecuteAnimationImmediate();
				}
				return;
			}
			
			if (_footDown)
			{
				_footDownAnimationBehaviour.Execute(false);
				_footDownSound.PlayEvent();
			}
			else
			{
				_footUpAnimationBehaviour.Execute(false);
				_footUpSound.PlayEvent();
			}
		}

		private void UpdateAudioSpeed()
		{
			_sewingMachineAudio.SetSpeed(_currentSpeed);
		}
		
		private void OnNeedleStitch()
		{
			_needle.CheckIfMouseIsOverHurtable();
			_pathEvaluator.CheckWorldPointPathAccuracy(_rotationCenter.transform.position);
		}

		#endregion

		#region Event Callbacks

		#endregion
	}
}