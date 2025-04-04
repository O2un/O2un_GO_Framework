using UnityEngine;

namespace O2un.Core
{
    public static class Math
    {
        #region PERCENTAGE
        public static readonly int C1PERCENT = 10000;
        public static readonly int C100PERCENT = C1PERCENT * 100;

        public static double CalculateRate(float origin, int rate)
        {
            return origin * ((double)rate/C100PERCENT);
        }

        public static double CalculateRate(double origin, double rate)
        {
            return origin * (rate/C100PERCENT);
        }

        public static double CalculateRate(int origin, int rate)
        {
            return origin * ((double)rate/C100PERCENT);
        }
        #endregion

        #region VECTOR
        public static Vector3 Direction2D(Vector3 from , Vector3 to)
        {
            from.z = 0;
            to.z = 0;

            return (to - from).normalized;
        }

        public static Vector3 Clamp(this Vector3 origin, Vector3 min, Vector3 max)
        {
            Vector3 clmap = origin;
            clmap.x = Mathf.Clamp(origin.x, min.x, max.x);
            clmap.y = Mathf.Clamp(origin.y, min.y, max.y);
            clmap.z = Mathf.Clamp(origin.z, min.z, max.z);

            return clmap;
        }

        public static bool IsInsideVector(Vector3 left, Vector3 right, Vector3 mid)
        {
            float angleAB = Vector3.Angle(left, right);
            // A와 C가 이루는 각도
            float angleAC = Vector3.Angle(left, mid);
            // C가 A와 B 사이 각도 범위 내에 있는지 우선 확인
            if (angleAC > angleAB) 
                return false;

            Vector3 crossAB = Vector3.Cross(left, right);
            Vector3 crossAC = Vector3.Cross(left, mid);
            float dotCross = Vector3.Dot(crossAB, crossAC);

            // dotCross가 음수면 C는 A->B의 회전 방향과 반대측에 위치
            return dotCross >= 0;
        }
        #endregion
    }
}
