using UnityEngine;

namespace Ateo.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a copy of this color with the specified <paramref name="alpha"/>
        /// </summary>
        public static Color SetAlpha(this Color self, float alpha)
        {
            return new Color(self.r, self.g, self.b, alpha);
        }
    }
}