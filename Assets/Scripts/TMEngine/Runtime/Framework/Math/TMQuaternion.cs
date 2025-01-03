namespace Tenmove.Runtime.Math
{
    public struct Quat
    {
        private float m_X;
        private float m_Y;
        private float m_Z;
        private float m_W;
        
        static public readonly Quat Identity = new Quat(0,0,0,1);

        public Quat(Quat quat)
        {
            m_X = quat.m_X;
            m_Y = quat.m_Y;
            m_Z = quat.m_Z;
            m_W = quat.m_W;
        }

        public Quat(float x, float y,float z,float w)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = w;
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

        static public bool operator ==(Quat _left, Quat _right)
        {
            return _left.x == _right.x && _left.y == _right.y && _left.z == _right.z && _left.w == _right.w;
        }

        static public bool operator !=(Quat _left, Quat _right)
        {
            return _left.x != _right.x || _left.y != _right.y || _left.z != _right.z || _left.w != _right.w;
        }
    }
}