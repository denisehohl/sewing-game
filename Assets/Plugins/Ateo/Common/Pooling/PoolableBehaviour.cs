using UnityEngine;
using Sirenix.OdinInspector;

namespace Ateo.Common
{
    public class PoolableBehaviour<T> : MonoBehaviour where T: PoolableBehaviour<T>
    {
        [Title("Poolable Object - Properties")]
        public int m_PoolSize;

        public int PoolSize => m_PoolSize;
        public bool IsPooled { get; private set; }

        private Pool<T> _pool;

        public void Initialize(Pool<T> pool)
        {
            _pool = pool;
        }

        public void Activate()
        {
            if(IsPooled)
            {
                IsPooled = false;
            }
        }

        public void Deactivate()
        {
            if(!IsPooled)
            {
                IsPooled = true;
            }
        }
    }
}
