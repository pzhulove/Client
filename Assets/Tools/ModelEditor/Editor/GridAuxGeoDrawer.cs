using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

[CustomEditor(typeof(GridDataProxy))]
public class GridAuxGeoDrawer : Editor
{
    static protected readonly int GRID_X_MAX_LIMIT = 60;
    static protected readonly int GRID_Y_MAX_LIMIT = 60;

    int m_Width = 0;
    int m_Height = 0;

    float m_WidthMinRange = 0.0f;
    float m_WidthMaxRange = 0.0f;
    float m_HeightMinRange = 0.0f;
    float m_HeightMaxRange = 0.0f;
    int m_WidthSegMin = 0;
    int m_WidthSegMax = 0;
    int m_HeightSegMin = 0;
    int m_HeightSegMax = 0;

    int m_OffsetX = 0;
    int m_OffsetY = 0;

    Vector3 m_MinBBox = Vector3.zero;
    Vector3 m_MaxBBox = Vector3.zero;

    GridDataProxy m_GridData = null;

    Vector3 m_PickPoint = new Vector3();

    static protected readonly float GRID_SIZE = 0.25f;
    static protected readonly Vector3[] RECT = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,0, GRID_SIZE),
            new Vector3(GRID_SIZE, 0, GRID_SIZE),
            new Vector3(GRID_SIZE, 0,0),
        };
    
    private byte[] m_BlockData = null;
    protected Vector2 m_CurMousePos;
    private int m_CurMouseIdx = -1;
    

    // Use this for initialization
    void OnEnable()
    {
        GameObject go = GameObject.Find("GridNode");
        if(null != go)
        {
            Selection.activeGameObject = go;
            m_GridData = go.GetComponent<GridDataProxy>();
            m_BlockData = m_GridData.GetBlockData();
            if (null != m_BlockData)
            {
                m_Width = m_GridData.m_Width;
                m_Height = m_GridData.m_Height;

                m_MinBBox = m_GridData.m_Min;
                m_MaxBBox = m_GridData.m_Max;

                _RefreshGridSize(false);
            }
        }
    }

    public void Init()
    {
    }

    void Update()
    {

    }
    void OnDestroy()
    {
    }

    public void OnSceneGUI()
    {
        //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (null != m_GridData)
        {
            if (m_Width != m_GridData.m_Width || m_Height != m_GridData.m_Height)
            {
                _RefreshGridSize();
            }
        }

        if (null == m_BlockData)
            return;

        if(m_GridData.m_DrawBlock)
        {
            //             Event evt = Event.current;
            //             if (evt.isMouse && evt.type == EventType.MouseDown)
            //             {
            //                 if (m_WidthMinRange < m_PickPoint.x && m_PickPoint.x < m_WidthMaxRange && m_HeightMinRange < m_PickPoint.z && m_PickPoint.z < m_HeightMaxRange)
            //                 {
            //                     if (0 <= m_CurMouseIdx && m_CurMouseIdx < m_BlockData.Length)
            //                     {
            //                         if (0 == evt.button)
            //                         {
            //                             m_BlockData[m_CurMouseIdx] = 1;
            //                             m_CurMouseIdx = -1;
            //                         }
            //                         else if (1 == evt.button)
            //                         {
            //                             m_BlockData[m_CurMouseIdx] = 0;
            //                             m_CurMouseIdx = -1;
            //                         }
            //                     }
            //                 }
            //             }
            //             m_CurMousePos = evt.mousePosition;


            Event evt = Event.current;
            if (evt.type == EventType.MouseDrag && (evt.alt || evt.shift))
            {
                if (m_WidthMinRange < m_PickPoint.x && m_PickPoint.x < m_WidthMaxRange && m_HeightMinRange < m_PickPoint.z && m_PickPoint.z < m_HeightMaxRange)
                {
                    if (0 <= m_CurMouseIdx && m_CurMouseIdx < m_BlockData.Length)
                    {
                        if (0 == evt.button)
                        {
                            m_BlockData[m_CurMouseIdx] = 1;
                            m_CurMouseIdx = -1;
                        }
                        else if (1 == evt.button)
                        {
                            m_BlockData[m_CurMouseIdx] = 0;
                            m_CurMouseIdx = -1;
                        }
                    }
                }

                evt.Use();
            }
            m_CurMousePos = evt.mousePosition;


            m_PickPoint = _PickGround(SceneView.currentDrawingSceneView);

            //Debug.LogWarningFormat("X:{0}Y:{1}Z:{2}", pos.x, pos.y, pos.z);

            //_DrawMouseGrid(pos);
            _DrawGridData(m_Width, m_Height, m_BlockData, m_PickPoint);
            //Handles.EndGUI();
        }

        if(m_GridData.m_DrawBBox)
        {
            _VerifyBoundingBox();

            Color colorBack = Handles.color;
            Handles.color = Color.yellow;

            Vector3[] vert = new Vector3[8];
            vert[0] = new Vector3(m_GridData.m_Min.x,m_GridData.m_Min.y ,m_GridData.m_Min.z);
            vert[1] = new Vector3(m_GridData.m_Min.x,m_GridData.m_Max.y ,m_GridData.m_Min.z);
            vert[2] = new Vector3(m_GridData.m_Max.x,m_GridData.m_Max.y ,m_GridData.m_Min.z);
            vert[3] = new Vector3(m_GridData.m_Max.x,m_GridData.m_Min.y ,m_GridData.m_Min.z);
            vert[4] = new Vector3(m_GridData.m_Min.x, m_GridData.m_Min.y, m_GridData.m_Max.z);
            vert[5] = new Vector3(m_GridData.m_Min.x, m_GridData.m_Max.y, m_GridData.m_Max.z);
            vert[6] = new Vector3(m_GridData.m_Max.x, m_GridData.m_Max.y, m_GridData.m_Max.z);
            vert[7] = new Vector3(m_GridData.m_Max.x, m_GridData.m_Min.y, m_GridData.m_Max.z);


            Handles.DrawLine(vert[0],vert[1]);
            Handles.DrawLine(vert[1],vert[2]);
            Handles.DrawLine(vert[2],vert[3]);
            Handles.DrawLine(vert[3],vert[0]);

            Handles.DrawLine(vert[4], vert[5]);
            Handles.DrawLine(vert[5], vert[6]);
            Handles.DrawLine(vert[6], vert[7]);
            Handles.DrawLine(vert[7], vert[4]);

            Handles.DrawLine(vert[0], vert[4]);
            Handles.DrawLine(vert[1], vert[5]);
            Handles.DrawLine(vert[2], vert[6]);
            Handles.DrawLine(vert[3], vert[7]);
        }
    }

    void OnDrawGizmos()
    {
    }

    protected void _VerifyGridSize()
    {
        if(m_GridData.m_Width > GRID_X_MAX_LIMIT)
        {
            Debug.LogWarningFormat("Grid data width [{0}] is larger than limit [{1}]!", m_GridData.m_Width, GRID_X_MAX_LIMIT);
            m_GridData.m_Width = GRID_X_MAX_LIMIT;
        }
        if (m_GridData.m_Width < 1)
        {
            Debug.LogWarningFormat("Grid data width [{0}] must be positive value!", m_GridData.m_Width);
            m_GridData.m_Width = 1;
        }

        if (m_GridData.m_Height > GRID_Y_MAX_LIMIT)
        {
            Debug.LogWarningFormat("Grid data height [{0}]is larger than limit [{1}]!", m_GridData.m_Height, GRID_Y_MAX_LIMIT);
            m_GridData.m_Height = GRID_Y_MAX_LIMIT;
        }

        if (m_GridData.m_Height < 1)
        {
            Debug.LogWarningFormat("Grid data height [{0}] must be positive value!", m_GridData.m_Height);
            m_GridData.m_Height = 1;
        }
    }

    protected void _VerifyBoundingBox()
    {
        if (m_GridData.m_Max.x < m_MinBBox.x)
            m_GridData.m_Max.x = m_MinBBox.x;
        if (m_GridData.m_Max.y < m_MinBBox.y)
            m_GridData.m_Max.y = m_MinBBox.y;
        if (m_GridData.m_Max.z < m_MinBBox.z)
            m_GridData.m_Max.z = m_MinBBox.z;

        if (m_GridData.m_Min.x > m_MaxBBox.x)
            m_GridData.m_Min.x = m_MaxBBox.x;
        if (m_GridData.m_Min.y > m_MaxBBox.y)
            m_GridData.m_Min.y = m_MaxBBox.y;
        if (m_GridData.m_Min.z > m_MaxBBox.z)
            m_GridData.m_Min.z = m_MaxBBox.z;

        m_MinBBox.x = m_GridData.m_Min.x;
        m_MaxBBox.x = m_GridData.m_Max.x;
        m_MinBBox.y = m_GridData.m_Min.y;
        m_MaxBBox.y = m_GridData.m_Max.y;
        m_MinBBox.z = m_GridData.m_Min.z;
        m_MaxBBox.z = m_GridData.m_Max.z;
    }

    protected void _RefreshGridSize(bool allocMem = true)
    {
        _VerifyGridSize();
        if(allocMem)
        {
            m_GridData.AllocBlockData();
            m_BlockData = m_GridData.GetBlockData();
        }

        m_Width = m_GridData.m_Width;
        m_Height = m_GridData.m_Height;

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
    }

    protected void _DrawMouseGrid(Vector3 mousePos)
    {
        Vector3 mouseGridPos = mousePos;
        mouseGridPos.y = 0;

        Color Blank = Color.green;
        Blank.a = 0.25f;

        Vector3[] gridRect = new Vector3[4];

        gridRect[0].x = RECT[0].x + mouseGridPos.x - GRID_SIZE/2;
        gridRect[0].y = 0;
        gridRect[0].z = RECT[0].z + mouseGridPos.z - GRID_SIZE / 2;

        gridRect[1].x = RECT[1].x + mouseGridPos.x - GRID_SIZE / 2;
        gridRect[1].y = 0;
        gridRect[1].z = RECT[1].z + mouseGridPos.z - GRID_SIZE / 2;

        gridRect[2].x = RECT[2].x + mouseGridPos.x - GRID_SIZE / 2;
        gridRect[2].y = 0;
        gridRect[2].z = RECT[2].z + mouseGridPos.z - GRID_SIZE / 2;

        gridRect[3].x = RECT[3].x + mouseGridPos.x - GRID_SIZE / 2;
        gridRect[3].y = 0;
        gridRect[3].z = RECT[3].z + mouseGridPos.z - GRID_SIZE / 2;

        Handles.DrawSolidRectangleWithOutline(gridRect, Blank, Color.grey);
    }

    protected Vector3 _PickGround(SceneView sceneview)
    {
        Ray pickRay = sceneview.camera.ScreenPointToRay(new Vector3(m_CurMousePos.x, sceneview.camera.pixelHeight - m_CurMousePos.y,0.0f));
        Plane groundPlane = new Plane(new Vector3(0, 1, 0), 0);
        float rayDistance;
        if (groundPlane.Raycast(pickRay, out rayDistance))
        {
            Vector3 o = pickRay.GetPoint(rayDistance);
            //Handles.color = Color.cyan;
            Handles.DrawLine(pickRay.origin, o);
            return o;
        }
        else
            return Vector3.zero;
    }

    protected void _DrawGridData(int x, int y, byte[] blockData, Vector3 mousePos)
    {
        if (null == blockData)
            return;

        Color Blank = Color.blue;
        Blank.a = 0.25f;
        Color Block = Color.red;
        Block.a = 0.25f;
        Color Mouse = Color.green;
        Mouse.a = 0.35f;

        Vector3[] gridRect = new Vector3[4];
        bool mouse = false;
        for (int j = m_HeightSegMin; j < m_HeightSegMax; ++j)
        {
            for (int i = m_WidthSegMin; i < m_WidthSegMax; ++i)
            {
                mouse = false;
                gridRect[0].x = RECT[0].x + i * GRID_SIZE;
                gridRect[0].y = 0;
                gridRect[0].z = RECT[0].z + j * GRID_SIZE;

                gridRect[1].x = RECT[1].x + i * GRID_SIZE;
                gridRect[1].y = 0;
                gridRect[1].z = RECT[1].z + j * GRID_SIZE;

                gridRect[2].x = RECT[2].x + i * GRID_SIZE;
                gridRect[2].y = 0;
                gridRect[2].z = RECT[2].z + j * GRID_SIZE;

                gridRect[3].x = RECT[3].x + i * GRID_SIZE;
                gridRect[3].y = 0;
                gridRect[3].z = RECT[3].z + j * GRID_SIZE;

                if (i * GRID_SIZE < mousePos.x && mousePos.x < (i + 1) * GRID_SIZE && j * GRID_SIZE < mousePos.z && mousePos.z < (j + 1) * GRID_SIZE)
                    mouse = true;

                int idx = (i + m_WidthSegMax - m_OffsetX) + m_Width * (j + m_HeightSegMax - m_OffsetY);
                if (idx < blockData.Length)
                {
                    if (0 == blockData[idx])
                    {
                        if (mouse)
                        {
                            m_CurMouseIdx = idx;
                            Handles.DrawSolidRectangleWithOutline(gridRect, Blank + Mouse, Color.grey);
                        }
                        else
                            Handles.DrawSolidRectangleWithOutline(gridRect, Blank, Color.grey);
                    }
                    else
                    {
                        if (mouse)
                        {
                            m_CurMouseIdx = idx;
                            Handles.DrawSolidRectangleWithOutline(gridRect, Block + Mouse, Color.grey);
                        }
                        else
                            Handles.DrawSolidRectangleWithOutline(gridRect, Block, Color.grey);
                    }

                }
                else
                    Logger.LogWarning("Index out of block data range!");
                
            }
        }
    }
}

