#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ateo.CodeGeneration
{
	public class CodeGeneratorCommon
	{
		public List<string> Names = new List<string>();

		public double NextCheckTime = 0.0;


		// directory at witch auto-generated scripts are placed
		public const string DirPath = "Assets/03_Scripts/";

		// file header format
		public const string AutoGenFormat =
			@"//-----------------------------------------------------------------------
// This file is AUTO-GENERATED.
// Changes for this script by hand might be lost when auto-generation is run.
// Generated date: {0}
//-----------------------------------------------------------------------
";

		public const string StateFormat =
@"using UnityEngine;

namespace Ateo.StateManagement
{{
	public sealed class {0} : State<{0}>
	{{
		public override IState StateParent => {1};
		public override IState StateNext => null;
		public override IState StateBack => null;

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{{
			Instance = new {0}();
		}}
#endif
	}}
}}
";

		public const string PathFormat = "{0}/{1}/";
		public const string FileFormat = "{0}.cs";
		public const string NameSpaceFormat = "namespace {0}";

		public const float CheckIntervalSec = 3f;

		public const string StringPrefix = "Str";

		// header
		public static string AutoGenTemplate => string.Format(AutoGenFormat, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

		// new state
		public static string StateTemplate(string stateName) => string.Format(StateFormat, stateName);

		// namespace
		public static string NameSpaceTemplate => $"namespace {PlayerSettings.companyName}.{PlayerSettings.productName}";


		// writes a file to the project folder
		public static void WriteCodeFile(string path, string filename, Action<StringBuilder> callback)
		{
			Debug.Assert(callback != null);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			try
			{
				// Always create a new file because overwriting to existing file may generate mal-formatted script.
				// for instance, when the number of tags is reduced, last tag will be remain after the last curly brace in the file.
				using (FileStream stream = File.Open(Path.Combine(path, filename), FileMode.Create, FileAccess.Write))
				{
					using (StreamWriter writer = new StreamWriter(stream))
					{
						StringBuilder builder = new StringBuilder();
						callback?.Invoke(builder);
						writer.Write(builder.ToString());
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);

				// if we have an error, it is certainly that the file is screwed up. Delete to be save
				//if (File.Exists(path))
				//{
				//    File.Delete(path);
				//}
			}

			AssetDatabase.Refresh();
		}

		// check if names are changed
		public static bool SomethingHasChanged(List<string> a, List<string> b)
		{
			if (a.Count != b.Count)
			{
				return true;
			}

			// loop thru all new tags and compare them to the old ones
			for (int i = 0; i < a.Count; i++)
			{
				if (!string.Equals(a[i], b[i]))
				{
					return true;
				}
			}

			return false;
		}

		public static string MakeIdentifier(string str)
		{
			string result = Regex.Replace(str, "[^a-zA-Z0-9]", "_");
			if ('0' <= result[0] && result[0] <= '9')
			{
				result = result.Insert(0, "_");
			}

			return result;
		}

		public static string EscapeDoubleQuote(string str)
		{
			return str.Replace("\"", "\"\"");
		}
	}
}
#endif