using UnityEngine;
using System.Collections;

public class GridDataProxy : MonoBehaviour
{
    public int m_Width = 4;
    public int m_Height = 4;

    public Vector3 m_Min = Vector3.zero;
    public Vector3 m_Max = Vector3.zero;

    public bool m_DrawBlock = false;
    public bool m_DrawBBox = false;

    private int m_OldWidth = 0;
    private int m_OldHeight = 0;

    private byte[] m_BlockData = null;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void AllocBlockData()
    {
        int unRectLen = m_Width * m_Height;
        if (unRectLen > 0)
        {
            byte[] oldData = m_BlockData;
            m_BlockData = new byte[unRectLen];

            int MaxWidth = m_Width > m_OldWidth ? m_Width : m_OldWidth;
            int MaxHeight = m_Height > m_OldHeight ? m_Height : m_OldHeight;

            int XOld = (m_OldWidth & 0x01) != 0 ? 0 : 1;
            int XNew = (m_Width & 0x01) != 0 ? 0 : 1;
            int YOld = (m_OldHeight & 0x01) != 0 ? 0 : 1;
            int YNew = (m_Height & 0x01) != 0 ? 0 : 1;

            int nOffsetX = (m_OldWidth + XOld) / 2 - (m_Width + XNew) / 2;
            int nOffsetY = (m_OldHeight + YOld) / 2 - (m_Height + YNew) / 2;

            int nResetX = (m_Width + XNew) / 2 - (m_OldWidth + XOld) / 2;
            int nResetY = (m_Height + YNew) / 2 - (m_OldHeight + YOld) / 2;

            int newIdx = 0;
            int oldIdx = 0;
            for (int y = 0; y < m_Height; ++y)
            {
                for (int x = 0; x < m_Width; ++x)
                {
                    newIdx = x + y * m_Width;
                    if (x < nResetX || y < nResetY)
                        m_BlockData[newIdx] = 0;
                    else
                    {
                        if(x < m_OldWidth + nResetX  && y < m_OldHeight + nResetY)
                        {
                            oldIdx = (x + nOffsetX) + ((y + nOffsetY) * m_OldWidth);
                            m_BlockData[newIdx] = oldData[oldIdx];
                        }
                        else
                            m_BlockData[newIdx] = 0;
                    }
                }
            }

            m_OldWidth = m_Width;
            m_OldHeight = m_Height;
        }
        else
            Debug.LogWarning("Block size must be positive value!");
    }

    public byte[] GetBlockData()
    {
        return m_BlockData;
    }
}
