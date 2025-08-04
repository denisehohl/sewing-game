using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ateo.Extensions
{
	/// <summary>
	/// GameObject Extensions.  Useful functionality for Unity's GameObject class.
	/// </summary>
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Reusable list of GameObjects for non-allocating calls.
		/// </summary>
		private static List<GameObject> s_GameObjects = new List<GameObject>();

#if !UNITY_2017_1_OR_NEWER
		/// <summary>
		/// Store this method as a way of getting the root game objects in a scene before the scene is considered loaded
		/// </summary>
		private static System.Reflection.MethodInfo	s_GetRootGameObjectsInternal;

		static GameObjectEx()
		{
#if NETFX_CORE
			s_GetRootGameObjectsInternal = typeof(Scene).GetTypeInfo().DeclaredMethods.FirstOrDefault( x => x.Name == "GetRootGameObjectsInternal");
#else
			s_GetRootGameObjectsInternal =
 typeof(Scene).GetMethod( "GetRootGameObjectsInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic );
#endif
		}
#endif

		/**
		 * Get the full path from the root to this GameObject
		 */
		public static string GetFullName(this GameObject gameObj)
		{
			return gameObj.transform.FullPath();
		}

		/// <summary>
		/// Create a GameObject in a specific Scene
		/// </summary>
		/// <param name="name">The object name</param>
		/// <param name="scene">The target scene to create the object in</param>
		/// <returns>The GameObject instance</returns>
		public static GameObject CreateGameObjectInScene(string name, Scene scene)
		{
			Scene oldActiveScene = SceneManager.GetActiveScene();
			SceneManager.SetActiveScene(scene);
			GameObject gameObject = new GameObject(name);
			SceneManager.SetActiveScene(oldActiveScene);

			return gameObject;
		}

		public static T GetSceneSingleton<T>(this Scene scene, bool bCreate) where T : MonoBehaviour
		{
			// Find the existing instance
			foreach (GameObject gameObj in scene.GetRootGameObjects())
			{
				T instance = gameObj.GetComponent<T>();
				if (instance)
					return instance;
			}

			// Create a new one
			if (bCreate)
			{
				GameObject gameObj = CreateGameObjectInScene("! " + typeof(T).Name, scene);
				return gameObj.AddComponent<T>();
			}

			// None found
			return null;
		}

		/// <summary>
		/// Unity by default will not return root objects during the initialzation phase of a Scene (i.e. before Awake has completed).
		/// However, we definitely want to know during the Awake() period, so we go into the internal code to do so.
		/// </summary>
		/// <param name="scene">The scene to get root objects for</param>
		/// <returns>The root objects</returns>
		public static List<GameObject> GetRootObjectsEvenIfNotLoaded(this Scene scene)
		{
			s_GameObjects.Clear();

			if (scene.IsValid())
			{
				s_GameObjects.Capacity = Mathf.Max(s_GameObjects.Capacity, scene.rootCount);
#if UNITY_2017_1_OR_NEWER
				scene.GetRootGameObjects(s_GameObjects);
#else
				s_GetRootGameObjectsInternal.Invoke( null, new object[] { scene.GetHashCode(), s_GameObjects } );
#endif
			}

			return s_GameObjects;
		}

		/// <summary>
		/// Gets the component on a given GameObject or creates it if it doesn't already exist.
		/// </summary>
		/// <typeparam name="T">The type of MonoBehaviour to look for (or add)</typeparam>
		/// <param name="gameObject">The GameObject to check/add</param>
		/// <returns>The instance of T (which either existed or was created)</returns>
		public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component
		{
			T instance = gameObject.GetComponent<T>();
			return instance != null ? instance : gameObject.AddComponent<T>();
		}

		/// <summary>
		/// Find a GameObject by an absolute path regardless of if that GameObject is disabled or not.
		/// </summary>
		/// <param name="absolutePath">The absolute path (e.g. "/Path/To/Object" or "Scene/Path/To/Object")</param>
		/// <returns>The GameObject if found, null otherwise</returns>
		public static GameObject FindByAbsolutePath(string absolutePath)
		{
			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				GameObject gameObj = FindBySceneAndPath(SceneManager.GetSceneAt(i), absolutePath);
				if (gameObj != null)
					return gameObj;
			}

			return null;
		}

		/// <summary>
		/// Find a GameObject in a specific scene with an absolute path.
		/// </summary>
		/// <param name="scene">The scene to search</param>
		/// <param name="absolutePath">The absolute path of the GameObject in the scene</param>
		/// <returns>The GameObject if found, null otherwise</returns>
		public static GameObject FindBySceneAndPath(Scene scene, string absolutePath)
		{
			string[] paths = absolutePath.Split('/');
			int numPaths = paths.Length;

			// Fail-safe
			if (numPaths < 1)
				return null;
			else if (numPaths < 2)
				paths = new string[2] {"/", paths[0]};

			foreach (GameObject gameObject in scene.GetRootObjectsEvenIfNotLoaded())
			{
				if (gameObject.name != paths[1])
					continue;

				Transform transform = gameObject.transform;
				for (int deep = 2; deep < numPaths && transform; ++deep)
				{
					transform = transform.Find(paths[deep]);
				}

				if (transform)
					return transform.gameObject;
			}

			return null;
		}

#if UNITY_EDITOR
		/**
		 * @Return true if this GameObject lives in the Scene.  False if it's part of an asset.
		 */
		public static bool EditorIsSceneObject(this GameObject gameObj)
		{
			string assetPath = AssetDatabase.GetAssetOrScenePath(gameObj);
			return !EditorUtility.IsPersistent(gameObj) || assetPath.EndsWith(".unity");
		}

		/**
		 * Get a nice, human-readable path for this GameObject. Takes into account if the GameObject lives in the Scene or an Asset.
		 */
		public static string EditorGetFriendlyPath(this GameObject gameObj)
		{
			if (EditorIsSceneObject(gameObj))
			{
				return GetFullName(gameObj) + " (Scene Object)";
			}
			else
			{
				string assetPath = AssetDatabase.GetAssetOrScenePath(gameObj);
				return assetPath + GetFullName(gameObj) + " (Asset Object)";
			}
		}

		/// <summary>
		/// Hack that returns all ScriptableObjects of a type attached to a GameObject (usually not allowed by the GUI)
		/// </summary>
		/// <typeparam name="T">The type of ScriptableObject to query for</typeparam>
		/// <param name="gameObj">The GameObject on which they are attached</param>
		/// <returns>A list of ScriptableObjects of type T attached on a specific GameObject</returns>
		public static IEnumerable<T> EditorGetScriptableObjects<T>(this GameObject gameObj) where T : ScriptableObject
		{
			List<T> returnList = new List<T>();

			SerializedObject so = new SerializedObject(gameObj);
			SerializedProperty spComponents = so.FindProperty("m_Component");

			for (int i = 0; i < spComponents.arraySize; ++i)
			{
				// For some reason the Components has a first/second pair...
				SerializedProperty spElement = spComponents.GetArrayElementAtIndex(i);
				SerializedProperty spObjRef = spElement.FindPropertyRelative("second");

				T scriptObj = spObjRef.objectReferenceValue as T;
				if (scriptObj)
				{
					returnList.Add(scriptObj);
				}
			}

			return returnList;
		}

		/// <summary>
		/// This will give you a GameObject from a Component instance (or null if it's not attached).
		/// </summary>
		/// <param name="obj"> The object is a 'component' but can be our hack of being a ScriptableObject </param>
		/// <returns> The GameObject the component is attached to </returns>
		public static GameObject EditorGetGameObjectFromComponent(this Object obj)
		{
			if (!obj)
				return null;

			int gameObjID = UnityEditorInternal.InternalEditorUtility.GetGameObjectInstanceIDFromComponent(obj.GetInstanceID());
			return gameObjID != 0 ? (GameObject) EditorUtility.InstanceIDToObject(gameObjID) : (obj as GameObject);
		}
#endif
		
		// This method is not precise when the GameObject is rotated
		public static Bounds GetBounds(this GameObject gos)
		{
			Bounds bounds = GetRenderBounds(gos);

			if (bounds.extents.x == 0)
			{
				bounds = new Bounds(gos.transform.position, Vector3.zero);

				foreach (Transform child in gos.transform.GetComponentsInChildren<Transform>())
				{
					if (child == gos.transform) continue;

					Renderer childRender = child.GetComponent<Renderer>();
					bounds.Encapsulate(childRender != null ? childRender.bounds : GetBounds(child.gameObject));
				}
			}

			Bounds GetRenderBounds(GameObject g)
			{
				Bounds b = new Bounds(Vector3.zero, Vector3.zero);
				Renderer r = g.GetComponent<Renderer>();
				return r != null ? r.bounds : b;
			}

			return bounds;
		}

		// This method is precise, but performance intensive
		public static Bounds GetBoundsFromMesh(this GameObject gos)
		{
			Bounds bounds = GetRenderBounds(gos);

			if (bounds.extents.x == 0)
			{
				bounds = new Bounds(gos.transform.position, Vector3.zero);

				foreach (Transform child in gos.transform.GetComponentsInChildren<Transform>())
				{
					if (child == gos.transform) continue;

					bounds.Encapsulate(GetRenderBounds(child.gameObject));
				}
			}

			Bounds GetRenderBounds(GameObject g)
			{
				return GetBoundsFromMesh(g.GetComponent<MeshFilter>(), g.transform);
			}

			return bounds;
		}

		private static Bounds GetBoundsFromMesh(MeshFilter meshFilter, Transform transform)
		{
			return meshFilter != null ? GetBoundsFromMesh(meshFilter.sharedMesh, transform) : new Bounds();
		}

		private static Bounds GetBoundsFromMesh(Mesh mesh, Transform transform)
		{
			Bounds bounds = new Bounds();

			if (mesh == null || transform == null)
			{
				return bounds;
			}

			Vector3[] vertices = mesh.vertices;

			if (vertices.Length <= 0)
			{
				return bounds;
			}

			// TransformPoint converts the local mesh vertice dependent on the transform
			// position, scale and orientation into a global position
			Vector3 min = transform.TransformPoint(vertices[0]);
			Vector3 max = min;

			// Iterate through all vertices
			// except first one
			for (int i = 1; i < vertices.Length; i++)
			{
				Vector3 v = transform.TransformPoint(vertices[i]);

				// Go through X,Y and Z of the Vector3
				for (int n = 0; n < 3; n++)
				{
					max[n] = Mathf.Max(v[n], max[n]);
					min[n] = Mathf.Min(v[n], min[n]);
				}
			}

			bounds.SetMinMax(min, max);
			return bounds;
		}

		public static T GetOrAddComponent<T>(this GameObject gos) where T : Component
		{
			T component = gos.GetComponent<T>();

			if (component == null)
				component = gos.AddComponent<T>() as T;

			return component;
		}
		
		public static void DisableSelfCollisions(this GameObject gos)
		{
			Collider[] colliders = gos.GetComponentsInChildren<Collider>();

			foreach (Collider a in colliders)
			{
				foreach (Collider b in colliders)
				{
					Physics.IgnoreCollision(a, b);
				}
			}
		}
		
		public static void DisableSelfCollisions(this GameObject gos, LayerMask layerMask)
		{
			Collider[] colliders = gos.GetComponentsInChildren<Collider>();

			foreach (Collider c in colliders)
			{
				if (!layerMask.Contains(c.gameObject.layer)) continue;
				
				foreach (Collider d in colliders)
				{
					if (!layerMask.Contains(d.gameObject.layer)) continue;
					Physics.IgnoreCollision(c, d);
				}
			}
		}
	}
}