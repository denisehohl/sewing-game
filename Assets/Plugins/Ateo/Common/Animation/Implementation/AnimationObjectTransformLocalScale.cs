using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform LocalScale", menuName = "Animation Object/Transform LocalScale")]
    public sealed class AnimationObjectTransformLocalScale : AnimationObjectTween<RectTransform, Vector3>
    {
        protected override string Name => $"Transform LocalScale - { name }";

        protected override void SetToStartValue()
        {
            Component.localScale = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.localScale = Properties.ValueTo;
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOScale(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
