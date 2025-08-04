using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - LayoutElement Prefered Height To Absolute",
        menuName = "Transition Object/LayoutElement Prefered Height To Absolute", order = 0)]
    public class TransitionObjectLayoutElementPreferedHeightToAbsolute : TransitionObjectLayoutElementPreferedHeight
    {
        protected override string Name { get; } = "Transition - LayoutElement Prefered Height To Absolute";
        
        protected override void GetComponent(GameObject gos)
        {
            base.GetComponent(gos);
            Properties.MinValue = Component.preferredHeight;
        }

        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Mathf.Lerp(Properties.MinValue, Properties.MaxValue, eval);
                Component.preferredHeight = value;

                return true;
            }

            return false;
        }
    }
}