using System;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/TextNoMask", 10)]
    public class TextProNoMask : Text
    {
        UInt32 m_textVersion;
        UInt32 m_preferredWidthVersion;
        float m_preferredWidth;
        UInt32 m_preferredHeightVersion;
        float m_preferredHeight;

        protected override void Awake()
        {
            base.Awake();
            maskable = false;
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            m_textVersion++;
        }

        public override float preferredWidth
        {
            get
            {
                if (m_preferredWidthVersion != m_textVersion)
                {
                    m_preferredWidth = base.preferredWidth;
                    m_preferredWidthVersion = m_textVersion;
                }

                return m_preferredWidth;
            }
        }

        public override float preferredHeight
        {
            get
            {
                if (m_preferredHeightVersion != m_textVersion)
                {
                    m_preferredHeight = base.preferredHeight;
                    m_preferredHeightVersion = m_textVersion;
                }

                return m_preferredHeight;
            }
        }
    }
}