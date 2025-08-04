using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Common
{
    [System.Serializable]
    public struct Margin
    {
        [HorizontalGroup("horizontal", PaddingRight = 0.1f), VerticalGroup("horizontal/top"), LabelWidth(50)]
        public float Left;

        [VerticalGroup("horizontal/bottom"), LabelWidth(50)]
        public float Right;

        [VerticalGroup("horizontal/top"), LabelWidth(50)]
        public float Top;

        [VerticalGroup("horizontal/bottom"), LabelWidth(50)]
        public float Bottom;

        public Margin(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public Margin(Vector4 value)
        {
            Left = value.x;
            Right = value.y;
            Top = value.z;
            Bottom = value.w;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(Left, Right, Top, Bottom);
        }
    }
}