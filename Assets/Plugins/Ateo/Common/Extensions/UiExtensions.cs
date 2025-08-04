using UnityEngine;
using UnityEngine.UI;

namespace Ateo.Extensions
{
    public static class UiExtensions
    {
        // Source: https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui
        // Gets the RectTransform Snap Position
        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            var viewportLocalPosition = instance.viewport.localPosition;
            var childLocalPosition = child.localPosition;
            var result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y + (child.sizeDelta.y * 1.5f))
            );
            return result;
        }
    }
}