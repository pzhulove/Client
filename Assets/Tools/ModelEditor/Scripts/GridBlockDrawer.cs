using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[ExecuteInEditMode]
public class GridBlockDrawer : MonoBehaviour
{
    static protected readonly float GRID_SIZE = 0.25f;
    static protected readonly Vector3[] RECT = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,0, GRID_SIZE),
            new Vector3(GRID_SIZE, 0, GRID_SIZE),
            new Vector3(GRID_SIZE, 0,0),
        };

    private int m_Width = 0;
    private int m_Height = 0;
    private byte[] m_BlockData = null;

    private float m_WidthMinRange = 0.0f;
    private float m_WidthMaxRange = 0.0f;
    private float m_HeightMinRange = 0.0f;
    private float m_HeightMaxRange = 0.0f;

    private int m_WidthSegMin = 0;
    private int m_WidthSegMax = 0;
    private int m_HeightSegMin = 0;
    private int m_HeightSegMax = 0;

    private int m_OffsetX = 0;
    private int m_OffsetY = 0;

    public bool bShow = true;
    void OnDrawGizmos()
    {
        if(bShow == false)
        {
            return;
        }
        
        _DrawGrid();
    }

    public void RefreshGridData(int width,int height,byte[] blockData)
    {
        int gridDataLen = width * height;
        if (gridDataLen > 0 )
        {
            m_Width = width;
            m_Height = height;
            m_WidthSegMin = -m_Width / 2;
            m_WidthSegMax = (m_Width + 1) / 2;
            m_HeightSegMin = -m_Height / 2;
            m_HeightSegMax = (m_Height + 1) / 2;

            m_OffsetX = m_WidthSegMax != m_Width / 2 ? 1 : 0;
            m_OffsetY = m_HeightSegMax != m_Height / 2 ? 1 : 0;

            m_WidthMinRange = m_WidthSegMin * GRID_SIZE;
            m_WidthMaxRange = m_WidthSegMax * GRID_SIZE;
            m_HeightMinRange = m_HeightSegMin * GRID_SIZE;
            m_HeightMaxRange = m_HeightSegMax * GRID_SIZE;

            m_BlockData = new byte[gridDataLen];
            for (int i = 0; i < gridDataLen; ++i)
                m_BlockData[i] = blockData[i];
        }
        else
            Debug.LogFormat("Warning grid width:{0} or height{1} is invalid value!", width, height);
    }

    protected void _DrawGrid()
    {
        if (null == m_BlockData)
            return;

        Color Block = Color.red;
        Block.a = 0.25f;

        Vector3[] gridRect = new Vector3[4];

        Vector3 objPos = gameObject.transform.position;
        for (int j = m_HeightSegMin; j < m_HeightSegMax; ++j)
        {
            for (int i = m_WidthSegMin; i < m_WidthSegMax; ++i)
            {
                gridRect[0].x = objPos.x + RECT[0].x + i * GRID_SIZE;
                gridRect[0].y = 0;
                gridRect[0].z = objPos.z + RECT[0].z + j * GRID_SIZE;

                gridRect[1].x = objPos.x + RECT[1].x + i * GRID_SIZE;
                gridRect[1].y = 0;
                gridRect[1].z = objPos.z + RECT[1].z + j * GRID_SIZE;

                gridRect[2].x = objPos.x + RECT[2].x + i * GRID_SIZE;
                gridRect[2].y = 0;
                gridRect[2].z = objPos.z + RECT[2].z + j * GRID_SIZE;

                gridRect[3].x = objPos.x + RECT[3].x + i * GRID_SIZE;
                gridRect[3].y = 0;
                gridRect[3].z = objPos.z + RECT[3].z + j * GRID_SIZE;

                int idx = (i + m_WidthSegMax - m_OffsetX) + m_Width * (j + m_HeightSegMax - m_OffsetY);
                if (idx < m_BlockData.Length)
                {
                    if (1 == m_BlockData[idx])
                        Handles.DrawSolidRectangleWithOutline(gridRect, Block, Color.grey);
                }
                else
                    Logger.LogWarning("Index out of block data range!");
            }
        }
    }
}
#endif
