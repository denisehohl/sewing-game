using System;
using System.Threading;
using Ateo.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Common.Inputs
{
	public enum ScrollSource
	{
		Undefined,
		Mouse,
		Trackpad
	}

	public class ScrollDelta : ComponentPublishBehaviour<ScrollDelta>
	{
		#region Fields

		[Serializable]
		public class ScrollData
		{
			public float ScrollSpeed;
			public float Smoothing;
			public float TimeOut;
			public float DampingFactor;
		}

		[BoxGroup("Mouse"), SerializeField, HideLabel, InlineProperty]
		private ScrollData _mouseData = new ScrollData();

		[BoxGroup("Trackpad"), SerializeField, HideLabel, InlineProperty]
		private ScrollData _trackpadData = new ScrollData();

		private static ScrollSource _source;
		private static ScrollData _scrollData;

		private static bool _isScrolling;
		private static float _timeLastScroll;
		private static int _scrollStartFrame;
		private static int _scrollDirection;

		private static int _countedInts;
		private static int _countedIntValues;
		private static int _countedFloats;
		private static float _counterStarted;

		private static CancellationTokenSource _sourceMode;
		private static CancellationTokenSource _sourceInput;

		#endregion

		#region Properties

		public static float Delta { get; private set; }
		public static bool ScrollStartedThisFrame => _scrollStartFrame == Time.frameCount;
		public static int ScrollDirection => _scrollDirection;
		public static ScrollSource Source => _source;

		#endregion

		#region ComponentPublishBehaviour Callbacks
		
		public override void ResetStatics()
		{
			base.ResetStatics();
			
			_source = ScrollSource.Undefined;
			_scrollData = null;
			
			_isScrolling = false;
			_timeLastScroll = 0f;
			_scrollStartFrame = 0;
			_scrollDirection = 0;

			_countedInts = 0;
			_countedIntValues = 0;
			_countedFloats = 0;
			_counterStarted = 0;

			_sourceMode = new CancellationTokenSource();
			_sourceInput = new CancellationTokenSource();
		}

		protected override void OnPublish()
		{
			DetermineScrollSource().Forget();
			FetchInput().Forget();
		}

		protected override void OnWithdraw()
		{
			TaskHelper.Kill(ref _sourceMode);
			TaskHelper.Kill(ref _sourceInput);
		}

		#endregion

		#region Public Methods

		public static float Dampen(float delta)
		{
			if (_source != ScrollSource.Undefined)
			{
				return delta * _scrollData.DampingFactor;
			}

			return delta;
		}

		#endregion

		#region Private Methods

		private void SetInputMethod(ScrollSource input)
		{
			_source = input;

			switch (input)
			{
				case ScrollSource.Trackpad:
					_scrollData = _trackpadData;
					break;
				case ScrollSource.Mouse:
				case ScrollSource.Undefined:
				default:
					_scrollData = _mouseData;
					break;
			}

			DebugDev.Log($"ScrollDelta.SetInputMethod: {input}");
		}

		private async UniTaskVoid DetermineScrollSource()
		{
			CancellationToken token = TaskHelper.RefreshToken(ref _sourceMode);

			_source = ScrollSource.Undefined;
			_countedInts = 0;
			_countedIntValues = 0;
			_countedFloats = 0;
			_counterStarted = 0;

			while (_source == ScrollSource.Undefined && !token.IsCancellationRequested)
			{
				if (await UniTask.Yield(PlayerLoopTiming.EarlyUpdate, token).SuppressCancellationThrow()) return;

				float absScroll = Mathf.Abs(Input.mouseScrollDelta.y);
				int absScrollInt = Mathf.RoundToInt(absScroll);

				if (absScrollInt > 0 && Math.Abs(absScrollInt - absScroll) < 0.01f)
				{
					LogInput();

					_countedInts++;
					_countedIntValues += absScrollInt;

					if (_countedInts == 1)
					{
						_counterStarted = Time.time;
					}

					if (absScrollInt > 1)
					{
						SetInputMethod(ScrollSource.Trackpad);
						continue;
					}
				}
				else if (absScroll > 0f)
				{
					LogInput();

					_countedFloats++;

					if (_countedFloats <= 10) continue;

					SetInputMethod(ScrollSource.Trackpad);
					continue;
				}

				if (_countedInts <= 0 || !(Time.time > _counterStarted + 0.5f)) continue;

				DebugDev.Log($"Total ints counted {_countedIntValues}");

				SetInputMethod(_countedIntValues > 10 ? ScrollSource.Trackpad : ScrollSource.Mouse);
			}
		}

		private async UniTaskVoid FetchInput()
		{
			CancellationToken token = TaskHelper.RefreshToken(ref _sourceInput);

			_source = ScrollSource.Undefined;
			_isScrolling = false;
			Delta = 0f;

			while (!token.IsCancellationRequested)
			{
				if (await UniTask.Yield(PlayerLoopTiming.EarlyUpdate, token).SuppressCancellationThrow()) return;

				if (_source == ScrollSource.Undefined) continue;

				float scrollDelta = Input.mouseScrollDelta.y;
				bool isInteracting = Mathf.Abs(scrollDelta) > 0.025f;
				int scrollDirection = scrollDelta < 0f ? -1 : 1;

#if !UNITY_EDITOR
				if (_source == ScrollSource.Trackpad)
				{
					scrollDelta *= 0.5f;
				}
#endif
				if (isInteracting)
				{
					_timeLastScroll = Time.time;

					if (!_isScrolling)
					{
						_isScrolling = true;
						_scrollStartFrame = Time.frameCount;
					}
				}
				else if (_isScrolling && Time.time > _timeLastScroll + _scrollData.TimeOut)
				{
					_isScrolling = false;
				}

				if (scrollDirection != _scrollDirection)
				{
					if (isInteracting)
					{
						_scrollDirection = scrollDirection;
						Delta = 0f;
					}
				}

				if (_isScrolling)
				{
					Delta = Mathf.Lerp(scrollDelta * _scrollData.ScrollSpeed, Delta, _scrollData.Smoothing);
				}
				else // Apply dampening to scroll delta
				{
					Delta *= _scrollData.DampingFactor;
				}
			}
		}

		private static void LogInput()
		{
			DebugDev.Log($"ScrollDelta.Log: {Input.mouseScrollDelta.y}");
		}

		#endregion
	}
}