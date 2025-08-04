using UnityEngine;
using System;
using Ateo.Extensions;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Ateo.UI
{
	[CreateAssetMenu(fileName = "TransitionObject - RectTransform AnchoredPosition", menuName = "Transition Object/RectTransform AnchoredPosition in Circle",
		order = 0)]
	public class TransitionObjectRectTransformAnchoredPositionCircle : TransitionObject
	{
		[Title("$Name"), FormerlySerializedAs("radius")]
		public float Radius;

		[FormerlySerializedAs("spacing")]
		public float Spacing = 1;

		[FormerlySerializedAs("minScale")]
		public float MinScale = 1;

		private RectTransform _rectTransform;
		private Canvas _canvas;

		protected override string Name { get; } = "Transition - RectTransform AnchoredPosition in Circle";

		public override bool Execute(float displacement, bool force = false)
		{
			if (!force && !(Math.Abs(Displacement - displacement) > 0.0001f))
			{
				return false;
			}

			Displacement = displacement;
			float offset = CalculateOffset(Displacement, out float scale);

			_rectTransform.localScale = Vector3.Lerp(MinScale * Vector3.one, Vector3.one, scale);
			_rectTransform.anchoredPosition = new Vector2(offset, 0);
			_canvas.sortingOrder = 2000 - Mathf.FloorToInt(Mathf.Abs(Displacement));

			return true;

			float CalculateOffset(float d, out float s)
			{
				float x = d * Spacing;

				// Limit to half circumference
				float quarterCircumference = Mathf.PI * Radius * 0.5f;
				x = Mathf.Clamp(x, -quarterCircumference, quarterCircumference);

				// Remap to -PI/2 to PI/2
				x /= quarterCircumference * Mathf.PI * 0.5f;

				// Calculate Position on circle (projected onto line)
				float position = Radius * Mathf.Sin(x);
				s = Mathf.Cos(x);

				// Calculate new displacement to match position
				return position - d;
			}
		}

		protected override void GetComponent(GameObject gos)
		{
			_rectTransform = gos.GetOrAddComponent<RectTransform>();
			_canvas = gos.transform.parent.GetOrAddComponent<Canvas>();
		}
	}
}