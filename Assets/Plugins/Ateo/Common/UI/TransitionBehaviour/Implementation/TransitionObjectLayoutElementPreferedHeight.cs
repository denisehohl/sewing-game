using UnityEngine;
using UnityEngine.UI;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - LayoutElement Prefered Height", menuName = "Transition Object/LayoutElement Prefered Height", order = 0)]
    public class TransitionObjectLayoutElementPreferedHeight : TransitionObject<LayoutElement, float>
    {
        protected override string Name { get; } = "Transition - LayoutElement Prefered Height";

        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                // var value = Properties.MinValue + (Properties.MaxValue - Properties.MinValue) * eval;
                var value = Mathf.Lerp(Properties.MinValue, Properties.MaxValue, eval);
                Component.preferredHeight = value;

                return true;
            }

            return false;
        }
    }
}