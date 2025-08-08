using System;
using Ateo.Common;
using Ateo.Extensions;
using Ateo.StateManagement;
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
		private bool _pressedDownOnFabric = false;
		private GameObject _currentInteractionObject;
		private Plane _backupPlane;
		#endregion

		#region Properties

		public Vector3 DeltaPosition => _deltaPosition;

		public Vector3 CurrentPosition => _currentPosition;

		public GameObject CurrentInteractionObject => _currentInteractionObject;

		public Vector3 MousePressedStartWorldPosition => _mousePressedStartWorldPosition;

		public bool MousePressed => _mousePressed;

		public bool PressedDownOnFabric => _pressedDownOnFabric;

		#endregion

		#region Delegates & Events

		public static  event Action<GameObject> OnObjectClicked;
		public static event Action OnMouseRelease; 
		public static  event Action<GameObject> OnInteractableEntered;

		#endregion

		#region Monobehaviour Callbacks

		protected override void OnStart()
		{
			_backupPlane = new Plane();
		}

		protected override void OnPublish()
		{
			InGame.Instance.OnStart += OnInGameStarted;
		}

		protected override void OnWithdraw()
		{
			InGame.Instance.OnStart -= OnInGameStarted;
			
		}

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
			GameObject previousObject = _currentInteractionObject;
			var ray = GetCameraRay();
			if (TryMouseRaycast(ray, out RaycastHit hit))
			{
				SetCurrentPosition(hit.point);

				if (StateManager.CurrentEnum != StatesEnum.InGame)
				{
					return;
				}
				var obj = hit.collider.gameObject;
				_currentInteractionObject = obj;
				bool isFabric = _fabricLayer.Contains(obj.layer);
				bool isInteractable = _interactableLayer.Contains(obj.layer);
				
				if (Input.GetMouseButtonDown(0))
				{
					_mousePressed = true;
					_mousePressedStartWorldPosition = hit.point;

					_pressedDownOnFabric = isFabric;
					PositionPlane(_currentInteractionObject.transform);

					OnObjectClicked?.Invoke(_currentInteractionObject);
				}

				if (isInteractable && previousObject != _currentInteractionObject)
				{
					OnInteractableEntered?.Invoke(_currentInteractionObject);
				}
			}
			else
			{
				if (_pressedDownOnFabric)
				{
					if(_backupPlane.Raycast(ray, out var enter))
					{
						Vector3 hitPoint = ray.GetPoint(enter);
						SetCurrentPosition(hitPoint);
					}
				}
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				_mousePressed = false;
				_pressedDownOnFabric = false;
				OnMouseRelease?.Invoke();
			}


		}

		private void SetCurrentPosition(Vector3 position)
		{
			_currentPosition = position;
			_pointer.position = position;
			
			_deltaPosition = _currentPosition - _previousPosition;
		}

		private Ray GetCameraRay()
		{
			return Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		public bool TryMouseRaycast(Ray ray, out RaycastHit info)
		{
			return Physics.Raycast(ray, out info, 20f);
		}

		private void PositionPlane(Transform target)
		{
			_backupPlane.SetNormalAndPosition(target.up,target.position);
		}

		#endregion

		#region Event Callbacks
		
		private void OnInGameStarted()
		{
			_pressedDownOnFabric = false;
		}

		#endregion

		
	}
}