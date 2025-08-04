// Original source: https://sassybot.com/blog/light-baking-automation-in-unity-5/

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Ateo.Common.Editor
{
	public class BakeScenes : OdinEditorWindow
	{
		public enum Status
		{
			Idle,
			Baking,
			Cancelled,
			Error,
			Done
		}

		// Array to store an Object array of the scenes
		[SerializeField]
		private List<Object> _scenes = new List<Object>();

		// Lists and string array for easier management
		private readonly List<string> _sceneList = new List<string>();
		private int _sceneIndex;

		// Editor text
		private Status _status = Status.Idle;
		private DateTime _timeStamp;

		private string BakeButton => Lightmapping.isRunning ? "Cancel" : "Bake";

		[MenuItem("Tools/Editor Tools/Bake Scenes")]
		public static void ShowWindow()
		{
			EditorWindow window = GetWindow(typeof(BakeScenes), false, "Bake Scenes");
			window.autoRepaintOnSceneChange = true;
		}

		private void OnFocus()
		{
			if (_status != Status.Baking)
			{
				_status = Status.Idle;
			}
		}

		protected override void OnImGUI()
		{
			GUILayout.Space(19);

			GUILayout.BeginHorizontal();

			GUIStyle headStyle = new GUIStyle(EditorStyles.label)
			{
				fontSize = 19,
				fontStyle = FontStyle.Bold
			};

			GUILayout.Label("Bake Scenes", headStyle);

			GUILayout.FlexibleSpace();

			string status;
			
			if (_status == Status.Idle)
				status = "Ready";
			else if (_status == Status.Baking)
				status = $"Baking ({_sceneIndex + 1}/{_scenes.Count}) - {SceneManager.GetActiveScene().name}";
			else if (_status == Status.Cancelled)
				status = "Cancelled";
			else if (_status == Status.Error)
				status = "Error";
			else if (_status == Status.Done)
				status = $"Done";
			else
				throw new ArgumentOutOfRangeException();

			Color c = GUI.contentColor;

			if (_status == Status.Idle)
				GUI.contentColor = Color.white;
			else if (_status == Status.Baking)
				GUI.contentColor = Color.green;
			else if (_status == Status.Cancelled)
				GUI.contentColor = Color.yellow;
			else if (_status == Status.Error)
				GUI.contentColor = Color.red;
			else if (_status == Status.Done)
				GUI.contentColor = Color.green;
			else
				throw new ArgumentOutOfRangeException();

			GUILayout.Label($"Status: {status}", EditorStyles.largeLabel);
			GUI.contentColor = c;

			GUILayout.EndHorizontal();

			GUILayout.Space(16f);

			base.OnImGUI();
		}

		[Button("$BakeButton", ButtonSizes.Large), PropertySpace(SpaceBefore = 8)]
		private void InitializeBake()
		{
			if (!Lightmapping.isRunning)
			{
				Lightmapping.bakeCompleted += OnBakeCompleted;

				_sceneList.Clear();
				_sceneIndex = -1;

				if (_scenes.Count == 0)
				{
					_status = Status.Idle;
				}
				else
				{
					_status = Status.Baking;

					foreach (Object t in _scenes)
					{
						_sceneList.Add(AssetDatabase.GetAssetPath(t));
					}

					BakeNewScene();
				}
			}
			else
			{
				_status = Status.Cancelled;
				Lightmapping.Cancel();
				DoneBaking();
			}
		}

		private void BakeNewScene()
		{
			if (_sceneIndex < _scenes.Count - 1)
			{
				_sceneIndex++;
				_timeStamp = DateTime.Now;

				EditorSceneManager.OpenScene(_sceneList[_sceneIndex]);
				Lightmapping.BakeAsync();

				if (!Lightmapping.isRunning)
				{
					_status = Status.Error;
					DoneBaking();
				}
			}
			else
			{
				_status = Status.Done;
				DoneBaking();
			}
		}

		private void OnBakeCompleted()
		{
			TimeSpan bakeSpan = DateTime.Now - _timeStamp;
			string bakeTime = $"{bakeSpan.Hours:D2}:{bakeSpan.Minutes:D2}:{bakeSpan.Seconds:D2}";

			Debug.Log(
				$"({_sceneIndex + 1}/{_scenes.Count}) Done baking: {SceneManager.GetActiveScene().name} after {bakeTime} on {DateTime.Now.ToString()}");
			EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

			BakeNewScene();
		}

		private void DoneBaking()
		{
			Lightmapping.bakeCompleted -= OnBakeCompleted;
		}
	}
}