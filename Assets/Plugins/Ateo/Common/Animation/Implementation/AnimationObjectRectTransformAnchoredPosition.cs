using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - RectTransform AnchoredPosition", menuName = "Animation Object/RectTransform AnchoredPosition")]
    public sealed class AnimationObjectRectTransformAnchoredPosition : AnimationObjectTween<RectTransform, Vector3>
    {
        protected override string Name => $"Move Animation - { name }";

        protected override void SetToStartValue()
        {
            Component.anchoredPosition = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.anchoredPosition = Properties.ValueTo;
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOAnchorPos(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
