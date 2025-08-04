using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform LocalPosition Y", menuName = "Animation Object/Transform LocalPosition Y")]
    public class AnimationObjectTransformLocalPositionY : AnimationObjectTween<Transform, float>
    {
        protected override string Name => $"Transform LocalPosition Y - { name }";

        public bool Additive = false;

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            if (Additive)
            {
                GetComponent(gos);

                var localPosition = Component.transform.localPosition;
                Properties.ValueFrom = localPosition.y + PropertiesBackup.ValueFrom;
                Properties.ValueTo = localPosition.y + PropertiesBackup.ValueTo;
            }

            base.Execute(gos, resetStartValue, callback);
        }

        protected override void SetToStartValue()
        {
            Component.localPosition = new Vector3(Component.transform.localPosition.x, Properties.ValueFrom, Component.localPosition.z);
        }
        
        protected override void SetToEndValue()
        {
            Component.localPosition = new Vector3(Component.transform.localPosition.x, Properties.ValueTo, Component.localPosition.z);
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOLocalMoveY(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)