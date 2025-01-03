


namespace Tenmove.Runtime.Math
{
    public struct Transform
    {
        private Vec3 m_Position;
        private Quat m_Rotation;
        private Vec3 m_Scale;

        static public readonly Transform Identity = new Transform(Vec3.Zero,Quat.Identity,Vec3.One);

        public Transform(Vec3 pos,Quat rot,Vec3 scale)
        {
            m_Position = pos;
            m_Rotation = rot;
            m_Scale = scale;
        }

        public Transform(Transform transform)
        {
            m_Position = transform.Position;
            m_Rotation = transform.Rotation;
            m_Scale = transform.Scale;
        }

        public Vec3 Position
        {
            set { m_Position = value; }
            get { return m_Position; }
        }

        public Quat Rotation
        {
            set { m_Rotation = value; }
            get { return m_Rotation; }
        }

        public Vec3 Scale
        {
            set { m_Scale = value; }
            get { return m_Scale; }
        }
    }
}