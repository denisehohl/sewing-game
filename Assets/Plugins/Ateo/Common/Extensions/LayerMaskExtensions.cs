using UnityEngine;

namespace Ateo.Extensions
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Extension method to check if a layer is in a LayerMask
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}