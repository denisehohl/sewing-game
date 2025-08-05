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
		private Vector3 _currentPosition, _previousPosition;
		private bool _mousePressed = true;
		#endregion

		#region Properties

		public Vector3 DeltaPosition
		{
			get
			{
				if(!_mousePressed) return Vector3.zero;
				return _currentPosition - _previousPosition;
			}
		}

		public Vector3 CurrentPosition => _currentPosition;

		public bool MousePressed => _mousePressed;

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void Update()
		{
			TryDetectMouseInput();
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
			if (TryMouseRaycast(out RaycastHit hit))
			{
				_currentPosition = hit.point;
				_pointer.position = _currentPosition;
				
				if (Input.GetMouseButtonDown(0))
				{
					_mousePressed = true;
					_mousePressedStartWorldPosition = hit.point;

					Debug.Log("Hit object: " + hit.collider.name);
					if (_fabricLayer.Contains(hit.collider.gameObject.layer))
					{
						//fabric clicked
					}
					else if (_interactableLayer.Contains(hit.collider.gameObject.layer))
					{
						//obstacle clicked
					}
				}

			}
			
			if (Input.GetMouseButtonUp(0))
			{
				_mousePressed = false;
			}

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