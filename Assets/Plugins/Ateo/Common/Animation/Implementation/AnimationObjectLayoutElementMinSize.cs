using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - LayoutElement MinSize", menuName = "Animation Object/LayoutElement MinSize")]
    public class AnimationObjectLayoutElementMinSize : AnimationObjectTween<LayoutElement, Vector2>
    {
        protected override string Name => $"LayoutElement MinSize - { name }";

        protected override void SetToStartValue()
        {
            Component.minWidth = Properties.ValueFrom.x;
            Component.minHeight = Properties.ValueFrom.y;
        }

        protected override void SetToEndValue()
        {
            Component.minWidth = Properties.ValueTo.x;
            Component.minHeight = Properties.ValueTo.y;
        }

        protected override void StartAnimation()
        {
            Tween = Component
                .DOMinSize(Properties.ValueTo, Properties.Duration)
                .SetDelay(Properties.Delay)
                .SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)