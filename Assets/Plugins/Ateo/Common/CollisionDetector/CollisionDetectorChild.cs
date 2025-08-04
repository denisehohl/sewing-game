using UnityEngine;

namespace Ateo
{
    public class CollisionDetectorChild : MonoBehaviour
    {
        public CollisionDetector CollisionDetector;

        private bool _isInitialized = false;

        private void Awake()
        {
            _isInitialized = CollisionDetector != null;
        }

        public void Initialize(CollisionDetector collisionDetector)
        {
            if (collisionDetector == null) return;
            
            CollisionDetector = collisionDetector;
            _isInitialized = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_isInitialized)
            {
                CollisionDetector.OnTriggerEnter(other);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if(_isInitialized)
            {
                CollisionDetector.OnTriggerExit(other);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if(_isInitialized)
            {
                CollisionDetector.OnCollisionEnter(other);
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if(_isInitialized)
            {
                CollisionDetector.OnCollisionExit(other);
            }
        }
    }
}
