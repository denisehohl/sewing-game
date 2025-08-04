using System;
using System.Threading;
using Ateo.Extensions;
using Cysharp.Threading.Tasks;

namespace Ateo.Common.Util
{
	public static class RuntimeHelper
	{
		public static void InvokeAfterXFrames(Action action, int frames, CancellationToken token = default)
		{
			Run().Forget();
			return;

			async UniTaskVoid Run()
			{
				while (frames > 0)
				{
					await UniTask.NextFrame(PlayerLoopTiming.Update, token);
					frames--;
				}

				if (!token.IsCancellationRequested)
				{
					action?.Invoke();
				}
			}
		}
		
		public static void InvokeAfterXFrames<T>(Action<T> action, T value, int frames, CancellationToken token = default)
		{
			Run().Forget();
			return;

			async UniTaskVoid Run()
			{
				while (frames > 0)
				{
					await UniTask.NextFrame(PlayerLoopTiming.Update, token);
					frames--;
				}

				if (!token.IsCancellationRequested)
				{
					action?.Invoke(value);
				}
			}
		}

		public static void InvokeAfterXSeconds(Action action, float seconds, CancellationToken token = default)
		{
			Run().Forget();
			return;

			async UniTaskVoid Run()
			{
				await UniTask.Delay(Timef.SecondsToMilliseconds(seconds), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
				
				if (!token.IsCancellationRequested)
				{
					action?.Invoke();
				}
			}
		}
		
		public static void InvokeAfterXSeconds<T>(Action<T> action, T value, float seconds, CancellationToken token = default)
		{
			Run().Forget();
			return;

			async UniTaskVoid Run()
			{
				await UniTask.Delay(Timef.SecondsToMilliseconds(seconds), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
				
				if (!token.IsCancellationRequested)
				{
					action?.Invoke(value);
				}
			}
		}
	}
}