using UnityEngine;

namespace Ateo.UI
{
    [CreateAssetMenu(fileName = "TransitionObject - Transform LocalScale", menuName = "Transition Object/Transform LocalScale", order = 0)]
    public class TransitionObjectTransformLocalScale : TransitionObject<Transform, Vector3>
    {
        protected override string Name { get; } = "Transition - Transform LocalScale";

        public override bool Execute(float displacement, bool force = false)
        {
            if (base.Execute(displacement, force))
            {
                var eval = Properties.Function.Evaluate(displacement);
                var value = Properties.MinValue + (Properties.MaxValue - Properties.MinValue) * eval;
                Component.localScale = value;

                return true;
            }

            return false;
        }
    }
}