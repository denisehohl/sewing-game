// IMPORTANT: Never animate the same component in different groups!
// Groups are as follows:
// - Down/Up
// - Enter/Exit
// - Interactive/Noninteractive

using Ateo.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Ateo.Animation
{
	public class ButtonAnimationBehaviour : SelectableAnimationBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
		IPointerUpHandler
	{
		#region Fields

		[BoxGroup("Settings"), SerializeField]
		private bool _enterExit = true;

		[BoxGroup("Settings"), SerializeField]
		private bool _downUp = true;

		[SerializeField, ShowIf(nameof(_downUp)), FormerlySerializedAs("AnimationBehaviour_PointerDown")]
		public AnimationBehaviour PointerDown;

		[SerializeField, ShowIf(nameof(_downUp)), FormerlySerializedAs("AnimationBehaviour_PointerUp")]
		public AnimationBehaviour PointerUp;

		[SerializeField, ShowIf(nameof(_enterExit)), FormerlySerializedAs("AnimationBehaviour_PointerEnter")]
		public AnimationBehaviour PointerEnter;

		[SerializeField, ShowIf(nameof(_enterExit)), FormerlySerializedAs("AnimationBehaviour_PointerExit")]
		public AnimationBehaviour PointerExit;

		private bool _buttonDown;

		#endregion

		#region Properties

		public Boolean IsPressed { get; } = new Boolean();
		public Boolean IsEntered { get; } = new Boolean();

		#endregion

		#region MonoBehaviour Callbacks

		protected override void OnEnable()
		{
			IsEntered.SetValueToFalse();
			IsPressed.SetValueToFalse();

			base.OnEnable();
		}

		#endregion

		#region Event Callbacks

		protected override void OnInteractableChanged(bool isInteractable)
		{
			if (isInteractable)
			{
				TryExecuteAnimationEnter();
				TryExecuteAnimationInteractive();
			}
			else
			{
				TryExecuteAnimationUp();
				TryExecuteAnimationExit();
				TryExecuteAnimationNonInteractive();
			}
		}

		#endregion

		#region Event System Interface Implementations

		public void OnPointerEnter(PointerEventData eventData)
		{
			IsEntered.SetValueToTrue();
			IsPressed.SetValueToFalse();
			TryExecuteAnimationEnter();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			IsEntered.SetValueToFalse();
			IsPressed.SetValueToFalse();
			TryExecuteAnimationExit();
			TryExecuteAnimationUp();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			IsPressed.SetValueToTrue();
			TryExecuteAnimationDown();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			IsPressed.SetValueToFalse();
			TryExecuteAnimationUp();
		}

		#endregion

		#region Animation Execution

		private void TryExecuteAnimationDown()
		{
			if (!_downUp || PointerDown == null || PointerDown.IsRunning) return;

			if (IsPressed && IsInteractable)
			{
				if (PointerUp.IsRunning)
				{
					PointerUp.Abort();
				}

				PointerDown.Execute(false, OnAnimationDownComplete);
			}
		}

		private void TryExecuteAnimationUp()
		{
			if (!_downUp || PointerDown == null || (PointerDown.IsRunning && IsEntered) || PointerUp == null || PointerUp.IsRunning) return;

			if (!IsPressed && _buttonDown)
			{
				PointerUp.Execute(false, OnAnimationUpComplete);
			}
		}

		private void TryExecuteAnimationEnter()
		{
			if (!_enterExit || PointerEnter == null || PointerEnter.IsRunning) return;

			if (IsEntered && IsInteractable)
			{
				if (PointerExit.IsRunning)
				{
					PointerExit.Abort();
				}

				PointerEnter.Execute(false);
			}
		}

		private void TryExecuteAnimationExit()
		{
			if (!_enterExit || PointerExit == null || PointerExit.IsRunning) return;

			if (!IsEntered || !IsInteractable)
			{
				if (PointerEnter.IsRunning)
				{
					PointerEnter.Abort();
				}

				PointerExit.Execute(false);
			}
		}

		#endregion

		#region Animation Callbacks

		private void OnAnimationDownComplete()
		{
			_buttonDown = true;

			if (!IsPressed || !IsInteractable)
			{
				TryExecuteAnimationUp();
			}
		}

		private void OnAnimationUpComplete()
		{
			_buttonDown = false;
		}

		#endregion

		#region Protected Methods

		protected override void ResetAnimation()
		{
			if (PointerDown != null && PointerDown.IsInitialized)
			{
				PointerDown.ResetBehaviour();
			}

			if (PointerEnter != null && PointerEnter.IsInitialized)
			{
				PointerEnter.ResetBehaviour();
			}

			base.ResetAnimation();
		}

		#endregion
	}
}
// © 2022 Ateo GmbH (https://www.ateo.ch)