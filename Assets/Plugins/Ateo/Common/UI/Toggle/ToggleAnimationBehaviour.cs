using Ateo.Animation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.UI
{
	public class ToggleAnimationBehaviour : ToggleAnimationBase
	{
		[SerializeField, Required]
		private AnimationBehaviour _animationEnable;

		[SerializeField, Required]
		private AnimationBehaviour _animationDisable;

		protected override void Initialize()
		{
		}

		protected override void Enable()
		{
			AbortRunningAnimations();
			_animationEnable.Execute();
		}

		protected override void EnableImmediate()
		{
			AbortRunningAnimations();
			_animationEnable.ExecuteAnimationImmediate();
		}

		protected override void Disable()
		{
			AbortRunningAnimations();
			_animationDisable.Execute();
		}

		protected override void DisableImmediate()
		{
			AbortRunningAnimations();
			_animationDisable.ExecuteAnimationImmediate();
		}

		private void AbortRunningAnimations()
		{
			if (_animationEnable.IsRunning)
				_animationEnable.Abort();

			if (_animationDisable.IsRunning)
				_animationDisable.Abort();
		}
	}
}