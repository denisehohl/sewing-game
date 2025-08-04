using System;
using UnityEngine;

namespace Ateo.ViewManagement
{
    [RequireComponent(typeof(CanvasGroup)), ExecuteInEditMode]
    public abstract class UIGroup : MonoBehaviour
    {
        [NonSerialized]
        protected CanvasGroup CanvasGroup;

        [NonSerialized]
        protected UIBehaviour Behaviour;

        [NonSerialized]
        protected UITweener Tweener;

        public bool IsVisible { get; private set; }

        private bool _isShowing;
        private bool _isHiding;

        protected virtual void OnEnable()
        {
            if (CanvasGroup == null)
            {
                CanvasGroup = GetComponent<CanvasGroup>();

                if (CanvasGroup == null)
                {
	                CanvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            IsVisible = CanvasGroup.alpha != 0f;

            if (Application.isPlaying)
            {
                SetCanvasGroupValues(IsVisible);

                Behaviour = GetComponent<UIBehaviour>();
                Tweener = GetComponent<UITweener>();
            }
        }

        public virtual void Show(bool animate = false, bool callBehaviour = false)
        {
            if (!IsVisible || _isHiding)
            {
	            _isShowing = true;
	            _isHiding = false;
	            
                if (callBehaviour && Behaviour != null)
                {
	                Behaviour.OnShow();
                }

                if (animate && Tweener != null)
                {
	                Tweener.Show(EnableCanvasGroup);
                }
                else
                {
	                EnableCanvasGroup();
                }
            }
        }

        public virtual void Hide(bool animate = false, bool callBehaviour = false)
        {
            if (IsVisible || _isShowing)
            {
	            _isHiding = true;
	            _isShowing = false;
	            
                if (callBehaviour && Behaviour != null)
                {
	                Behaviour.OnHide();
                }

                if (animate && Tweener != null)
                {
	                Tweener.Hide(DisableCanvasGroup);
                }
                else
                {
	                DisableCanvasGroup();
                }
            }
        }

        protected virtual void EnableCanvasGroup()
        {
	        _isShowing = false;
            SetCanvasGroupValues(true);
        }

        protected virtual void DisableCanvasGroup()
        {
	        _isHiding = false;
            SetCanvasGroupValues(false);
        }

        protected virtual void SetCanvasGroupValues(bool enable)
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = enable ? 1f : 0f;
                CanvasGroup.blocksRaycasts = enable;
                CanvasGroup.interactable = enable;
            }

            IsVisible = enable;
        }
    }
}