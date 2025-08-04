#if TMPRO
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Ateo.Animation
{
	[CreateAssetMenu(fileName = "Animation - TextMesh Pro Color", menuName = "Animation Object/TextMesh Pro Color")]
	public sealed class AnimationObjectTextMeshProColor : AnimationObjectTween<TextMeshProUGUI, Color>
	{
		protected override string Name => $"TextMesh Pro Color - { name }";

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
// © 2022 Ateo GmbH (https://www.ateo.ch)
#endif