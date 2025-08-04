using System.Collections.Generic;
using UnityEngine;

namespace Ateo.Common
{
    public class Pool<T> where T : PoolableBehaviour<T>
    {
        private readonly Queue<T> _pooledInstances = new Queue<T>();
        private readonly GameObject _prefab;
        private readonly Transform _poolParent;

        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _poolParent = new GameObject($"{typeof(T).Name} - Pool").transform;

            var instance = CreateInstance();

            if (instance != null)
            {
                AddToPool(instance);
                
                var poolSize = instance.PoolSize - 1;

                for (var i = 0; i < poolSize; i++)
                {
                    instance = CreateInstance();
                    AddToPool(instance);
                }
            }
        }

        public T GetFromPool()
        {
            T instance = null;

            instance = _pooledInstances.Count > 0 ? _pooledInstances.Dequeue() : CreateInstance();

            if (instance != null)
            {
                instance.gameObject.SetActive(true);
                instance.Activate();
            }

            return instance;
        }

        public void AddToPool(PoolableBehaviour<T> instance)
        {
            instance.Deactivate();
            instance.transform.SetParent(_poolParent);
            instance.gameObject.SetActive(false);

            _pooledInstances.Enqueue((T)instance);
        }

        private T CreateInstance()
        {
            var gos = GameObject.Instantiate(_prefab);
            var instance = gos.GetComponent<T>();

            if (instance != null)
            {
                instance.Initialize(this);
                return instance;
            }

            GameObject.Destroy(gos);
            return null;
        }
    }
}
