#if ADDRESSABLES
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Ateo.Common.AddressableAssets
{
	public static class AddressableLoaderList<T, TU> where T : AssetReferenceT<TU> where TU : Object
	{
		public static async UniTask Load(List<T> assetReferenceList, List<TU> results, Action<TU, int> callback = null,
			CancellationToken token = default)
		{
			if (assetReferenceList == null || results == null)
			{
				return;
			}

			DebugDev.Log($"AddressableLoaderList {typeof(TU)}: Loading started");

#if UNITY_EDITOR
			Stopwatch watch = new Stopwatch();
			watch.Start();
#endif

			int index = 0;
			results.Clear();

			foreach (T reference in assetReferenceList)
			{
				if (token.IsCancellationRequested)
				{
					continue;
				}

				TU asset = await AddressableLoader<TU>.Load(reference, token);

				if (asset != null)
				{
					results.Add(asset);
					callback?.Invoke(asset, index);

					index++;
				}
			}

			if (token.IsCancellationRequested)
			{
				DebugDev.Log($"AddressableLoaderList {typeof(TU)}: Loading aborted --> Unloading");
				results.Clear();
				Unload(assetReferenceList);
			}

#if UNITY_EDITOR
			watch.Stop();
			DebugDev.Log($"AddressableLoaderList {typeof(TU)}: Total loading duration = {watch.ElapsedMilliseconds}ms");
#else
            DebugDev.Log($"AddressableLoaderList {typeof(TU)}: Loading complete");
#endif
		}

		public static void Unload(List<T> list)
		{
			if (list != null)
			{
				foreach (T reference in list)
				{
					AddressableLoader<TU>.Unload(reference);
				}
			}
		}
	}
}
#endif