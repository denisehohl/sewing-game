using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public enum ButtonClickMode
	{
		Click,
		PointerDown,
		PointerUp
	};
	
	[Serializable, ExecuteInEditMode]
	public class Button2 : Button
	{
		[SerializeField]
		private ButtonClickMode _buttonClickMode = ButtonClickMode.Click;

		[SerializeField, MinValue(0)]
		private float _delay;
		
		private bool _interactable;
		private int _lastEventInvocation;
		
		public event Action<bool> OnInteractableChanged;
		public event Action OnDown;
		public event Action OnUp;
		
		public ButtonClickMode ButtonClickMode
		{
			get => _buttonClickMode;
			set => _buttonClickMode = value;
		}

		protected override void Awake()
		{
			base.Awake();
			_interactable = interactable;
		}

		protected virtual bool Click()
		{
			if (!IsActive() || !IsInteractable() || _lastEventInvocation == Time.frameCount)
				return false;

			_lastEventInvocation = Time.frameCount; // Prevent double invocations in case some script changes the ButtonClickMode in the same frame

			if (_delay > 0f)
			{
				Invoke(nameof(InvokeOnClick), _delay);
			}
			else
			{
				InvokeOnClick();
			}
			
			return true;
		}

		protected void InvokeOnClick()
		{
			onClick.Invoke();
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			
			if (_buttonClickMode == ButtonClickMode.Click)
			{
				Click();
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			base.OnPointerDown(eventData);
			
			OnDown?.Invoke();

			if (_buttonClickMode == ButtonClickMode.PointerDown)
			{
				Click();
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			base.OnPointerUp(eventData);
			
			OnUp?.Invoke();

			if (_buttonClickMode == ButtonClickMode.PointerUp)
			{
				Click();
			}
		}

		public override void OnSubmit(BaseEventData eventData)
		{
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);

			bool isInteractable = IsInteractable();

			if (!isInteractable && _interactable)
			{
				_interactable = false;
				OnInteractableChanged?.Invoke(false);
			}
			else if (isInteractable && !_interactable)
			{
				_interactable = true;
				OnInteractableChanged?.Invoke(true);
			}
		}
	}
}