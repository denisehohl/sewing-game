using UnityEngine;

namespace Ateo.Common
{
	public static class SceneReferenceManager
	{
		private const string SceneReferencesPath = "SceneReferences";
		private static SceneReferences _sceneReferences;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			_sceneReferences = null;
		}
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void LoadSceneReferences()
		{
			_sceneReferences = Resources.Load<SceneReferences>(SceneReferencesPath);
		}

		public static SceneReference GetSceneReference(string key)
		{
			if (_sceneReferences != null)
			{
				if (_sceneReferences.References.TryGetValue(key, out var sceneReference))
				{
					return sceneReference;
				}
			}

			return null;
		}
	}
}