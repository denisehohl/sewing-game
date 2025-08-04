using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform LocalRotation", menuName = "Animation Object/Transform LocalRotation")]
    public sealed class AnimationObjectTransformLocalRotation : AnimationObjectTween<RectTransform, Vector3>
    {
        protected override string Name => $"Transform LocalRotation - { name }";

        protected override void SetToStartValue()
        {
            Component.localRotation = Quaternion.Euler(Properties.ValueFrom);
        }

        protected override void SetToEndValue()
        {
            Component.localRotation = Quaternion.Euler(Properties.ValueTo);
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOLocalRotateQuaternion(Quaternion.Euler(Properties.ValueTo), Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
