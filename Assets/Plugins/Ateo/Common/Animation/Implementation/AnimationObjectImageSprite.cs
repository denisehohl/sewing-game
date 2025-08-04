using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.Animation
{
    [CreateAssetMenu(fileName = "Animation - Image Sprite", menuName = "Animation Object/Image Sprite")]
    public sealed class AnimationObjectImageSprite : AnimationObjectCoroutineFromTo<Image, Sprite>
    {
        protected override string Name => $"Image Sprite - { name }";
        
        protected override void SetToStartValue()
        {
            Component.sprite = Properties.ValueFrom;
        }

        protected override void SetToEndValue()
        {
            Component.sprite = Properties.ValueTo;
        }
        
        public override IEnumerator Coroutine()
        {
            float time = 0f;

            while (time < Properties.Duration)
            {
                time += Time.deltaTime;
                yield return null;
            }
            
            SetToEndValue();
        }
    }
}
// © 2021 Ateo GmbH (https://www.ateo.ch)