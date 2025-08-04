#if SCENESETUP
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.Common.Editor
{
    public class SceneSetupModularEditor
    {
        private const string ScriptingDefineSymbol = "SCENESETUP";
#if SCENESETUP
        [MenuItem("Tools/Scene Management/Create Scene Setup Modular", false, 1)]
        private static void CreateSceneSetupModular()
        {
            var gos = new GameObject()
            {
                name = "! SceneSetup"
            };

            var sceneSetup = gos.AddComponent<SceneSetupModular>();
            sceneSetup.SaveSceneSetup();
            
            Undo.RegisterCreatedObjectUndo(gos, "Created Scene Setup");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
#endif
    }
}
#endif