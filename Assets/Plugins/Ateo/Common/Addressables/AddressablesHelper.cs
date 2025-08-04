#if ADDRESSABLES
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ateo.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Ateo.Common.AddressableAssets
{
	public struct CacheResult
	{
		public readonly string Name;
		public readonly bool Cached;
		public readonly long Bytes;
		public readonly long MegaBytes;

		public CacheResult(string name, bool cached, long bytes)
		{
			Name = name;
			Cached = cached;
			Bytes = bytes;
			MegaBytes = bytes.FromBytesToMegabytes();
		}
	}

	public static class AddressablesHelper
	{
		/// <summary>
		/// Checks whether the amount of bytes is equal or lower to the specified threshold.
		/// </summary>
		/// <param name="bytes">Amount of bytes</param>
		/// <param name="threshold">A cached AssetReference returns a size of 0. You may increase this size.</param>
		/// <returns></returns>
		public static bool IsCached(long bytes, long threshold = 0)
		{
			return bytes <= threshold;
		}

		public static async UniTask<string> GetPrimaryKey(AssetReference assetReference, CancellationToken token = default)
		{
			IList<IResourceLocation> result = await Addressables.LoadResourceLocationsAsync(assetReference).ToUniTask(cancellationToken: token);
			return !token.IsCancellationRequested ? result.First()?.PrimaryKey : default;
		}

		public static async UniTask<CacheResult> CheckCached(AssetReference assetReference, long cachedThresholdInBytes = 0,
			CancellationToken token = default)
		{
			string name = await GetPrimaryKey(assetReference, token);

			if (token.IsCancellationRequested) return default;
			
			long bytes = await Addressables.GetDownloadSizeAsync(assetReference.RuntimeKey).ToUniTask(cancellationToken: token);

			return !token.IsCancellationRequested ? new CacheResult(name, IsCached(bytes, cachedThresholdInBytes), bytes) : default;
		}
	}
}
#endif