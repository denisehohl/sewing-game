using Ateo.Common;
using Ateo.Extensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "New Animation RectTransform Margin Object", menuName = "Animation Object/RectTransform Margin")]
    public class AnimationObjectRectTransformMargin : AnimationObjectTween<RectTransform, Margin>
    {
        protected override string Name => $"RectTransform Margin Animation - {name}";

        protected override void SetToStartValue()
        {
            Component.SetLeftRightTopBottom(Properties.ValueFrom.ToVector4());
        }

        protected override void SetToEndValue()
        {
            Component.SetLeftRightTopBottom(Properties.ValueTo.ToVector4());
        }

        protected override void StartAnimation()
        {
            Tween = DOTween
                .To(
                    Component.GetLeftRightTopBottom,
                    Component.SetLeftRightTopBottom,
                    Properties.ValueTo.ToVector4(),
                    Properties.Duration)
                .SetDelay(Properties.Delay)
                .SetEase(Properties.Ease)
                .OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)