#if ADDRESSABLES
using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Ateo.Common.AddressableAssets
{
	public static class AddressableLoaderScene
	{
		/// <summary>
		/// A cached AssetReferenceScene returns a size of around 0.4 MB when calling Addressables.GetDownloadSizeAsync().
		/// Since it does not match the expected value of 0, we have to set our own threshold.
		/// This value is in bytes.
		/// </summary>
		public static long CachedThresholdInBytes { get; set; } = 1048576;

		public static async UniTask<CacheResult> CheckCached(AssetReference assetReference, CancellationToken token = default)
		{
			CacheResult result = await AddressablesHelper.CheckCached(assetReference, CachedThresholdInBytes, token);
			return result;
		}
		
		public static async UniTask LoadDependencies(AssetReference sceneReference, CancellationToken token, IProgress<float> progress = null)
		{
			if (token.IsCancellationRequested) return;

			CacheResult result = await CheckCached(sceneReference, token);

			if (result.Bytes > 0)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();

				await Addressables.DownloadDependenciesAsync(sceneReference.RuntimeKey)
					.ToUniTask(cancellationToken: token, progress: progress);

				watch.Stop();

				if (token.IsCancellationRequested) return;

				progress?.Report(1f);

				DebugDev.Log($"[AddressableLoaderScene] Dependencies of scene '{result.Name}' downloaded in {watch.ElapsedMilliseconds:0} ms");
			}
			else
			{
				progress?.Report(1f);
				DebugDev.Log($"[AddressableLoaderScene] Dependencies of scene '{result.Name}' are cached");
			}
		}

		public static async UniTask LoadScene(AssetReference sceneReference, CancellationToken token, IProgress<float> progress = null)
		{
			if (token.IsCancellationRequested) return;

			CacheResult result = await CheckCached(sceneReference, token);

			Stopwatch watch = new Stopwatch();
			watch.Restart();

			await (sceneReference.IsValid()
				? sceneReference.OperationHandle.ToUniTask(cancellationToken: token, progress: progress)
				: sceneReference.LoadSceneAsync(LoadSceneMode.Additive).ToUniTask(cancellationToken: token, progress: progress));

			watch.Stop();

			progress?.Report(1f);

			DebugDev.Log($"[AddressableLoaderScene] Scene '{result.Name}' loaded in {watch.ElapsedMilliseconds:0} ms");
		}

		public static async UniTask LoadSceneWithDependencies(AssetReference sceneReference, CancellationToken token,
			IProgress<float> progressDependencies = null, IProgress<float> progressScene = null)
		{
			await LoadDependencies(sceneReference, token, progressDependencies);
			await LoadScene(sceneReference, token, progressScene);
		}

		public static async UniTask UnloadScene(AssetReferenceScene sceneReference, CancellationToken token, IProgress<float> progress = null)
		{
			if (token.IsCancellationRequested) return;

			CacheResult result = await CheckCached(sceneReference, token);

			if (sceneReference.IsValid())
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();

				await Addressables.UnloadSceneAsync(sceneReference.OperationHandle).ToUniTask(cancellationToken: token, progress: progress);

				if (token.IsCancellationRequested) return;

				progress?.Report(1f);

				if (sceneReference.IsValid())
				{
					sceneReference.ReleaseAsset();
				}

				watch.Stop();
				DebugDev.Log($"[AddressableLoaderScene] Scene '{result.Name}' unloaded and released in {watch.ElapsedMilliseconds:0} ms");
			}
			else
			{
				progress?.Report(1f);
				DebugDev.Log($"[AddressableLoaderScene] Scene '{result.Name}' was already unloaded");
			}
		}
	}
}
#endif