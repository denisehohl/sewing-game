using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Boolean = Ateo.Common.Boolean;

namespace Ateo.UI
{
    [Serializable, ExecuteInEditMode]
    public class UI_Slider : Slider, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private SliderEvent m_OnValueStart = new SliderEvent();

        [SerializeField]
        private SliderEvent m_OnValueEnd = new SliderEvent();

        public bool IsInteracting => IsClicking || IsDragging;
        public Boolean IsClicking { get; } = new Boolean();
        public Boolean IsDragging { get; } = new Boolean();

        private bool _onValueStartInvoked;
        private bool _onValueEndInvoked;
        
        public SliderEvent onValueStart
        {
            get => m_OnValueStart;
            set => m_OnValueStart = value;
        }

        public SliderEvent onValueEnd
        {
            get => m_OnValueEnd;
            set => m_OnValueEnd = value;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            IsClicking.SetValueToTrue();

            if (!_onValueStartInvoked)
            {
                onValueStart?.Invoke(m_Value);
                _onValueStartInvoked = true;
            }

            _onValueEndInvoked = false;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            IsClicking.SetValueToFalse();
            IsDragging.SetValueToFalse();
            
            if (!_onValueEndInvoked)
            {
                m_OnValueEnd?.Invoke(m_Value);
                _onValueEndInvoked = true;
            }

            _onValueStartInvoked = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging.SetValueToTrue();
            
            if (!_onValueStartInvoked)
            {
                onValueStart?.Invoke(m_Value);
                _onValueStartInvoked = true;
            }
            
            _onValueEndInvoked = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsClicking.SetValueToFalse();
            IsDragging.SetValueToFalse();

            if (!_onValueEndInvoked)
            {
                m_OnValueEnd?.Invoke(m_Value);
                _onValueEndInvoked = true;
            }
            
            _onValueStartInvoked = false;
        }
    }
}