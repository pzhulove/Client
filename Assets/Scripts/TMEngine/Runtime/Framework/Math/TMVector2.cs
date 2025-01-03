
namespace Tenmove.Runtime.Math
{
    public struct Vec2
    {
        private float m_X;
        private float m_Y;

        static public readonly Vec2 Zero = new Vec2(0, 0);
        static public readonly Vec2 One = new Vec2(1, 1);

        public Vec2(Vec2 vec2)
        {
            m_X = vec2.m_X;
            m_Y = vec2.m_Y;
        }

        public Vec2(Vec3 vec3)
        {
            m_X = vec3.x;
            m_Y = vec3.y;
        }

        public Vec2(float x, float y)
        {
            m_X = x;
            m_Y = y;
        }

        public Vec2(float real)
        {
            m_X = real;
            m_Y = real;
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
        
        public float Length
        {
            get { return (float)System.Math.Sqrt(m_X * m_X + m_Y * m_Y); }
        }

        public float LengthSquared
        {
            get { return m_X * m_X + m_Y * m_Y; }
        }


        public Vec2 Normalized
        {
            get
            {
                Vec2 res = this;
                res.Normalize();
                return res;
            }
        }

        public Vec2 Fliped
        {
            get
            {
                Vec2 res = this;
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
        }

        public void Flip()
        {
            m_X = -m_X;
            m_Y = -m_Y;
        }

        public Vec2 Lerp(Vec2 to, float factor)
        {
            Vec2 res;
            factor = Utility.Math.Clamp(factor, 0.0f, 1.0f);
            float oneMinusFactor = 1.0f - factor;
            res.m_X = m_X * oneMinusFactor + to.m_X * factor;
            res.m_Y = m_Y * oneMinusFactor + to.m_Y * factor;

            return res;
        }

        public float DotProduct(Vec2 right)
        {
            return m_X * right.m_X + m_Y * right.m_Y;
        }

        public void SetLength(float length)
        {
            float lenInv = Length;
            lenInv = Length / ((0 <= lenInv && lenInv < float.Epsilon) ? float.Epsilon : lenInv);
            m_X *= lenInv;
            m_Y *= lenInv;
        }

        static public Vec2 operator +(Vec2 _left, float _right)
        {
            return new Vec2(_left.m_X + _right, _left.m_Y + _right);
        }

        static public Vec2 operator -(Vec2 _left, float _right)
        {
            return new Vec2(_left.m_X - _right, _left.m_Y - _right);
        }

        static public Vec2 operator *(Vec2 _left, float _right)
        {
            return new Vec2(_left.m_X * _right, _left.m_Y * _right);
        }

        static public Vec2 operator /(Vec2 _left, float _right)
        {
            float right = (0 <= _right && _right < float.Epsilon) ? float.Epsilon : _right;

            return new Vec2(_left.m_X / right, _left.m_Y / right);
        }

        static public Vec2 operator +(Vec2 _left, Vec2 _right)
        {
            return new Vec2(_left.m_X + _right.m_X, _left.m_Y + _right.m_Y);
        }

        static public Vec2 operator -(Vec2 _left, Vec2 _right)
        {
            return new Vec2(_left.m_X - _right.m_X, _left.m_Y - _right.m_Y);
        }

        static public Vec2 operator *(Vec2 _left, Vec2 _right)
        {
            return new Vec2(_left.m_X * _right.m_X, _left.m_Y * _right.m_Y);
        }

        static public Vec2 operator /(Vec2 _left, Vec2 _right)
        {
            float x = (0 <= _right.m_X && _right.m_X < float.Epsilon) ? float.Epsilon : _right.m_X;
            float y = (0 <= _right.m_Y && _right.m_Y < float.Epsilon) ? float.Epsilon : _right.m_Y;

            return new Vec2(_left.m_X / x, _left.m_Y / y);
        }

        static public bool operator ==(Vec2 _left, Vec2 _right)
        {
            return _left.x == _right.x && _left.y == _right.y;
        }

        static public bool operator !=(Vec2 _left, Vec2 _right)
        {
            return _left.x != _right.x || _left.y != _right.y;
        }
    }
}