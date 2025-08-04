using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - LayoutElement PreferredSize", menuName = "Animation Object/LayoutElement PreferredSize")]
    public class AnimationObjectLayoutElementPreferredSize : AnimationObjectTween<LayoutElement, Vector2>
    {
        protected override string Name => $"LayoutElement PreferredSize - { name }";

        protected override void SetToStartValue()
        {
            Component.preferredWidth = Properties.ValueFrom.x;
            Component.preferredHeight = Properties.ValueFrom.y;
        }

        protected override void SetToEndValue()
        {
            Component.preferredWidth = Properties.ValueTo.x;
            Component.preferredHeight = Properties.ValueTo.y;
        }

        protected override void StartAnimation()
        {
            Tween = Component
                .DOPreferredSize(Properties.ValueTo, Properties.Duration)
                .SetDelay(Properties.Delay)
                .SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)