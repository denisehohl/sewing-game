using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform AngleAxis Y", menuName = "Animation Object/Transform AngleAxis Y")]
    public class AnimationObjectTransformAngleAxisY : AnimationObjectCoroutineFromTo<Transform, float>
    {
        protected override string Name => $"Transform AngleAxis Y - { name }";
        public bool Additive = false;

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            if(Additive)
            {
                GetComponent(gos);

                var localEulerAngles = Component.localEulerAngles;
                Properties.ValueFrom = localEulerAngles.y + Properties.ValueFrom;
                Properties.ValueTo = localEulerAngles.y + Properties.ValueTo;
            }
            
            base.Execute(gos, resetStartValue, callback);
        }

        public override IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(Properties.Delay);

            float time = 0;
            var speed = 1f / Properties.Duration;
            var previous = Properties.ValueFrom;
            var wait = new WaitForFixedUpdate();

            while (time < 1f)
            {
                time += Time.deltaTime * speed;

                var value = DOVirtual.EasedValue(Properties.ValueFrom, Properties.ValueTo, time, Properties.Ease);
                var delta = value - previous;
                
                previous = value;
                Component.rotation *= Quaternion.AngleAxis(delta, Vector3.up);

                yield return wait;
            }

            OnComplete();
        }

        protected override void SetToEndValue()
        {
            if (Component == null) return;
            
            var localEulerAngles = Component.localEulerAngles;
            Component.localRotation = Quaternion.Euler(localEulerAngles.x, Properties.ValueTo, localEulerAngles.z);
        }

        protected override void SetToStartValue()
        {
            if (Component == null) return;
            
            var localEulerAngles = Component.localEulerAngles;
            Component.localRotation = Quaternion.Euler(localEulerAngles.x, Properties.ValueFrom, localEulerAngles.z);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
