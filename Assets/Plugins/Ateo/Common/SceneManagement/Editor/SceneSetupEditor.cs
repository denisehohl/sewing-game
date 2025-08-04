using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.Common.Editor
{
    public class SceneSetupEditor
    {
        private const string ScriptingDefineSymbol = "SCENESETUP";
#if !SCENESETUP
        [MenuItem("Tools/Scene Management/Initialize Scene Setup", false, 0)]
        private static void SetupMultiScenes()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);

            EditorHelper.AddScriptingDefineSymbol(group, ScriptingDefineSymbol);
        }
#else
        [MenuItem("Tools/Scene Management/Create Scene Setup", false, 0)]
        private static void CreateSceneSetup()
        {
            var gos = new GameObject()
            {
                name = "! SceneSetup"
            };

            var sceneSetup = gos.AddComponent<SceneSetup>();
            sceneSetup.SaveSceneSetup();

            Undo.RegisterCreatedObjectUndo(gos, "Created Scene Setup");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
#endif
    }
}