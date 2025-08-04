using UnityEngine;

namespace Ateo.ViewManagement
{
    [RequireComponent(typeof(CanvasGroup)), ExecuteInEditMode]
    public class UIView : UIGroup
    {
        public string[] Elements = { };
    }
}