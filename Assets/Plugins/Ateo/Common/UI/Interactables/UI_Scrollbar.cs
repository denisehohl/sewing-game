using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.UI
{
    public class UI_Scrollbar : Scrollbar, IEndDragHandler
    {
        public delegate void Handler();

        public event Handler OnStartDrag;
        public event Handler OnCompleteDrag;
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            OnStartDrag?.Invoke();
            base.OnBeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnCompleteDrag?.Invoke();
        }
    }
}