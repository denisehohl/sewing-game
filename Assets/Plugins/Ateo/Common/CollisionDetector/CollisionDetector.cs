using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Ateo.Extensions;

namespace Ateo
{
    public class CollisionDetector : MonoBehaviour
    {
        public enum CollisionType
        {
            Collision,
            Trigger
        }

        public enum CompareType
        {
            None,
            Tag,
            LayerMask
        }

        public CollisionType TypeOfCollision;
        public CompareType TypeofCompare;

        [ShowIf("ShowCheckForTag"), TagSelector]
        public string CheckForTag;

        [ShowIf("ShowLayerMask")]
        public LayerMask LayerMask;

        public bool IsTriggered { get; private set; }
        public bool IsColliding { get; private set; }

        public delegate void Handler();

        public delegate void HandlerCollider(Collider collider);

        public event Handler OnEnter;
        public event Handler OnExit;

        public event HandlerCollider OnColliderEnter;
        public event HandlerCollider OnColliderExit;

        [SerializeField]
        private UnityEvent _onEnter;

        [SerializeField]
        private UnityEvent _onExit;

        public readonly List<Collider> Colliders = new List<Collider>();

        private void Awake()
        {
            var children = GetComponentsInChildren<CollisionDetectorChild>();

            if (children != null && children.Length > 0)
            {
                foreach (var child in children)
                {
                    child.Initialize(this);
                }
            }
        }

        public bool ContainsCollider(Collider coll)
        {
            foreach (var c in Colliders)
            {
                if (c == coll)
                    return true;
            }

            return false;
        }

        protected virtual bool Enter(Collider other)
        {
            switch (TypeofCompare)
            {
                case CompareType.Tag when !other.CompareTag(CheckForTag):
                case CompareType.LayerMask when !LayerMask.Contains(other.gameObject.layer):
                    return false;
            }

            if (TypeOfCollision == CollisionType.Trigger)
                IsTriggered = true;
            else
                IsColliding = true;

            if (!Colliders.Contains(other))
                Colliders.Add(other);

            if (Colliders.Count == 1)
            {
                OnEnter?.Invoke();
                _onEnter?.Invoke();
            }

            OnColliderEnter?.Invoke(other);
            return true;
        }

        protected virtual  bool Exit(Collider other)
        {
            if (other != null)
            {
                switch (TypeofCompare)
                {
                    case CompareType.Tag when !other.CompareTag(CheckForTag):
                    case CompareType.LayerMask when !LayerMask.Contains(other.gameObject.layer):
                        return false;
                }

                if (Colliders.Contains(other))
                    Colliders.Remove(other);
            }

            if (Colliders.Count == 0)
            {
                if (TypeOfCollision == CollisionType.Trigger)
                    IsTriggered = false;
                else
                    IsColliding = false;

                OnExit?.Invoke();
                _onExit?.Invoke();
            }

            OnColliderExit?.Invoke(other);
            return true;
        }

        private void Update()
        {
            if (!IsTriggered && !IsColliding) return;

            var nullCount = 0;

            for (var i = 0; i < Colliders.Count; i++)
            {
                if (Colliders[i] == null)
                    nullCount++;
            }

            if (nullCount != Colliders.Count) return;

            Colliders.Clear();
            Exit(null);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (TypeOfCollision == CollisionType.Trigger)
            {
                Enter(other);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (TypeOfCollision == CollisionType.Trigger)
            {
                Exit(other);
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            if (TypeOfCollision == CollisionType.Collision)
            {
                Enter(other.collider);
            }
        }

        public void OnCollisionExit(Collision other)
        {
            if (TypeOfCollision == CollisionType.Collision)
            {
                Exit(other.collider);
            }
        }

#if UNITY_EDITOR
        private bool ShowCheckForTag()
        {
            return TypeofCompare == CompareType.Tag;
        }

        private bool ShowLayerMask()
        {
            return TypeofCompare == CompareType.LayerMask;
        }
#endif
    }
}