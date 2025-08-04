using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HiddenGameObjectTool : EditorWindow
{
	#region Menu Command

	[MenuItem("Tools/Editor Tools/Hidden GameObject Tool")]
	public static void ShowWindow()
	{
		HiddenGameObjectTool window = GetWindow<HiddenGameObjectTool>();
		window.titleContent = new GUIContent("Hidden GameObjects");
		window.GatherHiddenObjects();
	}

	#endregion

	#region GUI

	private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(80);
	private static readonly GUILayoutOption BigButtonHeight = GUILayout.Height(32);

	private void OnGUI()
	{
		GUILayout.Space(19);

		GUILayout.BeginHorizontal();

		GUIStyle headStyle = new GUIStyle(EditorStyles.label)
		{
			fontSize = 19,
			fontStyle = FontStyle.Bold
		};

		GUILayout.Label("Hidden GameObjects", headStyle);

		GUILayout.EndHorizontal();

		GUILayout.Space(16f);
		
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Refresh", BigButtonHeight))
			{
				GatherHiddenObjects();
			}

			if (GUILayout.Button("Test", BigButtonHeight, ButtonWidth))
			{
				GameObject go = new GameObject("HiddenTestObject")
				{
					hideFlags = HideFlags.HideInHierarchy
				};

				GatherHiddenObjects();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);

		EditorGUILayout.LabelField("Hidden Objects (" + _hiddenObjects.Count + ")", EditorStyles.boldLabel);

		foreach (GameObject hiddenObject in _hiddenObjects)
		{
			GUILayout.BeginHorizontal();
			{
				bool gone = hiddenObject == null;
				GUILayout.Label(gone ? "null" : hiddenObject.name);
				GUILayout.FlexibleSpace();

				if (gone)
				{
					GUILayout.Box("Select", ButtonWidth);
					GUILayout.Box("Reveal", ButtonWidth);
					GUILayout.Box("Delete", ButtonWidth);
				}
				else
				{
					if (GUILayout.Button("Select", ButtonWidth))
					{
						Selection.activeGameObject = hiddenObject;
					}

					if (GUILayout.Button(IsHidden(hiddenObject) ? "Reveal" : "Hide", ButtonWidth))
					{
						hiddenObject.hideFlags ^= HideFlags.HideInHierarchy;
						EditorSceneManager.MarkSceneDirty(hiddenObject.scene);
					}

					if (GUILayout.Button("Delete", ButtonWidth))
					{
						Scene scene = hiddenObject.scene;
						DestroyImmediate(hiddenObject);
						EditorSceneManager.MarkSceneDirty(scene);
					}
				}
			}
			GUILayout.EndHorizontal();
		}
	}

	#endregion

	#region Hidden Objects

	private readonly List<GameObject> _hiddenObjects = new List<GameObject>();

	private void GatherHiddenObjects()
	{
		_hiddenObjects.Clear();

#if UNITY_2020_1_OR_NEWER
		GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
#else
		GameObject[] allObjects = FindObjectsOfType<GameObject>();
#endif

		foreach (GameObject go in allObjects)
		{
			if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
			{
				_hiddenObjects.Add(go);
			}
		}

		Repaint();
	}

	private static bool IsHidden(Object go)
	{
		return (go.hideFlags & HideFlags.HideInHierarchy) != 0;
	}

	#endregion
}