using System;
using System.Collections;
using Ateo.Common;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Ateo.Animation
{
    public enum AnimationState
    {
        AtStart,
        Running,
        Aborted,
        AtEnd
    }

    /// <summary>
    /// A ScriptableObject that animates a property of a component.
    /// </summary>
    [Serializable]
    public abstract class AnimationObject : SerializedScriptableObject
    {
        public AnimationState State { get; protected set; }
        protected AnimationObjectHandler Callback { get; set; }
        protected abstract string Name { get; }

        public delegate void AnimationObjectHandler(AnimationObject animationObject, AnimationState animationCompleteType);

        public AnimationObject()
        {
        }

        /// <summary>
        /// Override this method when you implement a new class that inherits from AnimationObject.
        /// This method should start a Tween.
        /// </summary>
        protected abstract void StartAnimation();

        /// <summary>
        /// Kills the current animation.
        /// </summary>
        protected abstract void KillAnimation();

        /// <summary>
        /// Override this method when you implement a new class that inherits from AnimationObject.
        /// This method should set the animated property to its start value.
        /// </summary>
        protected abstract void SetToStartValue();

        /// <summary>
        /// Override this method when you implement a new class that inherits from AnimationObject.
        /// This method should set the animated property to its end value.
        /// </summary>
        protected abstract void SetToEndValue();

        /// <summary>
        /// Starts the animation
        /// </summary>
        /// <param name="gos">GameObject to apply the animation to</param>
        /// <param name="resetStartValue">Determines if the animated Component should be reset to its start value when the animation starts, or if it should start from the value it currently holds.</param>
        /// <param name="callback">Callback when the animation completes or aborts.</param>
        public abstract void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null);

        /// <summary>
        /// Applies the start value of the animation to the component. If an animation is currently running, it will get stopped.
        /// </summary>
        /// <param name="gos">GameObject to apply the animation to</param>
        public abstract void SkipToStart(GameObject gos);

        /// <summary>
        /// Applies the end value of the animation to the component. If an animation is currently running, it will get stopped.
        /// </summary>
        /// <param name="gos">GameObject to apply the animation to</param>
        public abstract void SkipToEnd(GameObject gos);

        /// <summary>
        /// Aborts the currently running animation and leaves the animated property at it's current value.
        /// </summary>
        public abstract void Abort();

        /// <summary>
        /// This callback gets executed when the Tween has finished.
        /// </summary>
        protected abstract void OnComplete();

        /// <summary>
        /// This method gets the component to animate
        /// </summary>
        /// <param name="gos">GameObject to to get the component from</param>
        protected abstract void GetComponent(GameObject gos);

        protected abstract void OnDestroy();
    }

    public abstract class AnimationObjectTween<T, U> : AnimationObject<T> where T : Component where U : struct
    {
        [Title("$Name"), HideReferenceObjectPicker, HideLabel, NonSerialized, OdinSerialize]
        public AnimationPropertiesFromTo<U> Properties = new AnimationPropertiesFromTo<U>();

        [NonSerialized]
        private AnimationPropertiesFromTo<U> _propertiesBackup;

        protected AnimationPropertiesFromTo<U> PropertiesBackup
        {
            get
            {
                if (_propertiesBackup == null)
                {
                    _propertiesBackup = new AnimationPropertiesFromTo<U>(Properties);
                }

                return _propertiesBackup;
            }
        }

        protected Tween Tween { get; set; }

        protected override void KillAnimation()
        {
            Tween?.Kill();
            Tween = null;
        }
    }

    public interface IAnimationObjectCoroutineable
    {
        IEnumerator Coroutine();
    }

    public abstract class AnimationObjectCoroutine<T, TU> : AnimationObject<T>, IAnimationObjectCoroutineable where T : Component
    {
        protected override void StartAnimation()
        {
            CoroutineRunner.Run(Coroutine());
        }

        protected override void KillAnimation()
        {
            CoroutineRunner.Stop(Coroutine());
        }

        public abstract IEnumerator Coroutine();
    }

    public abstract class AnimationObjectCoroutineTo<T, U> : AnimationObjectCoroutine<T, U> where T : Component
    {
        [Title("$Name"), HideReferenceObjectPicker, HideLabel, NonSerialized, OdinSerialize]
        public AnimationPropertiesTo<U> Properties = new AnimationPropertiesTo<U>();

        [NonSerialized]
        private AnimationPropertiesTo<U> _propertiesBackup;

        protected AnimationPropertiesTo<U> PropertiesBackup
        {
            get
            {
                if (_propertiesBackup == null)
                {
                    _propertiesBackup = new AnimationPropertiesTo<U>(Properties);
                }

                return _propertiesBackup;
            }
        }
    }

    public abstract class AnimationObjectCoroutineFromTo<T, U> : AnimationObjectCoroutine<T, U> where T : Component
    {
        [Title("$Name"), HideReferenceObjectPicker, HideLabel, NonSerialized, OdinSerialize]
        public AnimationPropertiesFromTo<U> Properties = new AnimationPropertiesFromTo<U>();

        [NonSerialized]
        private AnimationPropertiesFromTo<U> _propertiesBackup;

        protected AnimationPropertiesFromTo<U> PropertiesBackup
        {
            get
            {
                if (_propertiesBackup == null)
                {
                    _propertiesBackup = new AnimationPropertiesFromTo<U>(Properties);
                }

                return _propertiesBackup;
            }
        }
    }

    public abstract class AnimationObject<T> : AnimationObject where T : Component
    {
        protected T Component { get; set; }

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            State = AnimationState.AtStart;
            Callback = callback;

            GetComponent(gos);
            KillAnimation();

            if (resetStartValue)
                SetToStartValue();

            StartAnimation();
        }

        public override void SkipToStart(GameObject gos)
        {
            State = AnimationState.AtStart;
            GetComponent(gos);
            KillAnimation();

            SetToStartValue();
        }

        public override void SkipToEnd(GameObject gos)
        {
            State = AnimationState.AtEnd;
            GetComponent(gos);
            KillAnimation();

            SetToEndValue();
        }

        public override void Abort()
        {
            State = AnimationState.Aborted;
            KillAnimation();
            Callback?.Invoke(this, AnimationState.Aborted);
        }

        protected override void OnComplete()
        {
            if (State == AnimationState.AtEnd)
            {
                DebugDev.LogWarning($"AnimationObject<{GetType().Name}>: Is already completed.");
                return;
            }

            State = AnimationState.AtEnd;
            Callback?.Invoke(this, State);
        }

        protected override void GetComponent(GameObject gos)
        {
            if (gos != null)
                Component = gos.GetComponent<T>();

            if (Component == null)
            {
                DebugDev.LogWarning(
                    $"AnimationObject<{GetType().Name}>.GetComponent(): Component is null. Please add a Component of Type {typeof(T).Name} to {gos.name}");
            }
        }

        protected override void OnDestroy()
        {
            Abort();
        }
    }

    [Serializable]
    public class AnimationProperties
    {
        public Ease Ease;
        public float Duration;
        public float Delay;

        public AnimationProperties()
        {
        }
        
        public AnimationProperties(Ease ease, float duration, float delay)
        {
	        Ease = ease;
	        Duration = duration;
	        Delay = delay;
        }

        public AnimationProperties(AnimationProperties source)
        {
            Ease = source.Ease;
            Duration = source.Duration;
            Delay = source.Delay;
        }
    }

    [Serializable]
    public class AnimationPropertiesTo<T> : AnimationProperties
    {
        [Title("Value"), InlineProperty, HideLabel, HideReferenceObjectPicker, NonSerialized, OdinSerialize]
        public T Value;

        public AnimationPropertiesTo()
        {
        }

        public AnimationPropertiesTo(Ease ease, float duration, float delay, T value) : base(ease, duration, delay)
        {
	        Value = value;
        }
        
        public AnimationPropertiesTo(AnimationProperties source, T value) : base(source)
        {
	        Value = value;
        }

        public AnimationPropertiesTo(AnimationPropertiesTo<T> source) : base(source)
        {
	        Value = source.Value;
        }
    }

    [Serializable]
    public class AnimationPropertiesFromTo<T> : AnimationProperties
    {
        [Title("Values"), InlineProperty, HideReferenceObjectPicker, NonSerialized, OdinSerialize]
        public T ValueFrom;

        [InlineProperty, HideReferenceObjectPicker, NonSerialized, OdinSerialize, Space]
        public T ValueTo;

        public AnimationPropertiesFromTo()
        {
        }
        
        public AnimationPropertiesFromTo(Ease ease, float duration, float delay, T valueFrom, T valueTo) : base(ease, duration, delay)
        {
	        ValueFrom = valueFrom;
	        ValueTo = valueTo;
        }
        
        public AnimationPropertiesFromTo(AnimationProperties source, T valueFrom, T valueTo) : base(source)
        {
	        ValueFrom = valueFrom;
	        ValueTo = valueTo;
        }

        public AnimationPropertiesFromTo(AnimationPropertiesFromTo<T> source) : base(source)
        {
            ValueFrom = source.ValueFrom;
            ValueTo = source.ValueTo;
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)