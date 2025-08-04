#if TMPRO
using System.Collections;
using TMPro;
using UnityEngine;

namespace Ateo.Animation
{
	[CreateAssetMenu(fileName = "Animation - TextMesh Pro Font Style", menuName = "Animation Object/TextMesh Pro Font Style")]
	public sealed class AnimationObjectTextMeshProFontStyle : AnimationObjectCoroutineFromTo<TextMeshProUGUI, FontStyles>
	{
		protected override string Name => $"TextMesh Pro Font Style - { name }";
        
		protected override void SetToStartValue()
		{
			Component.fontStyle = Properties.ValueFrom;
		}

		protected override void SetToEndValue()
		{
			Component.fontStyle = Properties.ValueTo;
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
// © 2022 Ateo GmbH (https://www.ateo.ch)
#endif