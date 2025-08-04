using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Image Color", menuName = "Animation Object/Image Color")]
    public sealed class AnimationObjectImageColor : AnimationObjectTween<MaskableGraphic, Color>
    {
        protected override string Name => $"Image Color - { name }";

        protected override void SetToStartValue()
        {
            Component.color = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.color = Properties.ValueTo;
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOColor(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
