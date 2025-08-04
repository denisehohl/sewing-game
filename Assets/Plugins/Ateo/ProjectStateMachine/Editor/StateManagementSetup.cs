using System;
using System.IO;
using System.Linq;
using System.Text;
using Ateo.CodeGeneration;
using UnityEditor;
using UnityEngine;

namespace Ateo.StateManagement
{
    public static class StateManagementSetup
    {
        public static readonly string[] Symbols = new string[]
        {
            "STATEMACHINE"
        };

        public const string AsmRefProjectStateMachine = @"{
    ""reference"": ""Ateo.ProjectStateMachine""
}
";

        public const string AsmDefDoozy = @"{
    ""name"": ""Doozy"",
    ""references"": [
        ""DOTween.Modules""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}
";

        public const string AsmDefDoozyEditor = @"{
    ""name"": ""Doozy.Editor"",
    ""rootNamespace"": """",
    ""references"": [
        ""Doozy"",
        ""DOTween.Modules""
    ],
    ""includePlatforms"": [
        ""Editor""
    ],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}
";

        public static readonly string PathDoozy = Path.Combine(Application.dataPath, "Doozy");
        public static readonly string PathDoozyEditor = Path.Combine(PathDoozy, "Editor");
        
        public const string AsmRefFilename = "Ateo.ProjectStateMachine.Reference.asmref";
        public const string AsmRefFilenameDoozy = "Doozy.asmdef";
        public const string AsmRefFilenameDoozyEditor = "Doozy.Editor.asmdef";

        [MenuItem("Tools/State Management/Setup", false, 0)]
        private static void Setup()
        {
            if (!Directory.Exists(CodeGeneratorStates.FolderPath))
            {
                Directory.CreateDirectory(CodeGeneratorStates.FolderPath);
            }

            WriteFile(CodeGeneratorStates.FolderPath, AsmRefFilename, AsmRefProjectStateMachine);

/*#if dUI_MANAGER*/
            WriteFile(PathDoozy, AsmRefFilenameDoozy, AsmDefDoozy);
            WriteFile(PathDoozyEditor, AsmRefFilenameDoozyEditor, AsmDefDoozyEditor);
/*#endif*/

            CodeGeneratorStates.GenerateStates();
            AddDefineSymbols();

            var gos = new GameObject {name = "Code Generator States"};
            gos.AddComponent<CodeGeneratorStatesBehaviour>();
            gos.tag = "EditorOnly";
        }

        private static void AddDefineSymbols()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public static void WriteFile(string path, string filename, string text)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                // Always create a new file because overwriting to existing file may generate mal-formatted script.
                // for instance, when the number of tags is reduced, last tag will be remain after the last curly brace in the file.
                using (var stream = File.Open(Path.Combine(path, filename), FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var builder = new StringBuilder();
                        builder.Append(text);
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
    }
}