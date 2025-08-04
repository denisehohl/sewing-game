#if UIGRADIENT
using JoshH.UI;
using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - Gradient Intensity", menuName = "Transition Object/Gradient Intensity", order = 0)]
    public class TransitionObjectGradientIntensity : TransitionObject<UIGradient, float>
    {
        protected override string Name { get; } = "Transition - Gradient Intensity";
        
        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                Component.Intensity = Mathf.Lerp(Properties.MinValue, Properties.MaxValue, eval);

                return true;
            }

            return false;
        }
    }
}
#endif