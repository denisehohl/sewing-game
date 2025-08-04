using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - TMPro Color", menuName = "Transition Object/TMPro Color", order = 0)]
    public class TransitionObjectTMProColor : TransitionObject<TextMeshProUGUI, Color>
    {
        protected override string Name { get; } = "Transition - TMPro Color";
        
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