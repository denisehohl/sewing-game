using DG.Tweening;
using UnityEngine;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform LocalPosition Z", menuName = "Animation Object/Transform LocalPosition Z")]
    public class AnimationObjectTransformLocalPositionZ : AnimationObjectTween<Transform, float>
    {
        protected override string Name => $"Transform LocalPosition Z - { name }";

        public bool Additive = false;

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            if (Additive)
            {
                GetComponent(gos);

                var localPosition = Component.transform.localPosition;
                Properties.ValueFrom = localPosition.z + PropertiesBackup.ValueFrom;
                Properties.ValueTo = localPosition.z + PropertiesBackup.ValueTo;
            }

            base.Execute(gos, resetStartValue, callback);
        }

        protected override void SetToStartValue()
        {
            Component.localPosition = new Vector3(Component.transform.localPosition.x, Component.localPosition.y, Properties.ValueFrom);
        }
        
        protected override void SetToEndValue()
        {
            Component.localPosition = new Vector3(Component.transform.localPosition.x, Component.localPosition.y, Properties.ValueTo);
        }

        protected override void StartAnimation()
        {
            Tween = Component.DOLocalMoveZ(Properties.ValueTo, Properties.Duration).SetDelay(Properties.Delay).SetEase(Properties.Ease).OnComplete(OnComplete);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)