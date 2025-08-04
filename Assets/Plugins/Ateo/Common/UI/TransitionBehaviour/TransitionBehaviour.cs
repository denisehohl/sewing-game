using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.UI
{
    public sealed class TransitionBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Data class that contains an array of TransitionObject references and
        /// a reference to a GameObject.
        /// </summary>
        [Serializable]
        public class TransitionBehaviourData
        {
            /// <summary>
            /// The GameObject to be transitioned.
            /// It needs to hold the component that will be transitioned.
            /// </summary>
            [Required]
            public GameObject Target;

            /// <summary>
            /// An array of TransitionObjects that define the transition.
            /// An TransitionObject is a ScriptableObject that contains parameters for a transition
            /// and the logic to execute that transition.
            /// The values of an TransitionObject will not be saved during play mode,
            /// since we operate on an instance of the TransitionObject.
            /// </summary>
            /// <value></value>
            [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden), Space, Required]
            public TransitionObject[] TransitionObjects = { };

            public TransitionObject[] TransitionObjectsRuntime { get; set; }

            /// <summary>
            /// True if the instances of the TransitionObject array have been instantiated and initialized.
            /// </summary>
            /// <value></value>
            public bool Initialized { get; private set; }

            /// <summary>
            /// Instantiates new TransitionObjects.
            /// </summary>
            public void Initialize()
            {
                if (Initialized)
                {
                    Debug.LogWarning("TransitionBehaviourData.Initialize: Already Initialized");
                    return;
                }

                TransitionObjectsRuntime = new TransitionObject[TransitionObjects.Length];

                // Instantiate each TransitionObject and save it back into the array.
                for (var i = 0; i < TransitionObjects.Length; i++)
                {
                    if (TransitionObjects[i] != null)
                    {
                        TransitionObjectsRuntime[i] = Instantiate(TransitionObjects[i]);
                        TransitionObjectsRuntime[i].Initialize(Target);
                    }
                    else
                    {
                        Debug.LogWarning($"TransitionBehaviour: Animation Object is null");
                    }
                }

                Initialized = true;
            }
        }

        [InlineProperty, Space]
        public TransitionBehaviourData[] Data = { };

        /// <summary>
        /// True if all the TransitionBehaviour has been initialized.
        /// Don't call any methods on this behaviour if this is set to false.
        /// </summary>
        /// <value></value>
        public bool IsInitialized { get; private set; }
        
        public bool IsInitializing { get; private set; }

        /// <summary>
        /// Last displacement value that was passed to Execute() method
        /// </summary>
        public float Displacement { get; private set; } = 0f;

        /*private void Start()
        {
            StartCoroutine(Initialize());
        }*/

        private void OnDestroy()
        {
            if (!IsInitialized) return;

            foreach (var data in Data)
            {
                foreach (var obj in data.TransitionObjectsRuntime)
                {
                    if (obj != null)
                        Destroy(obj);
                }
            }
        }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitializing = true;

                foreach (var data in Data)
                {
                    data.Initialize();
                }

                IsInitialized = true;
            }
        }

        public void Execute(float displacement, bool force = false)
        {
            if (!IsInitialized)
            {
                if (!IsInitializing)
                {
                    Initialize();
                    DebugDev.LogWarning("TransitionBehaviour.Execute(): Not initialized");
                }
            }

            Displacement = displacement;

            foreach (var data in Data)
            {
                foreach (var obj in data.TransitionObjectsRuntime)
                {
                    if (data.Initialized)
                    {
                        obj.Execute(displacement, force);
                    }
                }
            }
        }
    }
}