using UnityEngine;

namespace Ateo.Extensions
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Converts a Texture2D into a Sprite.
        /// </summary>
        /// <param name="texture">Texture2D to be converted</param>
        public static Sprite ToSprite(this Texture2D texture)
        {
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(texture, rect, pivot);
        }
    }
}