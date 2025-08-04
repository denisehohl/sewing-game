#if SCENESETUP
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;

#endif

namespace Ateo.Common
{
	[Serializable]
	public class SceneSetupModule
	{
		[ValidateInput("ValidateName", "Name already exists in another module")]
		public string Name;

		[OnValueChanged("LoadAtStartChanged")]
		public bool LoadAtStart;

		public List<SceneSetup.SceneEntry> SceneSetup = new List<SceneSetup.SceneEntry>();

		[SerializeField, HideInInspector]
		private SceneSetupModular _sceneSetup;

#if UNITY_EDITOR
		private bool ValidateName(string name)
		{
			if (_sceneSetup != null)
			{
				foreach (var module in _sceneSetup.SceneSetupModules)
				{
					if (module != this && string.Equals(name, module.Name))
					{
						return false;
					}
				}
			}

			return true;
		}

		private void LoadAtStartChanged()
		{
			if (_sceneSetup != null && LoadAtStart)
			{
				foreach (var module in _sceneSetup.SceneSetupModules)
				{
					if (module != this)
					{
						module.LoadAtStart = false;
					}
				}
			}
		}
#endif

		public void Initialize(SceneSetupModular sceneSetup)
		{
			if (_sceneSetup == null)
				_sceneSetup = sceneSetup;
		}

		[Button, ButtonGroup()]
		private void Load()
		{
			if (_sceneSetup != null)
			{
				_sceneSetup.SetActiveModule(this);
			}
		}

		[Button, ButtonGroup()]
		private void Unload()
		{
			if (_sceneSetup != null)
			{
				_sceneSetup.SetActiveModule((SceneSetupModule) default);
			}
		}
	}

	[ExecuteInEditMode]
	public sealed class SceneSetupModular : SceneSetup
	{
		[FormerlySerializedAs("_sceneSetupModules"), PropertyOrder(101), OnValueChanged("SetupModules")]
		public List<SceneSetupModule> SceneSetupModules = new List<SceneSetupModule>();

		[NonSerialized]
		private readonly List<SceneEntry> _sceneSetupWithModules = new List<SceneEntry>();

		[NonSerialized]
		private SceneSetupModule _activeModule;

		private static SceneSetupModular _instance;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			_instance = null;
		}
#endif

		public new static SceneSetupModular Instance
		{
			get
			{
				if (_instance == null)
					_instance = (SceneSetupModular) SceneSetup.Instance;

				return _instance;
			}
		}

		public void SetActiveModule(string moduleName, bool loadSceneSetup = true)
		{
			foreach (var module in SceneSetupModules)
			{
				if (string.Equals(moduleName, module.Name))
				{
					SetActiveModule(module, loadSceneSetup);
					break;
				}
			}
		}

		public void SetActiveModule(int index, bool loadSceneSetup = true)
		{
			if (index >= 0 && index < SceneSetupModules.Count)
			{
				SetActiveModule(SceneSetupModules[index], loadSceneSetup);
			}
		}

		public void SetActiveModule(SceneSetupModule module, bool loadSceneSetup = true)
		{
			_activeModule = module;
			_sceneSetupWithModules.Clear();

			foreach (var sceneEntry in _sceneSetup)
			{
				_sceneSetupWithModules.Add(sceneEntry);
			}

			if (_activeModule != null)
			{
				foreach (var sceneEntry in _activeModule.SceneSetup)
				{
					_sceneSetupWithModules.Add(sceneEntry);
				}
			}

			if (loadSceneSetup)
			{
				LoadSceneSetup(false);
			}
		}

		private void SetupModules()
		{
			foreach (var module in SceneSetupModules)
			{
				module.Initialize(this);
			}
		}

		public override ReadOnlyCollection<SceneEntry> GetSceneSetup()
		{
			return _sceneSetupWithModules.AsReadOnly();
		}

		public override void Reload()
		{
			foreach (var module in SceneSetupModules)
			{
				if (module.LoadAtStart)
				{
					SetActiveModule(module, false);
					base.Reload();
					return;
				}
			}

			SetActiveModule((SceneSetupModule) default, false);
			base.Reload();
		}

		protected override void Awake()
		{
			foreach (var module in SceneSetupModules)
			{
				if (module.LoadAtStart)
				{
					SetActiveModule(module, false);
					base.Awake();
					return;
				}
			}

			SetActiveModule((SceneSetupModule) default, false);
			base.Awake();
		}

		protected override void Start()
		{
			foreach (var module in SceneSetupModules)
			{
				if (module.LoadAtStart)
				{
					SetActiveModule(module, false);
					base.Start();
					return;
				}
			}

			SetActiveModule((SceneSetupModule) default, false);
			base.Start();
		}

#if UNITY_EDITOR
		protected override void LoadSceneSetupInEditor()
		{
			var sceneSetup = GetSceneSetup();

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				var unloadScene = true;

				if (string.Equals(gameObject.scene.path, scene.path))
				{
					continue;
				}

				foreach (var entry in sceneSetup)
				{
					if (string.Equals(entry.Scene.Scene.path, scene.path))
					{
						unloadScene = false;
					}
				}

				if (unloadScene)
				{
					EditorSceneManager.CloseScene(scene, false);
				}
			}

			base.LoadSceneSetupInEditor();
		}

		public override void SaveSceneSetup()
		{
			var activeScene = SceneManager.GetActiveScene();
			var editorSceneSetup = EditorSceneManager.GetSceneManagerSetup();
			var newSetup = new List<SceneEntry>();

			foreach (var editorEntry in editorSceneSetup)
			{
				if (string.Equals(editorEntry.path, activeScene.path))
					continue;

				var add = true;

				foreach (var module in SceneSetupModules)
				{
					if (!add) break;

					foreach (var entry in module.SceneSetup)
					{
						if (string.Equals(entry.Scene.Scene.path, editorEntry.path))
						{
							add = false;
							break;
						}
					}
				}

				if (!add) break;

				var newEntry = new SceneEntry(editorEntry);
				newSetup.Add(newEntry);

				var oldEntry = _sceneSetup.Find(x => newEntry.Scene.Scene.path.Equals(x.Scene.Scene.path));

				if (oldEntry != null)
				{
					newEntry.LoadMethod = oldEntry.LoadMethod;
					newEntry.LoadInEditor = oldEntry.LoadInEditor;
				}
			}

			_sceneSetup = newSetup;
		}

		public IList<ValueDropdownItem<string>> GetModules()
		{
			var list = new ValueDropdownList<string>();

			foreach (var module in SceneSetupModules)
			{
				if (module != null && !string.IsNullOrEmpty(module.Name))
					list.Add(module.Name, module.Name);
			}

			return list;
		}
#endif
	}
}
#endif