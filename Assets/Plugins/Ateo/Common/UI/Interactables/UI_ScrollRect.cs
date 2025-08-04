using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI_ScrollRect")]
    public class UI_ScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void Handler();

        public event Handler OnEnter;
        public event Handler OnExit;
        public event Handler OnScrollStart;
        public event Handler OnScrollEnd;

        public bool IsMouseHovering { get; private set; }
        public bool IsScrolling { get; private set; }

        private void Update()
        {
            if (!IsScrolling) return;
            if (velocity.magnitude > 0.0001f) return;

            IsScrolling = false;
            OnScrollEnd?.Invoke();
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (!IsScrolling)
            {
                IsScrolling = true;
                OnScrollStart?.Invoke();
            }

            var x = eventData.scrollDelta.x;
            var y = eventData.scrollDelta.y;

#if UNITY_WEBGL && !UNITY_EDITOR
            x *= 0.25f;
            y *= 0.25f;
#endif

            eventData.scrollDelta = new Vector2(x, y);
            base.OnScroll(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseHovering = true;
            OnEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseHovering = false;
            OnExit?.Invoke();
        }
    }
}