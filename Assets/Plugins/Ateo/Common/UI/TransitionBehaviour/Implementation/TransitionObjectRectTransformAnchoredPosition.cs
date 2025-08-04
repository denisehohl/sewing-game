using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - RectTransform AnchoredPosition", menuName = "Transition Object/RectTransform AnchoredPosition", order = 0)]
    public class TransitionObjectRectTransformAnchoredPosition : TransitionObject<RectTransform, Vector2>
    {
        protected override string Name { get; } = "Transition - RectTransform AnchoredPosition";

        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Properties.MinValue + (Properties.MaxValue - Properties.MinValue) * eval;
                Component.anchoredPosition = value;

                return true;
            }

            return false;
        }
    }
}