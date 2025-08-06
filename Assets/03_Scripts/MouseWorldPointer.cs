using System;
using Ateo.Common;
using Ateo.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class MouseWorldPointer : ComponentPublishBehaviour<MouseWorldPointer>
	{
		#region private Serialized Variables
		
		[SerializeField]
		private LayerMask _fabricLayer;

		[SerializeField]
		private LayerMask _interactableLayer;

		[SerializeField,Required]
		private Transform _pointer;

		#endregion

		#region private Variables

		private Vector3 _mousePressedStartWorldPosition;
		private Vector3 _currentPosition, _previousPosition, _deltaPosition;
		private bool _mousePressed = true;
		private GameObject _currentInteractionObject;
		#endregion

		#region Properties

		public Vector3 DeltaPosition => _deltaPosition;

		public Vector3 CurrentPosition => _currentPosition;

		public GameObject CurrentInteractionObject => _currentInteractionObject;

		public Vector3 MousePressedStartWorldPosition => _mousePressedStartWorldPosition;

		public bool MousePressed => _mousePressed;

		#endregion

		#region Delegates & Events

		public static  event Action<GameObject> OnObjectClicked;
		public static  event Action<GameObject> OnInteractableEntered;

		#endregion

		#region Monobehaviour Callbacks

		private void Update()
		{
			TryDetectMouseInput();
		}

		public override void ResetStatics()
		{
			OnObjectClicked = null;
			OnInteractableEntered = null;
			base.ResetStatics();
		}

		#endregion

		#region Public Methods
		
		public float TryDetectMouseDragDirection()
		{
			if (!_mousePressed) return 0;
			var delta = DeltaPosition;
			var dot = Vector3.Dot(delta.normalized, Vector3.left) * delta.magnitude;
			Debug.DrawRay(_currentPosition,delta,Color.aquamarine,0.1f,false);
			return dot;
		}

		#endregion

		#region Private Methods
		

		private void TryDetectMouseInput()
		{
			_previousPosition = _currentPosition;
			GameObject _previousObject = _currentInteractionObject;
			if (TryMouseRaycast(out RaycastHit hit))
			{
				_currentPosition = hit.point;
				_pointer.position = _currentPosition;
				var obj = hit.collider.gameObject;
				_currentInteractionObject = obj;
				bool isFabric = _fabricLayer.Contains(obj.layer);
				bool isInteractable = _interactableLayer.Contains(obj.layer);
				
				if (Input.GetMouseButtonDown(0))
				{
					_mousePressed = true;
					_mousePressedStartWorldPosition = hit.point;

					OnObjectClicked?.Invoke(_currentInteractionObject);
				}

				if (isInteractable && _previousObject != _currentInteractionObject)
				{
					OnInteractableEntered?.Invoke(_currentInteractionObject);
				}
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				_mousePressed = false;
			}

			_deltaPosition = _currentPosition - _previousPosition;

		}

		public bool TryMouseRaycast(out RaycastHit info)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return Physics.Raycast(ray, out info, 10f);
		}

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}