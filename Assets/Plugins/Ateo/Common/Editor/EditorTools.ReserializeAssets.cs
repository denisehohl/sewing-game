using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ateo.Common.Editor
{
	public static partial class EditorTools
	{
		private const string EXT_SCRIPTABLE_OBJECT = ".asset";
		private const string EXT_MATERIAL = ".mat";
		private const string EXT_PREFAB = ".prefab";
		private const string EXT_SCENE = ".unity";

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder/All")]
		private static void ReserializeAssets() =>
			ReserializeAssets(EditorHelper.GetSelectedPath(), false, EXT_SCRIPTABLE_OBJECT, EXT_MATERIAL, EXT_PREFAB, EXT_SCENE);

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder [Recursive]/All")]
		private static void ReserializeAssetsRecursive() =>
			ReserializeAssets(EditorHelper.GetSelectedPath(), true, EXT_SCRIPTABLE_OBJECT, EXT_MATERIAL, EXT_PREFAB, EXT_SCENE);


		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder/Scriptable Objects")]
		private static void ReserializeScriptableObjects() => ReserializeAssets(EditorHelper.GetSelectedPath(), false, EXT_SCRIPTABLE_OBJECT);

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder [Recursive]/Scriptable Objects")]
		private static void ReserializeScriptableObjectRecursive() => ReserializeAssets(EditorHelper.GetSelectedPath(), true, EXT_SCRIPTABLE_OBJECT);


		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder/Materials")]
		private static void ReserializeMaterials() => ReserializeAssets(EditorHelper.GetSelectedPath(), false, EXT_MATERIAL);

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder [Recursive]/Materials")]
		private static void ReserializeMaterialsRecursive() => ReserializeAssets(EditorHelper.GetSelectedPath(), true, EXT_MATERIAL);


		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder/Prefabs")]
		private static void ReserializePrefabs() => ReserializeAssets(EditorHelper.GetSelectedPath(), false, EXT_PREFAB);

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder [Recursive]/Prefabs")]
		private static void ReserializePrefabsRecursive() => ReserializeAssets(EditorHelper.GetSelectedPath(), true, EXT_PREFAB);


		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder/Scenes")]
		private static void ReserializeScenes() => ReserializeAssets(EditorHelper.GetSelectedPath(), false, EXT_SCENE);

		[MenuItem("Tools/Editor Tools/Reserialize Assets in Folder [Recursive]/Scenes")]
		private static void ReserializeScenesRecursive() => ReserializeAssets(EditorHelper.GetSelectedPath(), true, EXT_SCENE);


		public static void ReserializeAssets(string path, bool recursive, params string[] extensions)
		{
			if (!string.IsNullOrEmpty(path))
			{
				string folder = path.Remove(0, "Assets/".Length);
				string directory = Path.Combine(Application.dataPath, folder);

				foreach (string extension in extensions)
				{
					List<string> paths = EditorHelper.GetAssetPathsInsideDirectory(directory, extension, recursive);
					paths.Sort();

					foreach (string p in paths)
					{
						Debug.Log(p);
					}

					AssetDatabase.ForceReserializeAssets(paths);
				}
			}
			else
			{
				Debug.Log("No folder selected");
			}
		}
	}
}