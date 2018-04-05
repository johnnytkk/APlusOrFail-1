using UnityEngine;

namespace APlusOrFail
{
    public static class CollisionUtil
    {
        public enum ColliderSide
        {
            Left,
            Right,
            Top,
            Bottom
        }
        
        public static ContactPoint2D? HasContactForce(ContactPoint2D[] contactPoints, int count, ColliderSide side, float angle, float tolerate)
        {
            for (int i = 0; i < count && i < contactPoints.Length; ++i)
            {
                if (IsContactAtSide(ref contactPoints[i], side) && IsWithinTolerate(ref contactPoints[i], angle, tolerate))
                {
                    return contactPoints[i];
                }
            }
            return null;
        }

        public static bool IsContactAtSide(ref ContactPoint2D contactPoint, ColliderSide side)
        {
            Bounds colliderBounds = contactPoint.otherCollider.bounds;
            switch (side)
            {
                case ColliderSide.Bottom:
                    return contactPoint.point.y <= colliderBounds.min.y;
                case ColliderSide.Top:
                    return contactPoint.point.y >= colliderBounds.max.y;
                case ColliderSide.Left:
                    return contactPoint.point.x <= colliderBounds.min.x;
                case ColliderSide.Right:
                    return contactPoint.point.x >= colliderBounds.max.x;
                default:
                    return false;
            }
        }

        public static bool IsWithinTolerate(ref ContactPoint2D contactPoint, float angle, float tolerate)
        {
            angle = MathUtil.NormalizeRandianAngle(angle);
            tolerate = MathUtil.NormalizeRandianAngle(tolerate);
            return Mathf.Abs(MathUtil.NormalizeRandianAngle(Mathf.Atan2(contactPoint.normal.y, contactPoint.normal.x)) - angle) <= tolerate;
        }

        
    }
}
