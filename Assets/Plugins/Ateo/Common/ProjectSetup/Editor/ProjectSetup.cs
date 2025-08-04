using System.IO;
using UnityEditor;
using UnityEngine;

public class ProjectSetup : MonoBehaviour
{
    private static readonly string[] Folders =
    {
        "01_Scenes",
        "02_Prefabs",
        "03_Scripts",
        "03_Scripts/Editor",
        "03_Scripts/01_Shaders",
        "04_Assets",
        "04_Assets/01_Models",
        "04_Assets/02_Textures",
        "04_Assets/03_Materials",
        "04_Assets/04_AssetStore",
        "04_Assets/04_AssetStore/01_Props",
        "04_Assets/04_AssetStore/02_EditorTools",
        "04_Assets/04_AssetStore/03_GUI",
        "04_Assets/05_VFX",
        "04_Assets/06_Video",
        "05_UserInterface",
        "05_UserInterface/01_Textures",
        "05_UserInterface/02_Fonts",
        "05_UserInterface/03_Icons",
        "05_UserInterface/04_Prefabs",
        "05_UserInterface/05_Animation",
        "05_UserInterface/06_Materials",
        "06_Audio",
        "06_Audio/01_FX",
        "06_Audio/02_Ambient",
        "06_Audio/03_Music",
        "06_Audio/04_Mixers",
        "07_Animation",
        "08_Data",
        "08_Data/01_Presets",
        "08_Data/02_Settings",
        "Plugins"
    };

    [MenuItem("Window/Project Setup/Create Folder Structure")]
    static void CreateFolderStructure()
    {
        var path = Application.dataPath;

        foreach (var folder in Folders)
        {
            var folderPath = Path.Combine(path, folder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Folder structure created");
    }
    
    [MenuItem("Window/Project Setup/Import Odin Inspector")]
    static void ImportOdinInspector()
    {
        ImportAssetStoreAsset("Sirenix/Editor ExtensionsSystem/Odin - Inspector and Serializer.unitypackage");
    }
    
    [MenuItem("Window/Project Setup/Import Plastic SCM Plugin")]
    static void ImportPlasticUnityPlugin()
    {
        ImportAssetStoreAsset("Unity Technologies/Asset Store Tools/Plastic SCM Plugin for Unity beta.unitypackage");
    }
    
    private static void ImportAssetStoreAsset(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            var userFolderPath = System.Environment.GetEnvironmentVariable("USERPROFILE");
            var assetPath = Path.Combine(userFolderPath, "AppData/Roaming/Unity/Asset Store-5.x/", path);
            AssetDatabase.ImportPackage(assetPath, true);
        }
        else
        {
            var assetPath = Path.Combine("~/Library/Unity/Asset Store-5.x/", path);
            AssetDatabase.ImportPackage(assetPath, true);
        }
    }
}
