using System.Collections;
using Ateo.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.Common
{
	public static class SceneLoader
	{
		public delegate void SceneLoaderHandler(SceneReference sceneReference);

		public static event SceneLoaderHandler OnSceneLoaded;
		public static event SceneLoaderHandler OnSceneUnloaded;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			OnSceneLoaded = null;
			OnSceneUnloaded = null;
		}
#endif

		public static void LoadScene(SceneReference scene, LoadSceneMode mode = LoadSceneMode.Single, bool makeActive = true, float delay = 0f, DelegateVoid callback = null)
		{
			CoroutineRunner.Run(LoadSceneAsync(scene, mode, makeActive, delay, callback));
		}

		public static void UnloadScene(SceneReference scene, DelegateVoid callback = null)
		{
			CoroutineRunner.Run(UnloadSceneAsync(scene, callback));
		}

		public static IEnumerator LoadSceneAsync(SceneReference sceneReference, LoadSceneMode mode, bool makeActive, float delay, DelegateVoid callback = null)
		{
			yield return new WaitForSeconds(delay);

			DebugDev.Log($"SceneLoader: Start loading scene {sceneReference.ScenePath}");

			if (!IsSceneLoaded(sceneReference) || mode == LoadSceneMode.Single)
			{
				yield return SceneManager.LoadSceneAsync(sceneReference, mode);

				if (makeActive)
					MakeSceneActive(sceneReference);

				DebugDev.Log($"SceneLoader: Scene {sceneReference.ScenePath} loaded");
			}
			else
			{
				DebugDev.Log($"SceneLoader: Scene {sceneReference.ScenePath} is already loaded");
			}

			DebugDev.Log("SceneLoader: Scene loading complete");
			callback?.Invoke();
			OnSceneLoaded?.Invoke(sceneReference);
		}

		public static IEnumerator UnloadSceneAsync(SceneReference sceneReference, DelegateVoid callback = null)
		{
			DebugDev.Log("SceneLoader: Start unloading scenes");

			if (IsSceneLoaded(sceneReference))
			{
				yield return SceneManager.UnloadSceneAsync(sceneReference);
				DebugDev.Log($"SceneLoader: Scene {sceneReference.ScenePath} unloaded");
			}
			else
			{
				DebugDev.Log($"SceneLoader: Scene {sceneReference.ScenePath} is already unloaded");
			}

			DebugDev.Log("SceneLoader: Scene unloading complete");
			callback?.Invoke();
			OnSceneUnloaded?.Invoke(sceneReference);
		}

		public static bool MakeSceneActive(SceneReference sceneReference)
		{
			if (GetScene(sceneReference, out var scene))
			{
				SceneManager.SetActiveScene(scene);
				DebugDev.Log($"SceneLoader: Set active Scene to {sceneReference.ScenePath}");
				return true;
			}

			DebugDev.Log($"SceneLoader: Failed to make Scene {sceneReference.ScenePath} the active scene");
			return false;
		}

		public static bool GetScene(SceneReference reference, out Scene scene)
		{
			for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				var pathToScene = SceneUtility.GetScenePathByBuildIndex(i);

				if ((!string.Equals(pathToScene, reference.ScenePath))) continue;

				scene = SceneManager.GetSceneByBuildIndex(i);
				return true;
			}

			scene = default;
			return false;
		}

		public static bool IsSceneLoaded(SceneReference reference)
		{
			for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				var pathToScene = SceneUtility.GetScenePathByBuildIndex(i);

				if ((!string.Equals(pathToScene, reference.ScenePath))) continue;

				var scene = SceneManager.GetSceneByBuildIndex(i);
				return scene.isLoaded;
			}

			return true;
		}
	}
}