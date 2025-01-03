using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

class UnderLineGraphic : MaskableGraphic
{
    Vector2[] m_akUVTemps = new Vector2[4]
    {
        new Vector2(0,0),
        new Vector2(1,1),
        new Vector2(1,0),
        new Vector2(0,1),
    };

    List<Vector3> m_akVertexs = new List<Vector3>();
    List<Color> m_akColors = new List<Color>();
    List<UIVertex> m_akUIVertexs = new List<UIVertex>();
    List<int> m_aiIndexs = new List<int>();
    int m_iCount;

    protected override void OnEnable()
    {
        _Clear();
        base.OnEnable();
    }

    void _Clear()
    {
        m_iCount = 0;
        m_akVertexs.Clear();
        m_akColors.Clear();
        m_akUIVertexs.Clear();
        m_aiIndexs.Clear();
    }

    public void BeginDraw()
    {
        _Clear();
    }

    public void DrawRect(float fX, float fY, float fW, float fH, Color color)
    {
        m_akVertexs.Clear();
        m_akVertexs.Add(new Vector3(fX, fY, 0));
        m_akVertexs.Add(new Vector3(fX + fW, fY + fH, 0));
        m_akVertexs.Add(new Vector3(fX + fW, fY, 0));
        m_akVertexs.Add(new Vector3(fX, fY + fH, 0));

        m_akColors.Clear();
        m_akColors.Add(color);
        m_akColors.Add(color);
        m_akColors.Add(color);
        m_akColors.Add(color);

        m_aiIndexs.Add(m_akUIVertexs.Count + 0);
        m_aiIndexs.Add(m_akUIVertexs.Count + 1);
        m_aiIndexs.Add(m_akUIVertexs.Count + 2);
        m_aiIndexs.Add(m_akUIVertexs.Count + 1);
        m_aiIndexs.Add(m_akUIVertexs.Count + 0);
        m_aiIndexs.Add(m_akUIVertexs.Count + 3);

        for (int i = 0; i < 4; ++i)
        {
            UIVertex uiVertex = new UIVertex()
            {
                color = m_akColors[i],
                normal = Vector3.back,
                position = m_akVertexs[i],
                tangent = new Vector4(0, 1, 0, 1),
                uv0 = m_akUVTemps[i],
                uv1 = m_akUVTemps[i],
            };
            m_akUIVertexs.Add(uiVertex);
        }
    }

    public void EndDraw()
    {

    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
        if (m_akUIVertexs.Count > 0)
        {
            toFill.AddUIVertexStream(m_akUIVertexs,m_aiIndexs);
        }
    }

    protected override void OnDestroy()
    {
        m_akUVTemps = null;
        m_akVertexs = null;
        m_akColors = null;
        m_akUIVertexs = null;
        m_aiIndexs = null;
        base.OnDestroy();
    }
}