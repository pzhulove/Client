

namespace Tenmove.Runtime.Math
{
    public struct Mat3x3
    {
        private float m_m00, m_m01, m_m02;  
        private float m_m10, m_m11, m_m12; 
        private float m_m20, m_m21, m_m22;


        static public readonly Mat3x3 Identity = new Mat3x3(1,0,0,
                                                       0,1,0,
                                                       0,0,1);

        public Mat3x3(Mat3x3 mat)
        {
            m_m00 = mat.m_m00; m_m01 = mat.m_m01; m_m02 = mat.m_m02;
            m_m10 = mat.m_m10; m_m11 = mat.m_m11; m_m12 = mat.m_m12;
            m_m20 = mat.m_m20; m_m21 = mat.m_m21; m_m22 = mat.m_m22;
        }

        public Mat3x3(float m00, float m01, float m02,
                    float m10, float m11, float m12,
                   float m20, float m21, float m22)
        {
            m_m00 = m00; m_m01 = m01; m_m02 = m02;
            m_m10 = m10; m_m11 = m11; m_m12 = m12;
            m_m20 = m20; m_m21 = m21; m_m22 = m22;
        }

        public Mat3x3(Vec3 leftward,Vec3 upward, Vec3 forward)
        {
            m_m00 = leftward.x;
            m_m10 = leftward.y;
            m_m20 = leftward.z;

            m_m01 = upward.x;
            m_m11 = upward.y;
            m_m21 = upward.z;

            m_m02 = forward.x;
            m_m12 = forward.y;
            m_m22 = forward.z;
        }

        public Mat3x3 GetTransposed()
        {
            return new Mat3x3(m_m00, m_m10, m_m20,
                            m_m01, m_m11, m_m21,
                            m_m02, m_m12, m_m22);
        }

        static public Mat3x3 operator * (Mat3x3 _Left, Mat3x3 _Right)
        {
            Mat3x3 res = Identity;

            res.m_m00 = _Left.m_m00 * _Right.m_m00 + _Left.m_m01 * _Right.m_m10 + _Left.m_m02 * _Right.m_m20;
            res.m_m10 = _Left.m_m10 * _Right.m_m00 + _Left.m_m11 * _Right.m_m10 + _Left.m_m12 * _Right.m_m20;
            res.m_m20 = _Left.m_m20 * _Right.m_m00 + _Left.m_m21 * _Right.m_m10 + _Left.m_m22 * _Right.m_m20;

            res.m_m01 = _Left.m_m00 * _Right.m_m01 + _Left.m_m01 * _Right.m_m11 + _Left.m_m02 * _Right.m_m21;
            res.m_m11 = _Left.m_m10 * _Right.m_m01 + _Left.m_m11 * _Right.m_m11 + _Left.m_m12 * _Right.m_m21;
            res.m_m21 = _Left.m_m20 * _Right.m_m01 + _Left.m_m21 * _Right.m_m11 + _Left.m_m22 * _Right.m_m21;

            res.m_m02 = _Left.m_m00 * _Right.m_m02 + _Left.m_m01 * _Right.m_m12 + _Left.m_m02 * _Right.m_m22;
            res.m_m12 = _Left.m_m10 * _Right.m_m02 + _Left.m_m11 * _Right.m_m12 + _Left.m_m12 * _Right.m_m22;
            res.m_m22 = _Left.m_m20 * _Right.m_m02 + _Left.m_m21 * _Right.m_m12 + _Left.m_m22 * _Right.m_m22;

            return res;
        }

        public Quat ToQuaternion()
        {
            float fSum, p, tr = m_m00 + m_m11 + m_m22;
            Quat qRotate = Quat.Identity;
            
            if (tr > 0.0f)
            {
                fSum = Utility.Math.Sqrt(tr + 1.0f);
                p = 0.5f / fSum;

                qRotate.w = fSum * 0.5f;
                qRotate.x = (m_m12 - m_m21) * p;
                qRotate.y = (m_m20 - m_m02) * p;
                qRotate.z = (m_m01 - m_m10) * p;

                return qRotate;
            }

            if ((m_m00 >= m_m11) && (m_m00 >= m_m22))
            {
                fSum = Utility.Math.Sqrt(m_m00 - m_m11 - m_m22 + 1.0f);
                p = 0.5f / fSum;

                qRotate.w = (m_m12 - m_m21) * p;
                qRotate.x = fSum * 0.5f;
                qRotate.y = (m_m01 + m_m10) * p;
                qRotate.z = (m_m02 + m_m20) * p;

                return qRotate;
            }
            
            if ((m_m11 >= m_m00) && (m_m11 >= m_m22))
            {
                fSum = Utility.Math.Sqrt(m_m11 - m_m22 - m_m00 + 1.0f);
                p = 0.5f / fSum;

                qRotate.w = (m_m20 - m_m02) * p;
                qRotate.x = (m_m10 + m_m01) * p;
                qRotate.y = fSum * 0.5f;
                qRotate.z = (m_m12 + m_m21) * p;

                return qRotate;
            }
           
            if ((m_m22 >= m_m00) && (m_m22 >= m_m11))
            {
                fSum = Utility.Math.Sqrt(m_m22 - m_m00 - m_m11 + 1.0f);
                p = 0.5f / fSum;

                qRotate.w = (m_m01 - m_m10) * p;
                qRotate.x = (m_m20 + m_m02) * p;
                qRotate.y = (m_m21 + m_m12) * p;
                qRotate.z = fSum * 0.5f;

                return qRotate;
            }
            
            qRotate.w = 1.0f;
            qRotate.x = 0.0f;
            qRotate.y = 0.0f;
            qRotate.z = 0.0f;

            return qRotate;
        }
    }
}
