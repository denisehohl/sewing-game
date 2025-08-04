using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Transform AngleAxis X", menuName = "Animation Object/Transform AngleAxis X")]
    public class AnimationObjectTransformAngleAxisX : AnimationObjectCoroutineFromTo<Transform, float>
    {
        protected override string Name => $"Transform AngleAxis X - { name }";
        public bool Additive = false;

        public override void Execute(GameObject gos, bool resetStartValue = true, AnimationObjectHandler callback = null)
        {
            if (Additive)
            {
                GetComponent(gos);

                var localEulerAngles = Component.localEulerAngles;
                Properties.ValueFrom = localEulerAngles.x + Properties.ValueFrom;
                Properties.ValueTo = localEulerAngles.x + Properties.ValueTo;
            }

            base.Execute(gos, resetStartValue, callback);
        }

        public override IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(Properties.Delay);

            var time = 0f;
            var speed = 1f / Properties.Duration;
            var previous = Properties.ValueFrom;
            var wait = new WaitForFixedUpdate();

            while (time < 1f)
            {
                time += Time.deltaTime * speed;

                var value = DOVirtual.EasedValue(Properties.ValueFrom, Properties.ValueTo, time, Properties.Ease);
                var delta = value - previous;
                
                previous = value;
                Component.rotation *= Quaternion.AngleAxis(delta, Vector3.right);

                yield return wait;
            }

            OnComplete();
        }

        protected override void SetToEndValue()
        {
            if (Component == null) return;
            
            var localEulerAngles = Component.localEulerAngles;
            Component.localRotation = Quaternion.Euler(Properties.ValueTo, localEulerAngles.y, localEulerAngles.z);
        }

        protected override void SetToStartValue()
        {
            if (Component == null) return;
            
            var localEulerAngles = Component.localEulerAngles;
            Component.localRotation = Quaternion.Euler(Properties.ValueFrom, localEulerAngles.y, localEulerAngles.z);
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)
