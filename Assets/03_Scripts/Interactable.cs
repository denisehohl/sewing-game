using System;
using UnityEngine;
using UnityEngine.Events;

namespace Moreno.SewingGame
{
	public class Interactable : MonoBehaviour
	{
		#region private Variables

		private bool _clicked;

		#endregion

		#region Properties

		public bool Clicked => _clicked;

		#endregion

		#region Delegates & Events

		public UnityEvent OnClickedEvent;
		public UnityEvent OnReleasedEvent;

		public event Action OnClicked;
		public event Action OnReleased;

		#endregion

		#region Monobehaviour Callbacks

		private void OnEnable()
		{
			MouseWorldPointer.OnObjectClicked += OnObjectClicked;
			MouseWorldPointer.OnMouseRelease += OnMouseRelease;
		}

		private void OnDisable()
		{
			MouseWorldPointer.OnObjectClicked -= OnObjectClicked;
			MouseWorldPointer.OnMouseRelease -= OnMouseRelease;
		}


		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion

		#region Event Callbacks
		
		private void OnObjectClicked(GameObject obj)
		{
			if(gameObject != obj) return;
			_clicked = true;
			OnClicked?.Invoke();
			OnClickedEvent?.Invoke();
		}

		private void OnMouseRelease()
		{
			if(!_clicked) return;
			_clicked = false;
			OnReleased?.Invoke();
			OnReleasedEvent?.Invoke();
		}

		#endregion

		
	}
}