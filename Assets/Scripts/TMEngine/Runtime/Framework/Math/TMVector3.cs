
namespace Tenmove.Runtime.Math
{
    public struct Vec3
    {
        private float m_X;
        private float m_Y;
        private float m_Z;

        static public readonly Vec3 Zero = new Vec3(0, 0, 0);
        static public readonly Vec3 One = new Vec3(1, 1, 1);
        static public readonly Vec3 Upward = new Vec3(0, 1, 0);
        static public readonly Vec3 Forward = new Vec3(0, 0, 1);
        static public readonly Vec3 Leftward = new Vec3(1, 0, 0);

        public const float kEpsilon = 1E-05F;
        public const float kEpsilonNormalSqrt = 1E-15F;

        public Vec3(Vec3 triple)
        {
            m_X = triple.m_X;
            m_Y = triple.m_Y;
            m_Z = triple.m_Z;
        }

        public Vec3(Vec2 vec2,float z)
        {
            m_X = vec2.x;
            m_Y = vec2.y;
            m_Z = z;
        }

        public Vec3(float x,Vec2 vec2)
        {
            m_X = x;
            m_Y = vec2.x;
            m_Z = vec2.y;
        }

        public Vec3(float x, float y, float z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        public Vec3(float real)
        {
            m_X = real;
            m_Y = real;
            m_Z = real;
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

        public float Length
        {
            get { return (float)System.Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z); }
        }

        public float LengthSquared
        {
            get { return m_X * m_X + m_Y * m_Y + m_Z * m_Z; }
        }


        public Vec3 Normalized
        {
            get
            {
                Vec3 res = this;
                res.Normalize();
                return res;
            }
        }

        public Vec3 Fliped
        {
            get
            {
                Vec3 res = this;
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
        }

        public void Flip()
        {
            m_X = -m_X;
            m_Y = -m_Y;
            m_Z = -m_Z;
        }

        public Vec3 Lerp(Vec3 to,float factor)
        {
            Vec3 res;
            factor = Utility.Math.Clamp(factor, 0.0f, 1.0f);
            float oneMinusFactor = 1.0f - factor;
            res.m_X = m_X * oneMinusFactor + to.m_X * factor;
            res.m_Y = m_Y * oneMinusFactor + to.m_Y * factor;
            res.m_Z = m_Z * oneMinusFactor + to.m_Z * factor;

            return res;
        }

        public float DotProduct(Vec3 right)
        {
            return m_X * right.m_X + m_Y * right.m_Y + m_Z * right.m_Z;
        }

        public Vec3 CrossProduct(Vec3 right)
        {
            return new Vec3(m_Y * right.m_Z - m_Z * right.m_Y, m_Z * right.m_X - m_X * right.m_Z, m_X * right.m_Y - m_Y * right.m_X);
        }

        public float GetVolume()
        {
            return m_X * m_Y * m_Z;
        }

        public void SetLength(float length)
        {
            float lenInv = Length;
            lenInv = Length / ((0 <= lenInv && lenInv < float.Epsilon) ? float.Epsilon : lenInv);
            m_X *= lenInv;
            m_Y *= lenInv;
            m_Z *= lenInv;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", m_X, m_Y, m_Z);
        }

        static public Vec3 operator +(Vec3 _left, float _right)
        {
            return new Vec3(_left.m_X + _right, _left.m_Y + _right, _left.m_Z + _right);
        }

        static public Vec3 operator -(Vec3 _left, float _right)
        {
            return new Vec3(_left.m_X - _right, _left.m_Y - _right, _left.m_Z - _right);
        }

        static public Vec3 operator *(Vec3 _left, float _right)
        {
            return new Vec3(_left.m_X * _right, _left.m_Y * _right, _left.m_Z * _right);
        }

        static public Vec3 operator /(Vec3 _left, float _right)
        {
            float right = (0 <= _right && _right < float.Epsilon) ? float.Epsilon : _right;

            return new Vec3(_left.m_X / right, _left.m_Y / right, _left.m_Z / right);
        }

        static public Vec3 operator + (Vec3 _left, Vec3 _right)
        {
            return new Vec3(_left.m_X + _right.m_X, _left.m_Y + _right.m_Y, _left.m_Z + _right.m_Z);
        }

        static public Vec3 operator - (Vec3 _left, Vec3 _right)
        {
            return new Vec3(_left.m_X - _right.m_X, _left.m_Y - _right.m_Y, _left.m_Z - _right.m_Z);
        }

        static public Vec3 operator *(Vec3 _left, Vec3 _right)
        {
            return new Vec3(_left.m_X * _right.m_X, _left.m_Y * _right.m_Y, _left.m_Z * _right.m_Z);
        }

        static public Vec3 operator /(Vec3 _left, Vec3 _right)
        {
            float x = (0 <= _right.m_X && _right.m_X < float.Epsilon) ? float.Epsilon : _right.m_X;
            float y = (0 <= _right.m_Y && _right.m_Y < float.Epsilon) ? float.Epsilon : _right.m_Y;
            float z = (0 <= _right.m_Z && _right.m_Z < float.Epsilon) ? float.Epsilon : _right.m_Z;

            return new Vec3(_left.m_X / x, _left.m_Y / y, _left.m_Z / z);
        }

        static public bool operator == (Vec3 _left, Vec3 _right)
        {
            return _left.x == _right.x && _left.y == _right.y && _left.z == _right.z;
        }

        static public bool operator !=(Vec3 _left, Vec3 _right)
        {
            return _left.x != _right.x || _left.y != _right.y || _left.z != _right.z;
        }


    }
}