
namespace Tenmove.Runtime.Math
{
    public struct Rect
    {
        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;

        static public readonly Rect Zero = new Rect(0, 0, 0, 0);

        public Rect(Rect rect)
        {
            m_X = rect.x;
            m_Y = rect.y;
            m_Width = rect.Width;
            m_Height = rect.Height;
        }

        public Rect(Vec2 position, Vec2 size)
        {
            m_X = position.x;
            m_Y = position.y;
            m_Width = size.x;
            m_Height = size.y;
        }

        public Rect(float x, float y, float width, float height)
        {
            m_X = x;
            m_Y = y;
            m_Width = width;
            m_Height = height;
        }

        public float x
        {
            get { return m_X; }
        }

        public float y
        {
            get { return m_Y; }
        }

        public float Width
        {
            get { return m_Width; }
        }

        public float Height
        {
            get { return m_Height; }
        }
    }
}