using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - CanvasGroup Alpha", menuName = "Animation Object/CanvasGroup Alpha")]
    public sealed class AnimationObjectCanvasGroupAlpha : AnimationObjectTween<CanvasGroup, float>
    {
        protected override string Name => $"CanvasGroup Alpha - { name }";

        protected override void SetToStartValue()
        {
            Component.alpha = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.alpha = Properties.ValueTo;
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOFade(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
