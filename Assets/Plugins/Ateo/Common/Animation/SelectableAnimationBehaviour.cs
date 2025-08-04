// IMPORTANT: Never animate the same component in different groups!
// Groups are as follows:
// - Down/Up
// - Enter/Exit
// - Interactive/Noninteractive

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Boolean = Ateo.Common.Boolean;

namespace Ateo.Animation
{
	public class SelectableAnimationBehaviour : MonoBehaviour
	{
		#region Fields

		[BoxGroup("Settings"), SerializeField]
		protected bool _interactive = true;

		[SerializeField, ShowIf(nameof(_interactive)), FormerlySerializedAs("AnimationBehaviour_Interactive")]
		protected AnimationBehaviour Interactive;

		[SerializeField, ShowIf(nameof(_interactive)), FormerlySerializedAs("AnimationBehaviour_NonInteractive")]
		protected AnimationBehaviour NonInteractive;

		protected Selectable _selectable;

		#endregion

		#region Properties

		public Boolean IsInteractable { get; } = new Boolean();

		#endregion

		#region MonoBehaviour Callbacks

		protected virtual void Awake()
		{
			_selectable = GetComponent<Selectable>();
		}

		protected virtual void OnEnable()
		{
			IsInteractable.SetValue(_selectable.interactable);
			ResetAnimation();
			IsInteractable.AddCallback(OnInteractableChanged);
		}

		protected virtual void OnDisable()
		{
			IsInteractable.RemoveCallback(OnInteractableChanged);
		}

		protected virtual void LateUpdate()
		{
			IsInteractable.SetValue(_selectable.interactable);
		}

		#endregion

		#region Event Callbacks

		protected virtual void OnInteractableChanged(bool isInteractable)
		{
			if (isInteractable)
			{
				TryExecuteAnimationInteractive();
			}
			else
			{
				TryExecuteAnimationNonInteractive();
			}
		}

		#endregion

		#region Animation Execution

		protected virtual void TryExecuteAnimationInteractive()
		{
			if (!_interactive || Interactive == null || Interactive.IsRunning) return;

			if (IsInteractable)
			{
				if (NonInteractive.IsRunning)
				{
					NonInteractive.Abort();
				}

				Interactive.Execute(false);
			}
		}

		protected virtual void TryExecuteAnimationNonInteractive()
		{
			if (!_interactive || NonInteractive == null || NonInteractive.IsRunning)
				return;

			if (!IsInteractable)
			{
				if (Interactive.IsRunning)
				{
					Interactive.Abort();
				}

				NonInteractive.Execute(false);
			}
		}
		
		#endregion

		#region Protected Methods

		protected virtual void ResetAnimation()
		{
			if (IsInteractable)
			{
				if (Interactive != null && Interactive.IsInitialized)
				{
					Interactive.ExecuteAnimationImmediate();
				}
			}
			else
			{
				if (NonInteractive != null && NonInteractive.IsInitialized)
				{
					NonInteractive.ExecuteAnimationImmediate();
				}
			}
		}

		#endregion
	}
}
// © 2022 Ateo GmbH (https://www.ateo.ch)