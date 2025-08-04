using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UI_ClickParent : MonoBehaviour, IPointerClickHandler
{
    private Button m_ButtonParent;

    private void OnEnable()
    {
        m_ButtonParent = transform.parent.GetComponent<Button>();
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        DebugDev.Log("Click");
        
        m_ButtonParent.OnPointerClick(pointerEventData);
    }
}