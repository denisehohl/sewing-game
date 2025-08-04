using UnityEngine;

namespace Ateo.Extensions
{
    public static class TransformExtensions
    {
        #region Set Transform Values
        
        public static void SetPositionRotation(this Transform t, Transform source)
        {
            t.position = source.position;
            t.rotation = source.rotation;
        }
        
        public static void SetPositionRotationScale(this Transform t, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            t.position = position;
            t.rotation = rotation;
            t.localScale = scale;
        }
        
        #endregion
        
        #region Set Local Transform Values

        public static void SetLocalPositionRotation(this Transform t, Vector3 localPosition, Quaternion localRotation)
        {
            t.localPosition = localPosition;
            t.localRotation = localRotation;
        }

        public static void SetLocalPositionRotationScale(this Transform t, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            t.localPosition = localPosition;
            t.localRotation = localRotation;
            t.localScale = localScale;
        }
        
        #endregion

        #region Reset Transform Values

        public static void Reset(this Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public static void ResetPosition(this Transform t)
        {
            t.position = Vector3.zero;
        }

        public static void ResetRotation(this Transform t)
        {
            t.rotation = Quaternion.identity;
        }

        public static void ResetPositionRotation(this Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
        }

        #endregion Values

        #region Reset Local Transform Values

        public static void ResetLocal(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public static void ResetLocalPosition(this Transform t)
        {
            t.localPosition = Vector3.zero;
        }

        public static void ResetLocalRotation(this Transform t)
        {
            t.localRotation = Quaternion.identity;
        }

        public static void ResetLocalPositionRotation(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }

        public static void ResetLocalScale(this Transform t)
        {
            t.localScale = Vector3.one;
        }

        #endregion Values

        public static void FromMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.ExtractScale();
            transform.rotation = matrix.ExtractRotation();
            transform.position = matrix.ExtractPosition();
        }
        
        public static void DestroyChildren(this Transform transform)
        {
            if (transform == null)
                return;

            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);

                var gameObject = child.gameObject;
                gameObject.SetActive(false);
                Object.Destroy(gameObject);
            }
        }
        
        public static string FullPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();

            while (transform != null)
            {
                sb.Insert(0, transform.name);
                sb.Insert(0, '/');
                transform = transform.parent;
            }

#if UNITY_EDITOR
            if (transform && UnityEditor.EditorUtility.IsPersistent(transform))
            {
                sb.Append(" (Asset)");
            }
#endif

            return sb.ToString();
        }
        
        public static string GetPathRelativeTo(this Transform transform, Transform parent)
        {
            if (transform == parent)
                return "";

            return transform.IsChildOf(parent) ? transform.FullPath().Substring(parent.FullPath().Length + 1) : transform.FullPath();
        }
        
        public static T FindInParents<T>(this Transform transform, bool includeSelf = true) where T : Component
        {
            var current = includeSelf ? transform : transform.parent;
            for (; current != null; current = current.parent)
            {
                var comp = current.GetComponent<T>();
                if (comp != null)
                    return comp;
            }

            return null;
        }
    }
}