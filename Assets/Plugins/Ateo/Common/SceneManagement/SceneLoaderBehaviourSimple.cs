using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.Common
{
    public class SceneLoaderBehaviourSimple : MonoBehaviour
    {
        public enum LoadingMode
        {
            Manual,
            Awake,
            Start
        }
        
        [Serializable]
        public class Data
        {
            public float Delay = 0f;
            public bool MakeActiveScene = true;
            public LoadSceneMode LoadSceneMode;

            [Space, Required]
            public SceneReference Scene;
        }
        
        public LoadingMode Mode;

        [InlineProperty, HideLabel, Required]
        public Data Scene;

        private void Awake()
        {
            if (Mode == LoadingMode.Awake)
            {
                LoadScene();
            }
        }

        private void Start()
        {
            if (Mode == LoadingMode.Start)
            {
                LoadScene();
            }
        }
        
        [Button]
        public void LoadScene()
        {
            SceneLoader.LoadScene(Scene.Scene, Scene.LoadSceneMode, Scene.MakeActiveScene, Scene.Delay);
        }
    }
}