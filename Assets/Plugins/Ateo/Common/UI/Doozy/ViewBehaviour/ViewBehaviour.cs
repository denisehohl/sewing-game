#if DOOZY_3 || DOOZY_4
using System.Threading;
using Ateo.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#if DOOZY_3
using Doozy.Engine.UI;
#elif DOOZY_4
using Doozy.Runtime.UIManager.Containers;
#endif

namespace Ateo.UI
{
	[RequireComponent(typeof(UIView))]
	public abstract class ViewBehaviour : MonoBehaviour
	{
		[FoldoutGroup("View Behaviour"), SerializeField]
		private bool _hasParentView;

		[FoldoutGroup("View Behaviour"), SerializeField, ShowIf(nameof(_hasParentView))]
		private UIView _parentView;

		[FoldoutGroup("View Behaviour"), SerializeField, ShowIf(nameof(_hasParentView))]
		private bool _hideWhenParentViewIsHiding = true;

		private UnityAction _actionHide;
		private CancellationTokenSource _sourceUpdate;

		protected UIView UiView { get; private set; }

		protected virtual void Awake()
		{
			UiView = GetComponent<UIView>();

			if (UiView == null) return;
#if DOOZY_3
			UiView.ShowBehavior.OnStart.Event.AddListener(OnShowStart);
			UiView.ShowBehavior.OnFinished.Event.AddListener(OnShowFinished);
			UiView.HideBehavior.OnStart.Event.AddListener(OnHideStart);
			UiView.HideBehavior.OnFinished.Event.AddListener(OnHideFinished);

			// View Update
			UiView.ShowBehavior.OnFinished.Event.AddListener(StartViewUpdate);
			UiView.HideBehavior.OnStart.Event.AddListener(StopViewUpdate);

			if (_hasParentView && _parentView != null && _hideWhenParentViewIsHiding)
			{
				_actionHide = () => UiView.Hide();
				_parentView.HideBehavior.OnStart.Event.AddListener(_actionHide);
			}
#elif DOOZY_4
			UiView.OnShowCallback.Event.AddListener(OnShowCallback);
			UiView.OnVisibleCallback.Event.AddListener(OnVisibleCallback);
			UiView.OnHideCallback.Event.AddListener(OnHideCallback);
			UiView.OnHiddenCallback.Event.AddListener(OnHiddenCallback);

			if (_hasParentView && _parentView != null && _hideWhenParentViewIsHiding)
			{
				_actionHide = () => UiView.Hide();
				_parentView.OnHideCallback.Event.AddListener(_actionHide);
			}
#endif
		}

		protected virtual void OnDestroy()
		{
			if (UiView == null) return;
#if DOOZY_3
			UiView.ShowBehavior.OnStart.Event.RemoveListener(OnShowStart);
			UiView.ShowBehavior.OnFinished.Event.RemoveListener(OnShowFinished);
			UiView.HideBehavior.OnStart.Event.RemoveListener(OnHideStart);
			UiView.HideBehavior.OnFinished.Event.RemoveListener(OnHideFinished);

			// View Update
			UiView.ShowBehavior.OnFinished.Event.AddListener(StartViewUpdate);
			UiView.HideBehavior.OnStart.Event.AddListener(StopViewUpdate);

			if (_hasParentView && _parentView != null && _hideWhenParentViewIsHiding && _actionHide != null)
			{
				_parentView.HideBehavior.OnStart.Event.RemoveListener(_actionHide);
			}
#elif DOOZY_4
			UiView.OnShowCallback.Event.RemoveListener(OnShowCallback);
			UiView.OnVisibleCallback.Event.RemoveListener(OnVisibleCallback);
			UiView.OnHideCallback.Event.RemoveListener(OnHideCallback);
			UiView.OnHiddenCallback.Event.RemoveListener(OnHiddenCallback);

			if (_hasParentView && _parentView != null && _hideWhenParentViewIsHiding && _actionHide != null)
			{
				_parentView.OnHideCallback.Event.RemoveListener(_actionHide);
			}

			StopViewUpdate();
#endif
		}

		public void StartViewUpdate()
		{
			ViewUpdateTask().Forget();
		}
		
		public void StopViewUpdate()
		{
			TaskHelper.Kill(ref _sourceUpdate);
		}

		public async UniTaskVoid ViewUpdateTask()
		{
			TaskHelper.RefreshToken(ref _sourceUpdate);

			CancellationToken token = _sourceUpdate.Token;

			while (!token.IsCancellationRequested)
			{
				OnViewUpdate();
				await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
			}
		}

		public void ShowView()
		{
			ShowView(false);
		}

		public void ShowView(bool instant, bool triggerCallbacks = true)
		{
#if DOOZY_3
			UiView.Show(instant);
#elif DOOZY_4
			if (instant)
			{
				UiView.InstantShow(triggerCallbacks);
			}
			else
			{
				UiView.Show(triggerCallbacks);
			}
#endif
		}

		public void HideView()
		{
			HideView(false);
		}

		public void HideView(bool instant, bool triggerCallbacks = true)
		{
#if DOOZY_3
			UiView.Hide(instant);
#elif DOOZY_4
			if (instant)
			{
				UiView.InstantHide(triggerCallbacks);
			}
			else
			{
				UiView.Hide(triggerCallbacks);
			}
#endif
		}

		public bool IsViewVisible()
		{
#if DOOZY_3
			return UiView.IsVisible;
#elif DOOZY_4
			return UiView.isVisible;
#endif
		}

#if DOOZY_4

		private bool _wasVisible;
		
		private void OnShowCallback()
		{
			_wasVisible = true;
			OnShowStart();
		}

		private void OnVisibleCallback()
		{
			OnShowFinished();
			StartViewUpdate();
		}

		private void OnHideCallback()
		{
			if(!_wasVisible) return;
			
			StopViewUpdate();
			OnHideStart();
		}

		private void OnHiddenCallback()
		{
			if(!_wasVisible) return;
			
			_wasVisible = false;
			OnHideFinished();
		}
#endif
		
		protected virtual void OnShowStart()
		{
		}

		protected virtual void OnShowFinished()
		{
		}

		protected virtual void OnHideStart()
		{
		}

		protected virtual void OnHideFinished()
		{
		}

		protected virtual void OnViewUpdate()
		{
		}
	}
}
#endif