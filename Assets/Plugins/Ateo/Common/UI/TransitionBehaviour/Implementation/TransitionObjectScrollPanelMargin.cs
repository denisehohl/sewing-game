using Ateo.Common;
using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "Transition - ScrollPanel Margin", menuName = "Transition Object/ScrollPanel Margin", order = 0)]
    public class TransitionObjectScrollPanelMargin : TransitionObject<ScrollPanel, Margin>
    {
        protected override string Name { get; } = "Transition - ScrollPanel Margin";
        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Properties.MinValue.ToVector4() + (Properties.MaxValue.ToVector4() - Properties.MinValue.ToVector4()) * eval;
                Component.SetMargin(value);
                return true;
            }

            return false;
        }
    }
}