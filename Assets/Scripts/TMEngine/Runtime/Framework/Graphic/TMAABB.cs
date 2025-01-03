


using Tenmove.Runtime.Math;

namespace Tenmove.Runtime.Graphic
{
    public struct AABB
    {
        public static readonly AABB Invalid = new AABB(float.MaxValue, float.MaxValue, float.MaxValue, float.MinValue, float.MinValue, float.MinValue);

        private Vec3 m_Min;
        private Vec3 m_Max;

        public AABB(Vec3 min,Vec3 max)
        {
            m_Min = min;
            m_Max = max;
        }

        public AABB(float xMin, float yMin, float zMin, float xMax, float yMax, float zMax)
        {
            m_Min = new Vec3(xMin,yMin,zMin);
            m_Max = new Vec3(xMax, yMax, zMax);
        }

        public AABB(AABB aabb)
        {
            m_Min = aabb.m_Min;
            m_Max = aabb.m_Max; 
        }

        public Vec3 Max
        {
            set { m_Max = value; }
            get { return m_Max; }
        }

        public Vec3 Min
        {
            set { m_Min = value; }
            get { return m_Min; }
        }

        public bool IsOverlap(AABB other,out AABB intersectBox)
        {
            intersectBox = Invalid;
            if (Min.x < other.Max.x && other.Min.x < Max.x )
            {
                if (Min.y < other.Max.y && other.Min.y < Max.y)
                {
                    if (Min.z < other.Max.z && other.Min.z < Max.z)
                    {
                        intersectBox.Min = new Vec3(Utility.Math.Max(Min.x, other.Min.x),
                                                 Utility.Math.Max(Min.y, other.Min.y),
                                                 Utility.Math.Max(Min.z, other.Min.z));

                        intersectBox.Max = new Vec3(Utility.Math.Min(Max.x, other.Max.x),
                                                 Utility.Math.Min(Max.y, other.Max.y),
                                                 Utility.Math.Min(Max.z, other.Max.z));

                        return true;
                    }
                }
            }

            return false;
        }

        static public AABB operator | (AABB _left,AABB _right)
        {
            return new AABB( Utility.Math.Min(_left.Min.x, _right.Min.x),
                            Utility.Math.Min(_left.Min.y, _right.Min.y),
                            Utility.Math.Min(_left.Min.z, _right.Min.z),
                            Utility.Math.Max(_left.Max.x, _right.Max.x),
                            Utility.Math.Max(_left.Max.y, _right.Max.y),
                            Utility.Math.Max(_left.Max.z, _right.Max.z));
        }

        static public AABB operator & (AABB _left,AABB _right)
        {
            return new AABB( Utility.Math.Max(_left.Min.x, _right.Min.x),
                            Utility.Math.Max(_left.Min.y, _right.Min.y),
                            Utility.Math.Max(_left.Min.z, _right.Min.z),
                            Utility.Math.Min(_left.Max.x, _right.Max.x),
                            Utility.Math.Min(_left.Max.y, _right.Max.y),
                            Utility.Math.Min(_left.Max.z, _right.Max.z));
        }
        
    }
}