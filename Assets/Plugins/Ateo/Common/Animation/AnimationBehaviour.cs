using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Ateo.Animation
{
    /// <summary>
    /// Data class that contains an array of AnimationObject references and
    /// a reference to a GameObject.
    /// </summary>
    [System.Serializable]
    public class AnimationBehaviourData
    {
        /// <summary>
        /// The GameObject to be animated.
        /// It needs to hold the component that will be animated.
        /// </summary>
        public GameObject AnimationTarget;

        /// <summary>
        /// An array of AnimationObjects that define the animation.
        /// An AnimationObject is a ScriptableObject that contains parameters for an animation
        /// and the logic to execute that animation.
        /// The values of an AnimationObject will not be saved during play mode,
        /// since we operate on an instance of the AnimationObject.
        /// </summary>
        /// <value></value>
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden), Space]
        public AnimationObject[] AnimationObjects = { };

        public AnimationObject[] AnimationObjectsRuntime { get; set; }

        /// <summary>
        /// True if the instances of the AnimationObject array have been instantiated and initialized.
        /// </summary>
        /// <value></value>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Instantiates new AnimationObjects.
        /// </summary>
        /// <param name="gos"></param>
        public void Initialize(GameObject gos)
        {
            if (Initialized)
            {
                DebugDev.LogWarning("AnimationBehaviourData.Initialize: Already Initialized");
                return;
            }

            if (AnimationTarget == null)
                AnimationTarget = gos;

            AnimationObjectsRuntime = new AnimationObject[AnimationObjects.Length];

            // Instantiate each AnimationObject and save it back into the array.
            for (int i = 0; i < AnimationObjectsRuntime.Length; i++)
            {
                if(AnimationObjects[i] != null)
                    AnimationObjectsRuntime[i] = ScriptableObject.Instantiate(AnimationObjects[i]);
                else
                {
                    DebugDev.LogWarning($"AnimationBehaviour: Animation Object is null");
                }
            }

            Initialized = true;
        }
    }

    public class AnimationBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Automatically executes the animation OnEnable()
        /// </summary>
        public bool PlayOnEnable = false;

        [InlineProperty, Space] public AnimationBehaviourData[] Data = { };
        
        [FoldoutGroup("Events")]
        public UnityEvent OnAnimationStart;
        
        [FoldoutGroup("Events")]
        public UnityEvent OnAnimationComplete;
        
        [FoldoutGroup("Events")]
        public UnityEvent OnAnimationReset;
        
        public delegate void AnimationCallbackHandler();

        private AnimationCallbackHandler _callback;

        /// <summary>
        /// True if all the AnimationBehaviour has been initialized.
        /// Don't call any methods on this behaviour if this is set to false.
        /// </summary>
        /// <value></value>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Returns true if the animation is currently running.
        /// </summary>
        /// <value></value>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Return true if the animatin has been completed.
        /// </summary>
        /// <value></value>
        public bool IsCompleted { get; protected set; }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if (PlayOnEnable && IsInitialized)
            {
                Execute();
            }
        }

        protected virtual void OnDisable()
        {
            if (!IsInitialized) return;

            foreach (var data in Data)
            {
                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    if (animObj != null)
                        animObj.Abort();
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (!IsInitialized) return;

            foreach (var data in Data)
            {
                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    if (animObj != null)
                        Destroy(animObj);
                }
            }
        }

        [Button()]
        public virtual void Execute()
        {
            Execute(true, null);
        }

        public virtual void Execute(AnimationCallbackHandler callback)
        {
            Execute(true, callback);
        }

        public virtual void Execute(bool resetStartValue)
        {
            Execute(resetStartValue, null);
        }

        public virtual void Execute(bool resetStartValue, AnimationCallbackHandler callback)
        {
            if (!IsInitialized)
                Initialize();

            IsRunning = true;
            IsCompleted = false;
            _callback = callback;

            foreach (var data in Data)
            {
                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    if (data.Initialized)
                    {
                        animObj.Execute(data.AnimationTarget, resetStartValue, OnComplete);
                    }
                }
            }
            
            OnAnimationStart.Invoke();
        }

        public virtual void ExecuteAnimationImmediate()
        {
            if (!IsInitialized)
                Initialize();

            foreach (var data in Data)
            {
                if (!data.Initialized) continue;

                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    if (animObj != null)
                        animObj.SkipToEnd(data.AnimationTarget);
                }
            }
            
            IsCompleted = true;
            OnAnimationComplete.Invoke();
        }

        public virtual void Abort()
        {
            if (!IsInitialized)
                Initialize();

            IsRunning = false;

            foreach (var data in Data)
            {
                if (!data.Initialized) continue;

                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    animObj.Abort();
                }
            }
        }

        public virtual void ResetBehaviour()
        {
            if (!IsInitialized)
                Initialize();

            IsCompleted = false;
            IsRunning = false;

            _callback = null;

            foreach (var data in Data)
            {
                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    animObj.SkipToStart(data.AnimationTarget);
                }
            }
            
            OnAnimationReset?.Invoke();
        }

        protected virtual void Initialize()
        {
            if (IsInitialized) return;
            
            foreach (var data in Data)
            {
                data.Initialize(gameObject);
            }

            IsInitialized = true;
        }

        protected virtual void OnComplete(AnimationObject animationObject, AnimationState animationCompleteType)
        {
            if (animationCompleteType == AnimationState.Aborted)
            {
                return;
            }

            var shouldComplete = true;

            foreach (var data in Data)
            {
                foreach (var animObj in data.AnimationObjectsRuntime)
                {
                    if (animObj.State != AnimationState.AtEnd)
                        shouldComplete = false;
                }
            }

            if (!IsCompleted && shouldComplete)
            {
                IsRunning = false;
                IsCompleted = true;
                _callback?.Invoke();
                OnAnimationComplete.Invoke();
            }
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)