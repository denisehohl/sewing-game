#if UIGRADIENT
using JoshH.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - Gradient Color", menuName = "Transition Object/Gradient Color", order = 0)]
    public class TransitionObjectGradientColor : TransitionObject<UIGradient, GradientColors>
    {
        protected override string Name { get; } = "Transition - Gradient Color";
        
        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = GradientColors.Lerp(Properties.MinValue, Properties.MaxValue, eval);
                
                Component.LinearColor1 = value.Color1;
                Component.LinearColor2 = value.Color2;

                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public struct GradientColors
    {
        [HideLabel]
        public Color Color1;
        
        [HideLabel]
        public Color Color2;

        public GradientColors(Color a, Color b)
        {
            Color1 = a;
            Color2 = b;
        }

        public static GradientColors Lerp(GradientColors a, GradientColors b, float t)
        {
            return new GradientColors(Color.Lerp(a.Color1, b.Color1, t), Color.Lerp(a.Color2, b.Color2, t));
        }
    }
}
#endif