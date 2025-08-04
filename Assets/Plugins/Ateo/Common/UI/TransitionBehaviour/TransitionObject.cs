using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Ateo.UI
{
    [System.Serializable]
    public class TransitionProperties<U> where U: struct
    {
        [HideLabel, HideReferenceObjectPicker, NonSerialized, OdinSerialize]
        public AnimationCurve Function = new AnimationCurve();
        
        [NonSerialized, OdinSerialize]
        public U MinValue;
        
        [NonSerialized, OdinSerialize]
        public U MaxValue;

        public TransitionProperties() { }
        public TransitionProperties(TransitionProperties<U> source)
        {
            Function = source.Function;
            MinValue = source.MinValue;
            MaxValue = source.MaxValue;
        }
    }

    public abstract class TransitionObject : SerializedScriptableObject
    {
        protected abstract string Name { get; }
        protected float Displacement { get; set; } = -1f;

        public virtual void Initialize(GameObject gos)
        {
            GetComponent(gos);
        }

        public virtual bool Execute(float displacement, bool force = false)
        {
            if (force || Math.Abs(Displacement - displacement) > 0.0001f)
            {
                Displacement = displacement;
                return true;
            }

            return false;
        }

        protected abstract void GetComponent(GameObject gos);
    }

    // [CreateAssetMenu(fileName = "TransitionObject", menuName = "Ateo/Scroll Snap/Transition Object", order = 0)]
    public abstract class TransitionObject<T, U> : TransitionObject where T : Component where U: struct
    {
        [Title("$Name"), HideLabel, HideReferenceObjectPicker, NonSerialized, OdinSerialize]
        public readonly TransitionProperties<U> Properties = new TransitionProperties<U>();
        
        protected T Component { get; set; }
        
        protected override void GetComponent(GameObject gos)
        {
            Component = gos.TryGetComponent<T>(out var component) ? component : gos.AddComponent<T>();
        }
    }
}