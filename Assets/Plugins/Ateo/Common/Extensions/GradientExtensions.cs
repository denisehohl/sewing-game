using System.Collections.Generic;
using UnityEngine;

namespace Ateo.Extensions
{
    public static class Gradient
    {
        public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, false);
        }

        public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, true, false);
        }

        public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, true);
        }

        private static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
        {
            //list of all the unique key times
            var keysTimes = new List<float>();

            if (!noColor)
            {
                for (var i = 0; i < a.colorKeys.Length; i++)
                {
                    var k = a.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (var i = 0; i < b.colorKeys.Length; i++)
                {
                    var k = b.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (var i = 0; i < a.alphaKeys.Length; i++)
                {
                    var k = a.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (var i = 0; i < b.alphaKeys.Length; i++)
                {
                    var k = b.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            var clrs = new UnityEngine.GradientColorKey[keysTimes.Count];
            var alphas = new UnityEngine.GradientAlphaKey[keysTimes.Count];

            //Pick colors of both UnityEngine.Gradients at key times and lerp them
            for (var i = 0; i < keysTimes.Count; i++)
            {
                var key = keysTimes[i];
                var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                clrs[i] = new UnityEngine.GradientColorKey(clr, key);
                alphas[i] = new UnityEngine.GradientAlphaKey(clr.a, key);
            }

            var g = new UnityEngine.Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }
    }
}