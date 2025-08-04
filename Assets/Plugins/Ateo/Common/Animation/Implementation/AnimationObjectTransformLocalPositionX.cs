using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform LocalPosition X", menuName = "Animation Object/Transform LocalPosition X")]
    public class AnimationObjectTransformLocalPositionX : AnimationObjectTween<Transform, float>
    {
        protected override string Name => $"Transform LocalPosition X - { name }";

        public bool Additive = false;

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            if (Additive)
            {
                GetComponent(gos);

                var localPosition = Component.transform.localPosition;
                Properties.ValueFrom = localPosition.x + PropertiesBackup.ValueFrom;
                Properties.ValueTo = localPosition.x + PropertiesBackup.ValueTo;
            }

            base.Execute(gos, resetStartValue, callback);
        }

        protected override void SetToStartValue()
        {
            Component.localPosition = new Vector3(Properties.ValueFrom, Component.transform.localPosition.y, Component.localPosition.z);
        }

        protected override void SetToEndValue()
        {
            Component.localPosition = new Vector3(Properties.ValueTo, Component.transform.localPosition.y, Component.localPosition.z);
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOLocalMoveX(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)