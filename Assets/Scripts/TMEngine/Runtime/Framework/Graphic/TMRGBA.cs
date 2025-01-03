
namespace Tenmove.Runtime.Graphic
{
    public struct RGBA
    {
        private float m_R;
        private float m_G;
        private float m_B;
        private float m_A;

        static public readonly RGBA Black = new RGBA(0);
        static public readonly RGBA White = new RGBA(1);
        static public readonly RGBA Red = new RGBA(1,0,0);
        static public readonly RGBA Green = new RGBA(0, 1, 0);
        static public readonly RGBA Blue = new RGBA(0, 0, 1);
        static public readonly RGBA Yellow = new RGBA(1, 1, 0);
        static public readonly RGBA Purple = new RGBA(1, 0, 1);

        public RGBA(RGBA rgba)
        {
            m_R = rgba.m_R;
            m_G = rgba.m_G;
            m_B = rgba.m_B;
            m_A = rgba.m_A;
        }

        public RGBA(float r, float g, float b, float a)
        {
            m_R = r;
            m_G = g;
            m_B = b;
            m_A = a;
        }

        public RGBA(float r, float g, float b)
        {
            m_R = r;
            m_G = g;
            m_B = b;
            m_A = 1;
        }

        public RGBA(float real)
        {
            m_R = real;
            m_G = real;
            m_B = real;
            m_A = real;
        }

        public float r
        {
            set { m_R = value; }
            get { return m_R; }
        }

        public float g
        {
            set { m_G = value; }
            get { return m_G; }
        }

        public float b
        {
            set { m_B = value; }
            get { return m_B; }
        }

        public float a
        {
            set { m_A = value; }
            get { return m_A; }
        }

        public static bool operator ==(RGBA _left, RGBA _right)
        {
            return  _left.r == _right.r && _left.g == _right.g && _left.b == _right.b && _left.a == _right.a;
        }

        public static bool operator !=(RGBA _left, RGBA _right)
        {
            return _left.r != _right.r || _left.g != _right.g || _left.b != _right.b || _left.a != _right.a;
        }
    }
}