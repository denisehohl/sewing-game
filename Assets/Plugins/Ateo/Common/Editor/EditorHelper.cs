using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ateo.Common.Editor
{
	public static class EditorHelper
	{
		#region Selection

		public static string GetSelectedPath(string fallback = default)
		{
			string path = fallback;

			foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);

				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}

			return path;
		}

		#endregion

		#region Input / Output

		public static List<string> GetAssetPathsInsideDirectory(string path, string extension = default, bool recursive = false)
		{
			List<string> paths = new List<string>();

			foreach (string file in Directory.GetFiles(path))
			{
				FileInfo fileInfo = new FileInfo(file);

				if (extension == default || fileInfo.Extension == extension)
				{
					paths.Add($"Assets/{fileInfo.FullName.Remove(0, Application.dataPath.Length + 1)}");
				}
			}

			if (recursive)
			{
				foreach (string directory in Directory.GetDirectories(path))
				{
					paths.AddRange(GetAssetPathsInsideDirectory(directory, extension, true));
				}
			}

			return paths;
		}

		#endregion

		#region Project Settings

		#region Scripting Define Symbols

		public static void AddScriptingDefineSymbol(string define)
		{
			AddScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, define);
		}

		public static void AddScriptingDefineSymbol(BuildTargetGroup group, string define)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
			List<string> allDefines = definesString.Split(';').ToList();

			if (!allDefines.Contains(define))
			{
				allDefines.Add(define);
			}

			allDefines.Sort();

			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", allDefines.ToArray()));
			EditorApplication.ExecuteMenuItem("File/Save Project");
		}

		public static void RemoveScriptingDefineSymbol(string define)
		{
			RemoveScriptingDefineSymbol(EditorUserBuildSettings.selectedBuildTargetGroup, define);
		}

		public static void RemoveScriptingDefineSymbol(BuildTargetGroup group, string define)
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
			List<string> allDefines = definesString.Split(';').ToList();

			if (allDefines.Contains(define))
			{
				allDefines.Remove(define);
			}

			allDefines.Sort();

			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", allDefines.ToArray()));
			EditorApplication.ExecuteMenuItem("File/Save Project");
		}

		#endregion

		#region Audio Settings

		public enum DSPBufferSizes
		{
			BestLatency = 256,
			Default = 512,
			BestPerformance = 1024
		}

		public static void SetAudioDSPBufferSize(DSPBufferSizes bufferSize)
		{
			AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
			audioConfiguration.dspBufferSize = (int) bufferSize;
			AudioSettings.Reset(audioConfiguration);
		}

		#endregion

		#endregion
	}
}