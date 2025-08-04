using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - LayoutElement Prefered Height Relative",
        menuName = "Transition Object/LayoutElement Prefered Height Relative", order = 0)]
    public class TransitionObjectLayoutElementPreferedHeightRelative : TransitionObjectLayoutElementPreferedHeight
    {
        protected override string Name { get; } = "Transition - LayoutElement Prefered Height Relative";
        
        protected override void GetComponent(GameObject gos)
        {
            base.GetComponent(gos);
            Properties.MinValue += Component.preferredHeight;
            Properties.MaxValue += Component.preferredHeight;
        }

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