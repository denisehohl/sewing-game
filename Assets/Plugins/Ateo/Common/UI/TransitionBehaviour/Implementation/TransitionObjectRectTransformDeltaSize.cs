using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - RectTransform DeltaSize", menuName = "Transition Object/RectTransform DeltaSize", order = 0)]
    public class TransitionObjectRectTransformDeltaSize : TransitionObject<RectTransform, Vector2>
    {
        protected override string Name { get; } = "Transition - RectTransform DeltaSize";

        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Properties.MinValue + (Properties.MaxValue - Properties.MinValue) * eval;
                Component.sizeDelta = value;

                return true;
            }

            return false;
        }
    }
}