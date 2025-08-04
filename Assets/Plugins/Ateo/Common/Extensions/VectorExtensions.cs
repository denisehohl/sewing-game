using UnityEngine;

namespace Ateo.Extensions
{
    public static class VectorExtentions
    {
        public static Vector3 FlatX(this Vector3 vector)
        {
            return Vector3.Scale(vector, new Vector3(0f, 1f, 1f));
        }

        public static Vector3 FlatY(this Vector3 vector)
        {
            return Vector3.Scale(vector, new Vector3(1f, 0f, 1f));
        }

        public static Vector3 FlatZ(this Vector3 vector)
        {
            return Vector3.Scale(vector, new Vector3(1f, 1f, 0f));
        }

        public static Vector3 SetX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        // Source: https://forum.unity.com/threads/delta-angle-for-one-axis-on-a-custom-plane.457258/#post-2967371
        /// <summary>
        /// Calculates the the angle between two position vectors regarding to a reference point (plane), with a referenceUp
        /// a sign in which direction it points can be calculated (clockwise is positive and counter clockwise is negative)
        /// </summary>
        public static float SignedAngleFromPosition(this Vector3 referencePoint, Vector3 fromPoint, Vector3 toPoint,
            Vector3 referenceUp)
        {
            // Calculate the direction vector pointing from referencePoint towards fromPoint
            var fromDir = fromPoint - referencePoint;

            // Calculate the direction vector pointing from referencePoint towards toPoint
            var toDir = toPoint - referencePoint;

            // Calculate the plane normal (perpendicular vector)
            var planeNormal = Vector3.Cross(fromDir, toDir);

            // Calculate the angle between the 2 direction vectors (note: its always the smaller one smaller than 180°)
            var angle = Vector3.Angle(fromDir, toDir);

            // Calculate weather the normal and the referenceUp point in the same direction (>0) or not (<0), http://docs.unity3d.com/Documentation/Manual/ComputingNormalPerpendicularVector.html
            var orientationDot = Vector3.Dot(planeNormal, referenceUp);

            // The angle is positive (clockwise orientation seen from referenceUp)
            if (orientationDot > 0.0f)
                return angle;

            // The angle is negative (counter-clockwise orientation seen from referenceUp)
            return -angle;
        }

        // Source: https://forum.unity.com/threads/delta-angle-for-one-axis-on-a-custom-plane.457258/#post-2967371
        /// <summary>
        /// Calculates the the angle between two direction vectors, with a referenceUp a sign in which direction it points
        /// can be calculated (clockwise is positive and counter clockwise is negative)
        /// </summary>
        public static float SignedAngleFromDirection(this Vector3 fromDir, Vector3 toDir, Vector3 referenceUp)
        {
            // Calculate the plane normal (perpendicular vector)
            var planeNormal = Vector3.Cross(fromDir, toDir);

            // Calculate the angle between the 2 direction vectors (note: its always the smaller one smaller than 180°)
            var angle = Vector3.Angle(fromDir, toDir);

            // Calculate weather the normal and the referenceUp point in the same direction (>0) or not (<0), http://docs.unity3d.com/Documentation/Manual/ComputingNormalPerpendicularVector.html
            var orientationDot = Vector3.Dot(planeNormal, referenceUp);

            // The angle is positive (clockwise orientation seen from referenceUp)
            if (orientationDot > 0.0f)
                return angle;

            // The angle is negative (counter-clockwise orientation seen from referenceUp)
            return -angle;
        }
    }
}