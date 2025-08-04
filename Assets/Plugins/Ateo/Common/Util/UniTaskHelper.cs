using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Ateo.Common.Util
{
	/// <summary>
	/// Provides helper methods for working with UniTask and web requests.
	/// </summary>
	public static class UniTaskHelper
	{
		#region Public Methods

		/// <summary>
		/// Sends a web request asynchronously using UniTask.
		/// </summary>
		/// <param name="request">The UnityWebRequest to be sent.</param>
		/// <param name="token">Cancellation token to cancel the request.</param>
		/// <returns>Returns true if the request is successful, false otherwise.</returns>
		public static async UniTask<bool> SendWebRequest(UnityWebRequest request, CancellationToken token)
		{
			if (request == null)
			{
				Debug.LogError("UniTaskHelper.SendWebRequest: Request is null.");
				return false;
			}

			try
			{
				await request.SendWebRequest().WithCancellation(token).SuppressCancellationThrow();

				if (token.IsCancellationRequested)
				{
					Debug.LogWarning("UniTaskHelper.SendWebRequest: Request was cancelled.");
					return false;
				}

#if UNITY_2020_1_OR_NEWER
				if (request.result != UnityWebRequest.Result.Success)
#else
				if (request.isNetworkError || request.isHttpError)
#endif
				{
					Debug.LogError($"UniTaskHelper.SendWebRequest: Error = {request.error}");
					return false;
				}

				return true;
			}
			catch (Exception e)
			{
				Debug.LogError($"UniTaskHelper.SendWebRequest: Exception = {e.Message}");
				return false;
			}
		}

		/// <summary>
		/// Loads an image from a URL asynchronously using UniTask.
		/// </summary>
		/// <param name="url">The URL of the image to be loaded.</param>
		/// <param name="token">Cancellation token to cancel the request.</param>
		/// <returns>Returns the loaded Texture2D if successful, null otherwise.</returns>
		public static async UniTask<Texture2D> LoadImageFromUrl(string url, CancellationToken token)
		{
			if (!IsValidUrl(url))
			{
				Debug.LogError($"UniTaskHelper.LoadImageFromUrl: Invalid URL provided. URL = {url}");
				return null;
			}

			try
			{
#if UNITY_2020_1_OR_NEWER
				using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
#else
				UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
				using (request)
#endif
				{
					await request.SendWebRequest().WithCancellation(token).SuppressCancellationThrow();

					if (token.IsCancellationRequested)
					{
						Debug.LogWarning("UniTaskHelper.LoadImageFromUrl: Request was cancelled.");
						return null;
					}

#if UNITY_2020_1_OR_NEWER
					if (request.result != UnityWebRequest.Result.Success)
#else
					if (request.isNetworkError || request.isHttpError)
#endif
					{
						Debug.LogError($"UniTaskHelper.LoadImageFromUrl: Error = {request.error}");
						return null;
					}

					return DownloadHandlerTexture.GetContent(request);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"UniTaskHelper.LoadImageFromUrl: Exception = {e.Message}");
				return null;
			}
		}
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Validates if the provided string is a valid URL.
		/// </summary>
		/// <param name="url">The URL string to validate.</param>
		/// <returns>Returns true if the URL is valid, false otherwise.</returns>
		private static bool IsValidUrl(string url)
		{
			return !string.IsNullOrEmpty(url) && 
			       Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && 
			       (uriResult.Scheme == Uri.UriSchemeHttp || 
			        uriResult.Scheme == Uri.UriSchemeHttps ||
			        uriResult.Scheme == Uri.UriSchemeFile);
		}


		#endregion
	}
}