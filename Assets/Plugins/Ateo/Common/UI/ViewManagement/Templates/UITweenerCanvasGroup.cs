using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

namespace Ateo.ViewManagement
{
	[RequireComponent(typeof(CanvasGroup))]
    public class UITweenerCanvasGroup : UITweener
    {
        public TweenProperty m_Show;
        public TweenProperty m_Hide;
        private CanvasGroup _canvasGroup;
        private Tween _tween;
        
        private void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
	            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        public override void Show(TweenCallback callback = null)
        {
	        _tween?.Kill();
	        
            if(Application.isPlaying && _canvasGroup != null)
            {
	            _tween = _canvasGroup.DOFade(1f, m_Show.Duration).SetEase(m_Show.Ease).SetDelay(m_Show.Delay).OnComplete(callback);
            }
            else
            {
                callback?.Invoke();
            }
        }
        public override void Hide(TweenCallback callback = null)
        {
	        _tween?.Kill();
	        
            if(Application.isPlaying && _canvasGroup != null)
            {
	            _tween = _canvasGroup.DOFade(0f, m_Hide.Duration).SetEase(m_Hide.Ease).SetDelay(m_Hide.Delay).OnComplete(callback);
            }
            else
            {
                callback?.Invoke();
            }
        }
    }

    [System.Serializable]
    public class TweenProperty
    {
        public float Duration = 1f;
        public float Delay = 0f;
        public Ease Ease = Ease.InOutSine;
    }
}