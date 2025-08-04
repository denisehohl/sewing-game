using UnityEngine;

namespace Ateo.UI
{
    public abstract class Pagination : MonoBehaviour
    {
        public int Index { get; protected set; }
        public int Count { get; protected set; }

        public abstract void Initialize(int count, int index = 0);
        public abstract void SelectElement(int index, bool instant = false);
    }
}