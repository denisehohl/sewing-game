using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - CanvasGroup", menuName = "Transition Object/CanvasGroup", order = 0)]
    public class TransitionObjectCanvasGroup : TransitionObject<CanvasGroup, float>
    {
        protected override string Name { get; } = "Transition - CanvasGroup";
        
        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Mathf.Lerp(Properties.MinValue, Properties.MaxValue, eval);
                Component.alpha = value;

                return true;
            }

            return false;
        }
    }
}