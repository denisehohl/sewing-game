#if ADDRESSABLES
using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Ateo.Common.AddressableAssets
{
	public static class AddressableLoader<T> where T: Object
	{
		public static async UniTaskVoid Load(AssetReferenceT<T> assetReference, Action<T> callback = null, CancellationToken token = default)
		{
			T asset = await Load(assetReference, token);

			if (!token.IsCancellationRequested)
			{
				callback?.Invoke(asset);
			}
		}

		public static async UniTask<T> Load(AssetReferenceT<T> assetReference, CancellationToken token = default)
		{
			if (assetReference == null || !assetReference.RuntimeKeyIsValid())
			{
				return null;
			}

			DebugDev.Log($"AddressableLoader {typeof(T).Name}: Loading started");

#if UNITY_EDITOR
			Stopwatch watch = new Stopwatch();
			watch.Start();
#endif
			AsyncOperationHandle handle = assetReference.OperationHandle.IsValid()
				? assetReference.OperationHandle
				: assetReference.LoadAssetAsync();
			
			while (!handle.IsDone && !token.IsCancellationRequested)
			{
#if UNITY_EDITOR
				DebugDev.Log($"AddressableLoader {typeof(T).Name}: Loading - {handle.PercentComplete}%");
#endif
				await UniTask.Yield(token).SuppressCancellationThrow();
			}

			T asset = null;

			if (token.IsCancellationRequested)
			{
				DebugDev.Log($"AddressableLoader {typeof(T).Name}: Loading aborted --> Unloading");
				Unload(assetReference);
			}
			else
			{
				asset = (T) handle.Result;
			}

#if UNITY_EDITOR
			watch.Stop();
			DebugDev.Log($"AddressableLoader {typeof(T).Name}: Total loading duration = {watch.ElapsedMilliseconds}ms");
#else
            DebugDev.Log($"AddressableLoader {typeof(T).Name}: Loading complete");
#endif

			return asset;
		}

		public static void Unload(AssetReferenceT<T> modelReference)
		{
			if (modelReference == null || !modelReference.RuntimeKeyIsValid() || !modelReference.IsValid())
			{
				return;
			}
			
			DebugDev.Log($"AddressableLoader {typeof(T).Name}: Releasing Asset '{modelReference.SubObjectName}'");
			modelReference.ReleaseAsset();
		}
	}
}
#endif