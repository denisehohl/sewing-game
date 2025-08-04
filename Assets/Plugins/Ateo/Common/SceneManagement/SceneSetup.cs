#if SCENESETUP
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace Ateo.Common
{
	[ExecuteInEditMode]
	public class SceneSetup : MonoBehaviour
	{
		[Serializable]
		public enum LoadMethod
		{
			Additive,
			AdditiveAsync,
			DontLoad
		}

		[Serializable]
		public class SceneEntry
		{
			public SceneReference Scene;

			[Tooltip("Should this be automatically loaded in the Editor?")]
			public bool LoadInEditor;

			[Tooltip("Should this Scene be reloaded when it is already loaded")]
			public bool Reload;

			[Tooltip("How should we load this scene at Runtime?")]
			public LoadMethod LoadMethod;

			public AsyncOperation AsyncOp { get; set; }

			public override string ToString()
			{
				return $"{Scene.Name} loadInEditor: {LoadInEditor} loadMethod: {LoadMethod}";
			}

			/// <summary>
			/// Overridden Equals to we can compare entries.  Entries with the same scene references and load settings are considered equal.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (this == obj)
					return true;

				var other = obj as SceneEntry;
				if (other == null)
					return false;

				return Scene.Equals(other.Scene) && (LoadInEditor == other.LoadInEditor) && (LoadMethod == other.LoadMethod) &&
				       (AsyncOp == other.AsyncOp);
			}

			protected bool Equals(SceneEntry other)
			{
				return Equals(Scene, other.Scene) && LoadInEditor == other.LoadInEditor && LoadMethod == other.LoadMethod &&
				       Equals(AsyncOp, other.AsyncOp);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					// ReSharper disable NonReadonlyMemberInGetHashCode
					var hashCode = (Scene != null ? Scene.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ LoadInEditor.GetHashCode();
					hashCode = (hashCode * 397) ^ (int) LoadMethod;
					return hashCode;
				}
			}

#if UNITY_EDITOR
			public SceneEntry()
			{
				LoadInEditor = true;
				LoadMethod = LoadMethod.Additive;
			}

			public SceneEntry(UnityEditor.SceneManagement.SceneSetup sceneSetup)
			{
				Scene = new SceneReference(sceneSetup.path);
				LoadInEditor = sceneSetup.isLoaded;
				LoadMethod = LoadMethod.Additive;
			}
#endif
		}

		public static SceneSetup Instance { get; private set; }

		[SerializeField, PropertyOrder(99)]
		protected bool LoadIfNotActiveScene = false;

		[SerializeField, PropertyOrder(100)]
		protected List<SceneEntry> _sceneSetup = new List<SceneEntry>();

		public static event Action<SceneSetup> OnAwake;
		public static event Action<SceneSetup> OnStart;
		public static event Action<SceneSetup> OnDestroyed;

		/// <summary>
		/// Read-only access to the Scene Setup.
		/// </summary>
		public virtual ReadOnlyCollection<SceneEntry> GetSceneSetup()
		{
			return _sceneSetup.AsReadOnly();
		}

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Instance = null;
			OnAwake = null;
			OnStart = null;
			OnDestroyed = null;
		}
#endif

		protected virtual void Awake()
		{
			OnAwake?.Invoke(this);

			if (!Application.isEditor || gameObject.scene.isLoaded || Time.frameCount > 1)
				LoadSceneSetup(false);
		}

		protected virtual void OnDestroy()
		{
			OnDestroyed?.Invoke(this);
		}

		protected virtual void Start()
		{
			OnStart?.Invoke(this);

			// Second chance at loading scenes
			LoadSceneSetup(false);
		}

		public virtual void LoadSceneSetup(bool reload = true)
		{
			if (!LoadIfNotActiveScene && !string.Equals(SceneManager.GetActiveScene().path, gameObject.scene.path))
			{
				DebugDev.Log($"SceneSetup: Load of SceneSetup of scene '{gameObject.scene.name}' not executed since this is not the active scene");
				return;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
				LoadSceneSetupInEditor();
			else
				LoadSceneSetupAtRuntime(reload);
#else
			LoadSceneSetupAtRuntime(reload);
#endif
		}

		public virtual void Reload()
		{
			LoadSceneSetup();
		}

		/// <summary>
		/// Load Scene Setup at Runtime.
		/// </summary>
		protected virtual async void LoadSceneSetupAtRuntime(bool reload = true)
		{
			await UnloadScenes(reload);

			var sceneSetup = GetSceneSetup();

			for (var i = 0; i < sceneSetup.Count; i++)
			{
				var entry = sceneSetup[i];
				LoadEntryAtRuntime(entry);

#if UNITY_EDITOR
				if (i == 0)
				{
					EditorSceneManager.MoveSceneBefore(gameObject.scene, sceneSetup[i].Scene.Scene);
				}

				if (i < sceneSetup.Count - 1)
				{
					EditorSceneManager.MoveSceneBefore(sceneSetup[i].Scene.Scene, sceneSetup[i + 1].Scene.Scene);
				}
#endif
			}

#if UNITY_EDITOR
			if (sceneSetup.Count > 0)
			{
				EditorSceneManager.MoveSceneBefore(gameObject.scene, sceneSetup[0].Scene.Scene);
			}
#endif
		}

		/// <summary>
		/// Load a particular Scene Entry
		/// </summary>
		/// <param name="entry">The Entry to load</param>
		protected virtual void LoadEntryAtRuntime(SceneEntry entry)
		{
			// Don't load 
			if (entry.LoadMethod == LoadMethod.DontLoad)
				return;

			// Already loaded?
			var existingScene = SceneManager.GetSceneByPath(entry.Scene.ScenePath);

			// If it's already loaded, return early
			if (existingScene.IsValid())
				return;

			if (entry.LoadMethod == LoadMethod.AdditiveAsync)
			{
				/*Debug.LogFormat(this, "Loading {0} Asynchronously from {1}", entry.Scene.Name, gameObject.scene.name);*/
				entry.AsyncOp = SceneManager.LoadSceneAsync(entry.Scene.ScenePath, LoadSceneMode.Additive);
				return;
			}

			if (entry.LoadMethod == LoadMethod.Additive)
			{
				/*Debug.LogFormat(this, "Loading {0} from {1}", entry.Scene.Name, gameObject.scene.name);*/
				SceneManager.LoadScene(entry.Scene.ScenePath, LoadSceneMode.Additive);
			}
		}

		protected virtual async UniTask UnloadScenes(bool reload)
		{
			if (!Application.isPlaying) return;

			/*if (Instance == null)
			{
			    Instance = this;
			    return;
			}*/

			/*if (Instance == this)
			{
			    return;
			}*/

			var sceneSetup = GetSceneSetup();

			var scenesToUnload = new List<Scene>();

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				var remove = true;

				if (string.Equals(gameObject.scene.path, scene.path))
				{
					continue;
				}

				foreach (var entry in sceneSetup)
				{
					if (string.Equals(entry.Scene.Scene.path, scene.path))
					{
						remove = reload && entry.Reload;
					}
				}

				if (remove)
				{
					scenesToUnload.Add(scene);
				}
			}

			foreach (var scene in scenesToUnload)
			{
				if (scene.IsValid())
				{
					/*DebugDev.Log($"Unload Scene {scene.path}");*/
					var operation = SceneManager.UnloadSceneAsync(scene);

					if (operation != null)
						await operation;
				}
			}

			Instance = this;
			await Task.Yield();
		}

		/// <summary>
		/// This executes in the Editor when a behaviour is initially added to a GameObject.
		/// </summary>
		protected virtual void Reset()
		{
			transform.SetAsFirstSibling();
		}

#if UNITY_EDITOR

		/// <summary>
		/// Loads the scene setup in the Editor
		/// </summary>
		protected virtual void LoadSceneSetupInEditor()
		{
			foreach (var entry in GetSceneSetup())
			{
				LoadEntryInEditor(entry);
			}
		}

		/// <summary>
		/// Loads a particular Scene Entry in the Editor
		/// </summary>
		/// <param name="entry">The entry to load</param>
		protected virtual void LoadEntryInEditor(SceneEntry entry)
		{
			// Bad time to do this.
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
				return;

			// We can't do this
			if (string.IsNullOrEmpty(entry.Scene.ScenePath) || entry.Scene.ScenePath == gameObject.scene.path)
				return;

			var shouldLoad = entry.LoadInEditor;
			var scene = entry.Scene.Scene;

			try
			{
				if (!scene.IsValid())
				{
					if (shouldLoad)
					{
						/*Debug.LogFormat("Scene {0} is loading Scene {1} in Editor", gameObject.scene.name, entry.Scene.Name);*/
						EditorSceneManager.OpenScene(entry.Scene.ScenePath, OpenSceneMode.Additive);
					}
					else
					{
						/*Debug.LogFormat(this, "Scene {0} is opening Scene {1} (without loading) in Editor", gameObject.scene.name, entry.Scene.Name);*/
						EditorSceneManager.OpenScene(entry.Scene.ScenePath, OpenSceneMode.AdditiveWithoutLoading);
					}
				}
				else if (shouldLoad != scene.isLoaded)
				{
					if (shouldLoad && !scene.isLoaded)
					{
						/*Debug.LogFormat(this, "Scene {0} is loading existing Scene {1} in Editor", gameObject.scene.name, entry.Scene.Name);*/
						EditorSceneManager.OpenScene(entry.Scene.ScenePath, OpenSceneMode.Additive);
					}
					else
					{
						/*Debug.LogFormat(this, "Scene {0} is closing Scene {1} in Editor", gameObject.scene.name, entry.Scene.Name);*/
						EditorSceneManager.CloseScene(scene, false);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		[Button(ButtonSizes.Large, ButtonStyle.Box), PropertyOrder(200)]
		public virtual void SaveSceneSetup()
		{
			var activeScene = SceneManager.GetActiveScene();
			var editorSceneSetup = EditorSceneManager.GetSceneManagerSetup();
			var newSetup = new List<SceneEntry>();

			foreach (var editorEntry in editorSceneSetup)
			{
				if (string.Equals(editorEntry.path, activeScene.path))
					continue;

				var newEntry = new SceneEntry(editorEntry);
				newSetup.Add(newEntry);

				var oldEntry = _sceneSetup.Find(x => newEntry.Scene.Scene.path.Equals(x.Scene.Scene.path));

				if (oldEntry != null)
				{
					newEntry.LoadMethod = oldEntry.LoadMethod;
					// newEntry.LoadInEditor = oldEntry.LoadInEditor;
				}
			}

			_sceneSetup = newSetup;
		}
#endif
	}
}
#endif