using UnityEngine;

namespace Ateo.Extensions
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Sets the height of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="height">Desired height.</param>
        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.y = height;
            rectTransform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// Sets the width of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="width">Desired width.</param>
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x = width;
            rectTransform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// Sets the left offset of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="left">Desired left offset.</param>
        public static void SetLeft(this RectTransform rectTransform, float left)
        {
            rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        }

        /// <summary>
        /// Sets the right offset of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="right">Desired right offset.</param>
        public static void SetRight(this RectTransform rectTransform, float right)
        {
            rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        }

        /// <summary>
        /// Sets the top offset of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="top">Desired top offset.</param>
        public static void SetTop(this RectTransform rectTransform, float top)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        }

        /// <summary>
        /// Sets the bottom offset of the RectTransform.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="bottom">Desired bottom offset.</param>
        public static void SetBottom(this RectTransform rectTransform, float bottom)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        }

        /// <summary>
        /// Gets the left, right, top, and bottom offsets of the RectTransform as a Vector4.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <returns>A Vector4 representing left, right, top, and bottom offsets.</returns>
        public static Vector4 GetLeftRightTopBottom(this RectTransform rectTransform)
        {
            Vector2 offsetMax = rectTransform.offsetMax;
            Vector2 offsetMin = rectTransform.offsetMin;
            return new Vector4(offsetMin.x, -offsetMax.x, -offsetMax.y, offsetMin.y);
        }

        /// <summary>
        /// Sets the left, right, top, and bottom offsets of the RectTransform from a Vector4.
        /// </summary>
        /// <param name="rectTransform">Target RectTransform.</param>
        /// <param name="value">Vector4 representing desired left, right, top, and bottom offsets.</param>
        public static void SetLeftRightTopBottom(this RectTransform rectTransform, Vector4 value)
        {
            rectTransform.SetLeft(value.x);
            rectTransform.SetRight(value.y);
            rectTransform.SetTop(value.z);
            rectTransform.SetBottom(value.w);
        }
    }
}
