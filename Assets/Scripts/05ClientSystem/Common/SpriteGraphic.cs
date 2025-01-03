using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

class SpriteGraphic : MaskableGraphic
{
    public SpriteAsset m_spriteAsset;

    public override Texture mainTexture
    {
        get
        {
            if (m_spriteAsset == null)
                return s_WhiteTexture;

            if (m_spriteAsset.texSource == null)
                return s_WhiteTexture;
            else
                return m_spriteAsset.texSource;
        }
    }

    public void LoadDefault()
    {
        if(null == m_spriteAsset)
        {
            m_spriteAsset = AssetLoader.instance.LoadRes("UI/Image/Emotion/emotion", typeof(SpriteAsset)).obj as SpriteAsset;
        }
    }

    List<Vector3> m_akVertexs = new List<Vector3>();
    List<Color> m_akColors = new List<Color>();
    List<Vector2> m_akUVs = new List<Vector2>();
    List<UIVertex> m_akUIVertexs = new List<UIVertex>();
    List<int> m_aiIndexs = new List<int>();
    int m_iCount;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void _Clear()
    {
        m_iCount = 0;
        m_akVertexs.Clear();
        m_akColors.Clear();
        m_akUVs.Clear();
        m_akUIVertexs.Clear();
        m_aiIndexs.Clear();
    }

    public void BeginDraw()
    {
        _Clear();

        InvokeMethod.RemoveInvokeCall(this);
        InvokeMethod.Invoke(this, 0.20f, () =>
          {
              SetVerticesDirty();
              SetLayoutDirty();
          });
    }

    public void DrawSprite(int iEmotionID,float fX, float fY, float fW, float fH, Color color)
    {
        if(m_spriteAsset == null || m_spriteAsset.listSpriteAssetInfor == null ||
           m_spriteAsset.listSpriteAssetInfor.Count <= 0)
        {
            return;
        }

        bool bFindEmotion = false;
        Rect spriteRect = m_spriteAsset.listSpriteAssetInfor[0].rect;
        for (int k = 0; k < m_spriteAsset.listSpriteAssetInfor.Count; ++k)
        {
            if (m_spriteAsset.listSpriteAssetInfor[k].ID == iEmotionID)
            {
                spriteRect = m_spriteAsset.listSpriteAssetInfor[k].rect;
                bFindEmotion = true;
                break;
            }
        }
        if(!bFindEmotion)
        {
            return;
        }

        Vector2 texSize = new Vector2(m_spriteAsset.texSource.width, m_spriteAsset.texSource.height);

        int iCurCount = m_akUIVertexs.Count;
        m_akVertexs.Clear();
        m_akVertexs.Add(new Vector3(fX, fY, 0));
        m_akVertexs.Add(new Vector3(fX + fW, fY + fH, 0));
        m_akVertexs.Add(new Vector3(fX + fW, fY, 0));
        m_akVertexs.Add(new Vector3(fX, fY + fH, 0));

        m_akUVs.Clear();
        m_akUVs.Add(new Vector2(spriteRect.x / texSize.x, spriteRect.y / texSize.y));
        m_akUVs.Add(new Vector2((spriteRect.x + spriteRect.width) / texSize.x, (spriteRect.y + spriteRect.height) / texSize.y));
        m_akUVs.Add(new Vector2((spriteRect.x + spriteRect.width) / texSize.x, spriteRect.y / texSize.y));
        m_akUVs.Add(new Vector2(spriteRect.x / texSize.x, (spriteRect.y + spriteRect.height) / texSize.y));

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
            var uiVertex = new UIVertex()
            {
                color = m_akColors[i],
                normal = Vector3.back,
                position = m_akVertexs[i],
                tangent = new Vector4(0, 1, 0, 1),
                uv0 = m_akUVs[i],
                uv1 = m_akUVs[i],
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
        if(m_akUIVertexs.Count > 0)
        {
            toFill.AddUIVertexStream(m_akUIVertexs, m_aiIndexs);
        }
    }

    protected override void OnDestroy()
    {
        m_akUVs = null;
        m_akVertexs = null;
        m_akColors = null;
        m_akUIVertexs = null;
        m_aiIndexs = null;
        m_spriteAsset = null;
        InvokeMethod.RemoveInvokeCall(this);
        base.OnDestroy();
    }
}