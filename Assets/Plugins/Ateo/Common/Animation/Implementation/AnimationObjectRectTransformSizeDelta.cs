using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - RectTransform SizeDelta", menuName = "Animation Object/RectTransform SizeDelta")]
    public sealed class AnimationObjectRectTransformSizeDelta : AnimationObjectTween<RectTransform, Vector2>
    {
        protected override string Name => $"RectTransform SizeDelta - { name }";

        protected override void SetToStartValue()
        {
            Component.sizeDelta = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.sizeDelta = Properties.ValueTo;
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOSizeDelta(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)