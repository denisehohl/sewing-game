using UnityEngine;
using UnityEngine.UI;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - Image Color", menuName = "Transition Object/Image Color", order = 0)]
    public class TransitionObjectImageColor : TransitionObject<Image, Color>
    {
        protected override string Name { get; } = "Transition - Image Color";
        
        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Color.Lerp(Properties.MinValue, Properties.MaxValue, eval);
                Component.color = value;

                return true;
            }

            return false;
        }
    }
}