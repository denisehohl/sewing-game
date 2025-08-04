using System.Collections.Generic;
using Ateo.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Ateo.UI
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ScrollPanel : MonoBehaviour
    {
        [Title("Panel"), FoldoutGroup("Properties")]
        public Vector2 Size;

        [FoldoutGroup("Properties")]
        public bool Overrideable = true;
        
        [FoldoutGroup("Properties/Margin"), InlineProperty]
        public Margin Margin;
        
        [Title("Panel"), FoldoutGroup("References"),Required]
        public TransitionBehaviour Transition;
        
        [FoldoutGroup("References")]
        public List<Behaviour> ComponentsToBeOcclusionCulled = new List<Behaviour>();

        [FoldoutGroup("Events")]
        public UnityEvent OnSelected;
        
        [FoldoutGroup("Events")]
        public UnityEvent OnDeselected;
        
        protected ScrollSnap ScrollSnap;
        
        public bool IsInitialized { get; protected set; }
        public bool IsSelected { get; protected set; }
        public bool AreComponentsEnabled { get; protected set; } = true;

        public virtual void Initialize(ScrollSnap scrollSnap)
        {
            if (!IsInitialized)
            {
                ScrollSnap = scrollSnap;
                Transition.Initialize();
                IsInitialized = true;
            }
        }

        public virtual void EnableComponents(bool enable)
        {
            foreach (var component in ComponentsToBeOcclusionCulled)
            {
                component.enabled = enable;
            }

            AreComponentsEnabled = enable;
        }

        public virtual void SelectPanel(bool isSelected, bool immediate = false, bool forceUpdate = false)
        {
            if (IsSelected != isSelected || forceUpdate)
            {
                IsSelected = isSelected;

                if (IsSelected)
                {
                    OnPanelSelected(immediate);
                }
                else
                {
                    OnPanelDeselected(immediate);
                }
            }
        }

        public void SetMargin(Vector4 value, bool updateScrollSnap = true)
        {
            SetMargin(new Margin(value), updateScrollSnap);
        }

        public void SetMargin(Margin margin, bool updateScrollSnap = true)
        {
            Margin = margin;

            if (updateScrollSnap && ScrollSnap != null)
            {
                ScrollSnap.UpdateLayout();
            }
        }
        
        public void SetSize(Vector2 size, bool updateScrollSnap = true)
        {
            Size = size;

            if (updateScrollSnap && ScrollSnap != null)
            {
                ScrollSnap.UpdateLayout();
            }
        }

        public virtual void SetTransitionProgress(float progress)
        {
            Transition.Execute(progress);
        }

        protected virtual void OnPanelSelected(bool immediate = false)
        {
            OnSelected.Invoke();
        }
        
        protected virtual void OnPanelDeselected(bool immediate = false)
        {
            OnDeselected.Invoke();
        }
    }
}