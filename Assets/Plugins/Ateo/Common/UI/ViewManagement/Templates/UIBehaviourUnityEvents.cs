using UnityEngine;
using UnityEngine.Events;
using Ateo.ViewManagement;

public class UIBehaviourUnityEvents : UIBehaviour
{
    public UnityEvent m_OnHide;
    public UnityEvent m_OnShow;
    
    public override void OnHide()
    {
        m_OnHide?.Invoke();
    }

    public override void OnShow()
    {
        m_OnShow?.Invoke();
    }
}
