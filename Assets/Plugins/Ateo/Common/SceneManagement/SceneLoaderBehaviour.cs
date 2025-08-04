using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace Ateo.Common
{
    public class SceneLoaderBehaviour : MonoBehaviour
    {
        public bool LoadOnStart = false;
        public float Delay = 0f;

        [InlineProperty, HideLabel]
        public List<SceneLoaderData> SceneData;

        public bool ReadyToQuit { get; set; } = false;

        private void Start()
        {
            if (LoadOnStart)
            {
                LoadScenes(Delay);
            }
        }

        public void LoadScenes()
        {
            LoadScenes(Delay);
        }

        public void LoadScenes(float delay)
        {
            if (!AreAllScenesLoaded())
            {
                SceneData.Sort(new SceneLoadComparer());
                StartCoroutine(LoadScenesAsync(delay));
            }
            else
            {
                DebugDev.Log("SceneLoaderBehaviour: All scenes are loaded");
                ReadyToQuit = false;
            }
        }

        public void UnloadScenes()
        {
            if (!AreAllScenesUnLoaded())
            {
                SceneData.Sort(new SceneUnloadComparer());
                StartCoroutine(UnloadScenesAsync());
            }
            else
            {
                DebugDev.Log("SceneLoaderBehaviour: All scenes are unloaded");
                ReadyToQuit = true;
            }
        }
        
        private IEnumerator LoadScenesAsync(float delay)
        {
            yield return new WaitForSeconds(delay);

            DebugDev.Log("SceneLoaderBehaviour: Loading scenes started");

            foreach (var scene in SceneData)
            {
                yield return SceneLoader.LoadSceneAsync(
                    scene.Scene, LoadSceneMode.Additive, scene.ActiveScene, scene.WaitForSeconds);
            }

            DebugDev.Log("SceneLoaderBehaviour: Loading scenes completed");
        }

        private IEnumerator UnloadScenesAsync()
        {
            DebugDev.Log("SceneLoaderBehaviour: Unloading scenes started");

            for (var i = SceneData.Count - 1; i >= 0; i--)
            {
                yield return SceneLoader.UnloadSceneAsync(SceneData[i].Scene);
            }

            ReadyToQuit = true;
            DebugDev.Log("SceneLoaderBehaviour: Unloading scenes completed");
        }

        
        private bool AreAllScenesLoaded()
        {
            foreach (var scene in SceneData)
            {
                if (!SceneLoader.IsSceneLoaded(scene.Scene))
                    return false;
            }

            return true;
        }

        private bool AreAllScenesUnLoaded()
        {
            foreach (var scene in SceneData)
            {
                if (SceneLoader.IsSceneLoaded(scene.Scene))
                    return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public class SceneLoaderData
    {
        public float WaitForSeconds = 0f;
        public int LoadIndex;
        public int UnloadIndex;
        public bool ActiveScene = false;

        [Space]
        public SceneReference Scene;
    }

    public class SceneLoadComparer : IComparer<SceneLoaderData>
    {
        public int Compare(SceneLoaderData x, SceneLoaderData y)
        {
            if (y != null && x != null && x.LoadIndex > y.LoadIndex)
                return 1;
            if (y != null && x != null && x.LoadIndex < y.LoadIndex)
                return -1;
            return 0;
        }
    }

    public class SceneUnloadComparer : IComparer<SceneLoaderData>
    {
        public int Compare(SceneLoaderData x, SceneLoaderData y)
        {
            if (y != null && x != null && x.UnloadIndex > y.UnloadIndex)
                return 1;
            if (y != null && x != null && x.UnloadIndex < y.UnloadIndex)
                return -1;
            return 0;
        }
    }
}