
namespace Tenmove.Runtime.Math
{
    public struct Vec4
    {
        private float m_X;
        private float m_Y;
        private float m_Z;
        private float m_W;

        static public readonly Vec4 Zero = new Vec4(0, 0, 0,0);
        static public readonly Vec4 One = new Vec4(1, 1, 1,1);

        public const float kEpsilon = 1E-05F;
        public const float kEpsilonNormalSqrt = 1E-15F;

        public Vec4(Vec4 vec4)
        {
            m_X = vec4.m_X;
            m_Y = vec4.m_Y;
            m_Z = vec4.m_Z;
            m_W = vec4.m_W;
        }

        public Vec4(Vec3 vec3,float w)
        {
            m_X = vec3.x;
            m_Y = vec3.y;
            m_Z = vec3.z;
            m_W = w;
        }

        public Vec4(float x, float y, float z,float w)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = w;
        }

        public Vec4(float real)
        {
            m_X = real;
            m_Y = real;
            m_Z = real;
            m_W = real;
        }

        public float x
        {
            set { m_X = value; }
            get { return m_X; }
        }

        public float y
        {
            set { m_Y = value; }
            get { return m_Y; }
        }

        public float z
        {
            set { m_Z = value; }
            get { return m_Z; }
        }

        public float w
        {
            set { m_W = value; }
            get { return m_W; }
        }

        public float Length
        {
            get { return (float)System.Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z + m_W * m_W); }
        }

        public float LengthSquared
        {
            get { return m_X * m_X + m_Y * m_Y + m_Z * m_Z + m_W * m_W; }
        }

        public Vec4 Normalized
        {
            get
            {
                Vec4 res = this;
                res.Normalize();
                return res;
            }
        }

        public Vec4 Fliped
        {
            get
            {
                Vec4 res = this;
                res.Flip();
                return res;
            }
        }

        public void Normalize()
        {
            float lenInv = Length;
            lenInv = 1.0f / ((0 <= lenInv && lenInv < float.Epsilon) ? float.Epsilon : lenInv);
            m_X *= lenInv;
            m_Y *= lenInv;
            m_Z *= lenInv;
            m_W *= lenInv;
        }

        public void Flip()
        {
            m_X = -m_X;
            m_Y = -m_Y;
            m_Z = -m_Z;
            m_W = -m_W;
        }

        public Vec4 Lerp(Vec4 to,float factor)
        {
            Vec4 res;
            factor = Utility.Math.Clamp(factor, 0.0f, 1.0f);
            float oneMinusFactor = 1.0f - factor;
            res.m_X = m_X * oneMinusFactor + to.m_X * factor;
            res.m_Y = m_Y * oneMinusFactor + to.m_Y * factor;
            res.m_Z = m_Z * oneMinusFactor + to.m_Z * factor;
            res.m_W = m_W * oneMinusFactor + to.m_W * factor;

            return res;
        }

        public float DotProduct(Vec4 right)
        {
            return m_X * right.m_X + m_Y * right.m_Y + m_Z * right.m_Z + m_W * right.m_W;
        }

        public void SetLength(float length)
        {
            float lenInv = Length;
            lenInv = Length / ((0 <= lenInv && lenInv < float.Epsilon) ? float.Epsilon : lenInv);
            m_X *= lenInv;
            m_Y *= lenInv;
            m_Z *= lenInv;
            m_W *= lenInv;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", m_X, m_Y, m_Z,m_W);
        }

        static public Vec4 operator +(Vec4 _left, float _right)
        {
            return new Vec4(_left.m_X + _right, _left.m_Y + _right, _left.m_Z + _right, _left.m_W + _right);
        }

        static public Vec4 operator -(Vec4 _left, float _right)
        {
            return new Vec4(_left.m_X - _right, _left.m_Y - _right, _left.m_Z - _right, _left.m_W - _right);
        }

        static public Vec4 operator *(Vec4 _left, float _right)
        {
            return new Vec4(_left.m_X * _right, _left.m_Y * _right, _left.m_Z * _right, _left.m_W * _right);
        }

        static public Vec4 operator /(Vec4 _left, float _right)
        {
            float oneOverRight = (0 <= _right && _right < float.Epsilon) ? 1/float.Epsilon : 1/_right;
            return new Vec4(_left.m_X * oneOverRight, _left.m_Y * oneOverRight, _left.m_Z * oneOverRight, _left.m_W * oneOverRight);
        }

        static public Vec4 operator + (Vec4 _left, Vec4 _right)
        {
            return new Vec4(_left.m_X + _right.m_X, _left.m_Y + _right.m_Y, _left.m_Z + _right.m_Z, _left.m_W + _right.m_W);
        }

        static public Vec4 operator - (Vec4 _left, Vec4 _right)
        {
            return new Vec4(_left.m_X - _right.m_X, _left.m_Y - _right.m_Y, _left.m_Z - _right.m_Z, _left.m_W - _right.m_W);
        }

        static public Vec4 operator *(Vec4 _left, Vec4 _right)
        {
            return new Vec4(_left.m_X * _right.m_X, _left.m_Y * _right.m_Y, _left.m_Z * _right.m_Z, _left.m_W * _right.m_W);
        }

        static public Vec4 operator /(Vec4 _left, Vec4 _right)
        {
            float x = (0 <= _right.m_X && _right.m_X < float.Epsilon) ? float.Epsilon : _right.m_X;
            float y = (0 <= _right.m_Y && _right.m_Y < float.Epsilon) ? float.Epsilon : _right.m_Y;
            float z = (0 <= _right.m_Z && _right.m_Z < float.Epsilon) ? float.Epsilon : _right.m_Z;
            float w = (0 <= _right.m_W && _right.m_W < float.Epsilon) ? float.Epsilon : _right.m_W;
            return new Vec4(_left.m_X / x, _left.m_Y / y, _left.m_Z / z, _left.m_Z / w);
        }

        static public bool operator == (Vec4 _left, Vec4 _right)
        {
            return _left.m_X == _right.m_X && _left.m_Y == _right.m_Y && _left.m_Z == _right.m_Z && _left.m_W == _right.m_W;
        }

        static public bool operator !=(Vec4 _left, Vec4 _right)
        {
            return _left.m_X != _right.m_X || _left.m_Y != _right.m_Y || _left.m_Z != _right.m_Z || _left.m_W != _right.m_W;
        }
    }
}