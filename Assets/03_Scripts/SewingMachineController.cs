using System;
using Ateo.Animation;
using Ateo.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moreno.SewingGame
{
	public class SewingMachineController : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField, Required]
		private Transform _fabricParent;

		[SerializeField, Required]
		private Transform _rotationCenter;
		
		[SerializeField, Required]
		private AnimationBehaviour _footDownAnimationBehaviour;
		[SerializeField, Required]
		private AnimationBehaviour _footUpAnimationBehaviour;
		[SerializeField, Required]
		private Transform _needleTransform;
		[SerializeField]
		private Vector2 _needleTransformMinMaxY;

		[SerializeField]
		private float _maxSpeed = 1f;
		[SerializeField]
		private float _rotationSpeed = 1f;

		[SerializeField]
		private float _needleSpeed = 30f;
		
		[SerializeField]
		private AnimationCurve _accelerationCurve = new AnimationCurve(new []{new Keyframe(0,0), new Keyframe(1,1)});
		[SerializeField]
		private AnimationCurve _decelerationCurve = new AnimationCurve(new []{new Keyframe(0,1), new Keyframe(1,0)});

		
		#endregion

		#region private Variables

		private float _currentSpeed = 0; 
		private float _currentHeldDownTime = 0;
		private float _needleAnimationTime = 0;
		private float _currentRotation = 0;
		private bool _footDownState;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void Update()
		{
			CheckPlayerInput();
		}

		#endregion

		#region Public Methods

		public void ResetMachine()
		{
			SetFootState(false);
		}

		public void StopMachine()
		{
			_currentHeldDownTime = 0;
		}

		public void AddAcceleration(float value)
		{
			_currentHeldDownTime += value;
			EvaluateCurrentSpeed(value > 0);
		}

		#endregion

		#region Private Methods

		private void CheckPlayerInput()
		{
			bool gasDown = Input.GetKey(KeyCode.W);

			if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.W))
			{
				_currentHeldDownTime = 0;
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				ToggleFootState();
			}

			if (gasDown)
			{
				_currentHeldDownTime += Time.deltaTime;
				_needleAnimationTime += _needleSpeed * _currentSpeed * Time.deltaTime;
			}
			EvaluateCurrentSpeed(gasDown);
			MoveFabricForward();
			AnimateNeedle();
			
			if(!gasDown) return;
			if(MouseWorldPointer.Instance == null) return;

			float rotationDirection = MouseWorldPointer.Instance.TryDetectMouseDragDirection();
			RotateFabric(rotationDirection);
		}

		private void RotateFabric(float direction)
		{
			//_fabricParent.RotateAround(_rotationCenter.position, Vector3.up, direction * _currentSpeed);
			_fabricParent.RotateAroundCustomPivot(
				_fabricParent.InverseTransformPoint(MouseWorldPointer.Instance.CurrentPosition),
				_rotationCenter.position,
				Vector3.up,
				direction * _currentSpeed * _rotationSpeed);
		}

		private void MoveFabricForward()
		{
			if (_currentSpeed == 0) return;
			_fabricParent.position += _rotationCenter.forward * (_currentSpeed * _maxSpeed);
		}

		private void EvaluateCurrentSpeed(bool gasDown)
		{
			_currentSpeed = gasDown 
				? -_accelerationCurve.Evaluate(_currentHeldDownTime) 
				: 0;
		}

		private void AnimateNeedle()
		{
			float sinHeldTime = Mathf.Sin(_needleAnimationTime);
			float normalizedHeldTime = (sinHeldTime * 0.5f) + 0.5f;
			var vector3 = _needleTransform.localPosition;
			float y = Mathf.Lerp(_needleTransformMinMaxY.x, _needleTransformMinMaxY.y, normalizedHeldTime);
			vector3.y = y;
			_needleTransform.localPosition = vector3;
		}

		private void ToggleFootState()
		{
			_footDownState = !_footDownState;
			UpdateFootAnimation();
		}

		private void SetFootState(bool value, bool immediate = false)
		{
			_footDownState = value;
			UpdateFootAnimation(immediate);
		}

		private void UpdateFootAnimation(bool immediate = false)
		{
			if (immediate)
			{
				if (_footDownState)
				{
					_footDownAnimationBehaviour.ExecuteAnimationImmediate();
				}
				else
				{
					_footUpAnimationBehaviour.ExecuteAnimationImmediate();
				}
				return;
			}
			
			if (_footDownState)
			{
				_footDownAnimationBehaviour.Execute(false);
			}
			else
			{
				_footUpAnimationBehaviour.Execute(false);
			}
		}

		#endregion

		#region Event Callbacks

		#endregion
	}
}