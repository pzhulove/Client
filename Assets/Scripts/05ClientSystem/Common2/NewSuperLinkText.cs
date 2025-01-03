using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Reflection;
using ProtoTable;

namespace GameClient
{
    public class NewSuperLinkText : Text, IPointerClickHandler, IPointerDownHandler
    {
        #region declare
        static byte[] ms_abVertexBytePool = new byte[4096];
        static Color[] ms_akColors = new Color[(int)SpriteAssetColor.SAC_COUNT]
        {
        Color.white,//SAC_WHITE
        new Color(0.0f,0.75f,1.0f),//SAC_BLUE
        new Color(0.75f,0.0f,1.0f),//SAC_PURPLE
        new Color(1.0f,0.0f,0.75f),//SAC_PINK
        new Color(1.0f,0.75f,0.0f),//SAC_ORANGE
        new Color(0.0f,1.0f,0.0f),//SAC_GREEN
        new Color(1.0f,0.8157f,0.2588f),//SAC_OTHER_NAME
        new Color(0.0f,0.78f,1.0f),//SAC_SELF_NAME
        new Color(1.0f,0.0f,0.0f),//SAC_WRAP_STONE_RED
        new Color(0.0f,1.0f,0.2824f),//SAC_WRAP_STONE_GREEN
        new Color(1.0f,0.9647f,0.0f),//SAC_WRAP_STONE_YELLOW
        new Color(0.0f,0.6353f,1.0f),//SAC_WRAP_STONE_BLUE
        new Color(0.898f,0.4353f,0.9137f),//SAC_WRAP_STONE_BLACK
        new Color(1.0f, 0.0f, 0.35f),     //SAC_PINK_RED,
        };

        public static string GetColorString(SpriteAssetColor eSpriteAssetColor)
        {
            var color = Color.white;
            if (eSpriteAssetColor >= 0 && (int)eSpriteAssetColor < ms_akColors.Length)
            {
                color = ms_akColors[(int)eSpriteAssetColor];
            }
            byte vr = (byte)(255.0f * color.r);
            byte vg = (byte)(255.0f * color.g);
            byte vb = (byte)(255.0f * color.b);
            byte va = (byte)(255.0f * color.a);
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", vr, vg, vb, va);
        }

        public static string[] ms_stringColors = new string[(int)SpriteAssetColor.SAC_COUNT]
        {
            "FFFFFFFF",
            "00BFFFFF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
            "770887FF",
        };

        public float fMaxWidth = -1.0f;
        public override float preferredWidth
        {
            get
            {
                if (fMaxWidth < 0.0f)
                {
                    return base.preferredWidth;
                }
                if (fMaxWidth < base.preferredWidth)
                {
                    return fMaxWidth;
                }
                return base.preferredWidth;
            }
        }

        List<LinkParse.LinkInfo> m_akLinkInfos = new List<LinkParse.LinkInfo>();
        SpriteAsset m_spriteAsset;
        SpriteGraphic m_spriteGraphic;
        UnderLineGraphic m_underLineGraphic;
        UIVertex m_kVertex;
        List<UIVertex> m_akTempVertex = new List<UIVertex>();
        List<int> m_akIndexs = new List<int>();

        public delegate void OnClickItem(ulong guid, int dataid, int strengthLv);
        public OnClickItem onClickItem;

        public delegate void OnClickPlayerName(ulong guid, string name, byte occu, uint level);
        public OnClickPlayerName onClickPlayerName;

        public delegate void OnClickLocalLink(int iParam0, string name);
        public OnClickLocalLink onClickLocalLink;

        public delegate void OnClickRetinueLink(ulong guid, int dataid);
        public OnClickRetinueLink onClickRetinueLink;

        public delegate void OnClickWrapStoneLink(int iParam0);
        public OnClickWrapStoneLink onClickWrapStoneLink;

        private readonly List<Image> m_ImagesPool = new List<Image>();
        private int m_iValidCount = 0;

        class SpriteRenderInfo
        {
            public SpriteAniRender spriteRender;
            public Image img;
        }
        private readonly List<SpriteRenderInfo> m_spriteRenders = new List<SpriteRenderInfo>();
        private int m_iRenderCount = 0;

        void _EnableImages(int iValidCount)
        {
            for (int i = 0; i < m_ImagesPool.Count; ++i)
            {
                m_ImagesPool[i].enabled = i < iValidCount;
                if (!m_ImagesPool[i].enabled)
                {
                    m_ImagesPool[i].sprite = null;
                    // added by litao
                    m_ImagesPool[i].material = null;
                }
            }
        }

        void _EnableSpriteRenders(int iValidCount)
        {
            for (int i = 0; i < m_spriteRenders.Count; ++i)
            {
                m_spriteRenders[i].img.enabled = i < iValidCount;
                if (!m_spriteRenders[i].img.enabled)
                {
                    m_spriteRenders[i].img.sprite = null;
                    m_spriteRenders[i].img.material = null;
                }
            }
        }
        #endregion

        #region override
        protected override void OnDestroy()
        {
            m_akLinkInfos.Clear();
            m_ImagesPool.Clear();
            m_spriteAsset = null;
            m_spriteGraphic = null;
            m_underLineGraphic = null;
            //m_kVertex;
            m_akTempVertex.Clear();
            m_akIndexs.Clear();

            RemoveAllDelegate();

            base.OnDestroy();
        }

        protected override void Awake()
        {
            base.Awake();
            m_spriteGraphic = Utility.FindComponent<SpriteGraphic>(gameObject, "SpriteGraphic");
            m_spriteGraphic.LoadDefault();
            m_underLineGraphic = Utility.FindComponent<UnderLineGraphic>(gameObject, "UnderLineGraphic");
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (m_akLinkInfos.Count <= 0)
            {
                base.OnPopulateMesh(toFill);
                return;
            }
            //_CreateBounds(toFill);
            //_AddEffectiveTextVertexs(toFill);
            OnPopulateNewMesh(toFill);
            _SetEmotionVertexDirty();
            _SetUnderLineVertexsDirty();
            _DrawDynamicImages();
            _DrawSpriteRenders();

            //_DrawEffectiveTextVertex(toFill);
        }

        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected void OnPopulateNewMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(text, settings);

            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(settings.textAnchor);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
            refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line...
            int vertCount = verts.Count - 4;

            //Create Bounds
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                curLink.bNeedRemove = true;
                curLink.allBounds.Clear();

                int iStart = curLink.iStartIndex * 4;
                int iEnd = curLink.iEndIndex * 4;
                if (curLink == null || iEnd > vertCount)
                {
                    continue;
                }

                curLink.bNeedRemove = false;
                var bounds = new Bounds();
                var lastPosition = Vector3.zero;
                for (int j = iStart; j < iEnd; ++j)
                {
                    if (j < 0 || j >= vertCount)
                    {
                        Logger.LogErrorFormat("out of index j = {0} toFill.currentVertCount={1}", j, vertCount);
                        curLink.bNeedRemove = true;
                        break;
                    }

                    if (roundingOffset != Vector2.zero)
                    {
                        m_kVertex = verts[j];
                        m_kVertex.position *= unitsPerPixel;
                        m_kVertex.position.x += roundingOffset.x;
                        m_kVertex.position.y += roundingOffset.y;
                    }
                    else
                    {
                        m_kVertex = verts[j];
                        m_kVertex.position *= unitsPerPixel;
                    }

                    //toFill.PopulateUIVertex(ref m_kVertex, j);

                    if (j == iStart)
                    {
                        bounds = new Bounds(m_kVertex.position, Vector3.zero);
                    }
                    else if (j % 4 == 0 && lastPosition.x > m_kVertex.position.x)
                    {
                        curLink.allBounds.Add(bounds);
                        bounds = new Bounds(m_kVertex.position, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(m_kVertex.position);
                    }
                    lastPosition = m_kVertex.position;

                    if (curLink.eRegexType == LinkParse.RegexType.RT_EMOTION ||
                        curLink.eRegexType == LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                    {
                        m_kVertex.position = Vector3.zero;
                    }
                }
                if (bounds.size.x > 0 && bounds.size.y > 0)
                {
                    curLink.allBounds.Add(bounds);
                }
                if (curLink.allBounds.Count > 0)
                {
                    curLink.bounds = curLink.allBounds[0];
                }
            }

            m_akLinkInfos.RemoveAll(x =>
            {
                return x.bNeedRemove;
            });

            toFill.Clear();

            if (vertCount >= ms_abVertexBytePool.Length)
            {
                Logger.LogErrorFormat("the max flags pool size is {0}", ms_abVertexBytePool.Length);
                m_DisableFontTextureRebuiltCallback = false;
                return;
            }

            for (int i = 0; i < ms_abVertexBytePool.Length && i < vertCount; ++i)
            {
                ms_abVertexBytePool[i] = 1;
            }

            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                int iStart = m_akLinkInfos[i].iStartIndex * 4;
                int iEnd = m_akLinkInfos[i].iEndIndex * 4;
                if (iStart < 0 || iEnd > vertCount)
                {
                    continue;
                }

                if (m_akLinkInfos[i].bNeedRemove)
                {
                    continue;
                }

                if (m_akLinkInfos[i].eRegexType != LinkParse.RegexType.RT_EMOTION &&
                    m_akLinkInfos[i].eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                for (int j = iStart; j < iEnd; ++j)
                {
                    ms_abVertexBytePool[j] = 0;
                }
            }


            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;

                    if(ms_abVertexBytePool[i] == 0)
                        continue;

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;

                    if(ms_abVertexBytePool[i] == 0)
                        continue;

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            m_DisableFontTextureRebuiltCallback = false;

        }
        UnityEngine.Events.UnityAction onFailed;
        public void AddFailedCallBack(UnityEngine.Events.UnityAction callback)
        {
            onFailed += callback;
        }
        public void RemoveFailedCallBack(UnityEngine.Events.UnityAction callback)
        {
            onFailed -= callback;
        }
        #endregion

        #region private
        void _CreateBounds(VertexHelper toFill)
        {
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                curLink.bNeedRemove = true;
                curLink.allBounds.Clear();

                int iStart = curLink.iStartIndex * 4;
                int iEnd = curLink.iEndIndex * 4;
                if (curLink == null || iEnd > toFill.currentVertCount)
                {
                    continue;
                }

                curLink.bNeedRemove = false;
                var bounds = new Bounds();
                var lastPosition = Vector3.zero;
                for (int j = iStart; j < iEnd; ++j)
                {
                    if (j < 0 || j >= toFill.currentVertCount)
                    {
                        Logger.LogErrorFormat("out of index j = {0} toFill.currentVertCount={1}", j, toFill.currentVertCount);
                        curLink.bNeedRemove = true;
                        break;
                    }
                    toFill.PopulateUIVertex(ref m_kVertex, j);

                    if (j == iStart)
                    {
                        bounds = new Bounds(m_kVertex.position, Vector3.zero);
                    }
                    else if (j % 4 == 0 && lastPosition.x > m_kVertex.position.x)
                    {
                        curLink.allBounds.Add(bounds);
                        bounds = new Bounds(m_kVertex.position, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(m_kVertex.position);
                    }
                    lastPosition = m_kVertex.position;

                    if (curLink.eRegexType == LinkParse.RegexType.RT_EMOTION ||
                        curLink.eRegexType == LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                    {
                        m_kVertex.position = Vector3.zero;
                    }
                }
                if (bounds.size.x > 0 && bounds.size.y > 0)
                {
                    curLink.allBounds.Add(bounds);
                }
                if (curLink.allBounds.Count > 0)
                {
                    curLink.bounds = curLink.allBounds[0];
                }
            }

            m_akLinkInfos.RemoveAll(x =>
            {
                return x.bNeedRemove;
            });
        }

        void _SetEmotionVertexDirty()
        {
            if (m_spriteGraphic != null)
            {
                m_spriteGraphic.BeginDraw();

                for (int i = 0; i < m_akLinkInfos.Count; ++i)
                {
                    var curLink = m_akLinkInfos[i];
                    if (curLink == null || curLink.eRegexType != LinkParse.RegexType.RT_EMOTION)
                    {
                        continue;
                    }

                    if (curLink.allBounds.Count > 0)
                    {
                        /*
                        float fX = curLink.bounds.center.x - curLink.bounds.size.x / 2;
                        float fY = curLink.bounds.center.y - curLink.bounds.size.x / 2;
                        */
                        float fX = curLink.bounds.center.x - curLink.bounds.size.x / 2;
                        float fY = curLink.bounds.center.y - curLink.bounds.size.y / 2 + fontSize / 2 - curLink.bounds.size.y / 2;
                        float fW = curLink.bounds.size.x;
                        float fH = curLink.bounds.size.y;
                        m_spriteGraphic.DrawSprite(curLink.iParam0, fX, fY, fW, fH, Color.white);
                    }
                }
                m_spriteGraphic.EndDraw();
            }
        }

        void _SetUnderLineVertexsDirty()
        {
            if (m_underLineGraphic != null)
            {
                m_underLineGraphic.BeginDraw();
                for (int i = 0; i < m_akLinkInfos.Count; ++i)
                {
                    var curLink = m_akLinkInfos[i];
                    if (curLink == null || curLink.eColor == SpriteAssetColor.SAC_COUNT || curLink.eRegexType == LinkParse.RegexType.RT_DYNAMIC_IMAGE ||
                        curLink.bNeedRemove)
                    {
                        continue;
                    }

                    if (curLink.eRegexType == LinkParse.RegexType.RT_SUPER_GROUP)
                    {

                    }

                    for (int j = 0; j < curLink.allBounds.Count; ++j)
                    {
                        float fX = curLink.allBounds[j].center.x - curLink.allBounds[j].size.x / 2;
                        float fY = curLink.allBounds[j].center.y - curLink.allBounds[j].size.y / 2;
                        float fW = curLink.allBounds[j].size.x;
                        float fH = 2;

                        m_underLineGraphic.DrawRect(fX, fY, fW, fH, ms_akColors[(int)curLink.eColor]);
                    }
                }
                m_underLineGraphic.EndDraw();
            }
        }

        void _SetDynamicImages()
        {
            m_iValidCount = 0;
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                if (curLink == null || curLink.eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                var imgItem = TableManager.GetInstance().GetTableItem<ProtoTable.LiaoTianDynamicTextureTable>(curLink.iParam0);
                if (null == imgItem || imgItem.Type != ProtoTable.LiaoTianDynamicTextureTable.eType.Image)
                {
                    continue;
                }

                ++m_iValidCount;

                Image img = null;
                if (m_ImagesPool.Count < m_iValidCount)
                {
                    GameObject imgRoot = new GameObject("Img", typeof(Image));
                    img = imgRoot.GetComponent<Image>();
                    Utility.AttachTo(imgRoot, gameObject);
                    img.CustomActive(true);
                    m_ImagesPool.Add(img);
                }
                else
                {
                    img = m_ImagesPool[m_iValidCount - 1];
                }

                img.rectTransform.anchorMin = rectTransform.anchorMin;
                img.rectTransform.anchorMax = rectTransform.anchorMax;
                img.rectTransform.pivot = Vector2.zero;
                img.rectTransform.sizeDelta = new Vector2((imgItem.Size - 2.0f) * 1.0f / imgItem.Height * imgItem.Width, (imgItem.Size - 2.0f));
                // img.sprite = AssetLoader.instance.LoadRes(imgItem.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref img, imgItem.Icon);
            }

            _EnableImages(m_iValidCount);
        }

        void _SetDynamicFrameSprites()
        {
            m_iRenderCount = 0;
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                if (curLink == null || curLink.eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                var imgItem = TableManager.GetInstance().GetTableItem<ProtoTable.LiaoTianDynamicTextureTable>(curLink.iParam0);
                if (null == imgItem || imgItem.Type != ProtoTable.LiaoTianDynamicTextureTable.eType.FrameSprite)
                {
                    continue;
                }

                ++m_iRenderCount;

                SpriteRenderInfo spriteRenderInfo = null;
                if (m_spriteRenders.Count < m_iRenderCount)
                {
                    GameObject spriteRoot = new GameObject("Sprite", typeof(Image), typeof(SpriteAniRender));
                    spriteRenderInfo = new SpriteRenderInfo
                    {
                        spriteRender = spriteRoot.GetComponent<SpriteAniRender>(),
                        img = spriteRoot.GetComponent<Image>()
                    };

                    Utility.AttachTo(spriteRoot, gameObject);
                    spriteRenderInfo.img.CustomActive(true);
                    m_spriteRenders.Add(spriteRenderInfo);
                }
                else
                {
                    spriteRenderInfo = m_spriteRenders[m_iRenderCount - 1];
                }

                spriteRenderInfo.img.rectTransform.anchorMin = rectTransform.anchorMin;
                spriteRenderInfo.img.rectTransform.anchorMax = rectTransform.anchorMax;
                spriteRenderInfo.img.rectTransform.pivot = Vector2.zero;
                spriteRenderInfo.img.rectTransform.sizeDelta = new Vector2((imgItem.Size - 2.0f) * 1.0f / imgItem.Height * imgItem.Width, imgItem.Size - 2.0f);
                //spriteRenderInfo.img.sprite = AssetLoader.instance.LoadRes(imgItem.Icon, typeof(Sprite)).obj as Sprite;
                spriteRenderInfo.spriteRender.orgPath = imgItem.Param[0];
                spriteRenderInfo.spriteRender.orgName = imgItem.Param[1];
                spriteRenderInfo.spriteRender.count = int.Parse(imgItem.Param[2]);
                spriteRenderInfo.spriteRender.looptimes = 0;
                spriteRenderInfo.spriteRender.playinterval = int.Parse(imgItem.Param[3]);
                spriteRenderInfo.spriteRender.Reset(imgItem.Param[0], imgItem.Param[1], int.Parse(imgItem.Param[2]), (imgItem.Size - 2.0f) * 1.0f / imgItem.Height);
            }

            _EnableSpriteRenders(m_iRenderCount);
        }

        void _DrawDynamicImages()
        {
            m_iValidCount = 0;
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                if (curLink == null || curLink.eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                var imgItem = TableManager.GetInstance().GetTableItem<ProtoTable.LiaoTianDynamicTextureTable>(curLink.iParam0);
                if (null == imgItem || imgItem.Type != ProtoTable.LiaoTianDynamicTextureTable.eType.Image)
                {
                    continue;
                }

                ++m_iValidCount;

                var bounds = curLink.allBounds[0];
                float fX = bounds.center.x - m_ImagesPool[m_iValidCount - 1].rectTransform.sizeDelta.x / 2;
                float fY = bounds.center.y - m_ImagesPool[m_iValidCount - 1].rectTransform.sizeDelta.y / 2;
                float fW = bounds.size.x;
                float fH = bounds.size.y;

                if (m_iValidCount <= m_ImagesPool.Count)
                {
                    m_ImagesPool[m_iValidCount - 1].rectTransform.anchoredPosition = new Vector2(fX, fY);
                }
            }
        }

        void _DrawSpriteRenders()
        {
            m_iRenderCount = 0;
            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var curLink = m_akLinkInfos[i];
                if (curLink == null || curLink.eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                var imgItem = TableManager.GetInstance().GetTableItem<ProtoTable.LiaoTianDynamicTextureTable>(curLink.iParam0);
                if (null == imgItem || imgItem.Type != ProtoTable.LiaoTianDynamicTextureTable.eType.FrameSprite)
                {
                    continue;
                }

                ++m_iRenderCount;

                var bounds = curLink.allBounds[0];
                float fX = bounds.center.x - m_spriteRenders[m_iRenderCount - 1].img.rectTransform.sizeDelta.x / 2;
                float fY = bounds.center.y - m_spriteRenders[m_iRenderCount - 1].img.rectTransform.sizeDelta.y / 2;
                float fW = bounds.size.x;
                float fH = bounds.size.y;

                if (m_iRenderCount <= m_spriteRenders.Count)
                {
                    m_spriteRenders[m_iRenderCount - 1].img.rectTransform.anchoredPosition = new Vector2(fX, fY);
                }
            }
        }

        void _AddEffectiveTextVertexs(VertexHelper toFill)
        {
            m_akIndexs.Clear();
            m_akTempVertex.Clear();
            if (toFill.currentVertCount <= 0)
            {
                return;
            }

            if (toFill.currentIndexCount >= ms_abVertexBytePool.Length)
            {
                Logger.LogErrorFormat("the max flags pool size is {0}", ms_abVertexBytePool.Length);
                return;
            }

            for (int i = 0; i < ms_abVertexBytePool.Length && i < toFill.currentIndexCount; ++i)
            {
                ms_abVertexBytePool[i] = 1;
            }

            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                int iStart = m_akLinkInfos[i].iStartIndex * 4;
                int iEnd = m_akLinkInfos[i].iEndIndex * 4;
                if (iStart < 0 || iEnd > toFill.currentVertCount)
                {
                    Logger.LogErrorFormat("out of index!");
                    continue;
                }

                if (m_akLinkInfos[i].bNeedRemove)
                {
                    continue;
                }

                if (m_akLinkInfos[i].eRegexType != LinkParse.RegexType.RT_EMOTION &&
                    m_akLinkInfos[i].eRegexType != LinkParse.RegexType.RT_DYNAMIC_IMAGE)
                {
                    continue;
                }

                for (int j = iStart; j < iEnd; ++j)
                {
                    ms_abVertexBytePool[j] = 0;
                }
            }

            for (int j = 0; j < toFill.currentVertCount; ++j)
            {
                if (ms_abVertexBytePool[j] == 1)
                {
                    toFill.PopulateUIVertex(ref m_kVertex, j);
                    m_akTempVertex.Add(m_kVertex);
                }
            }

            m_akIndexs.Clear();
            for (int k = 0; k < m_akTempVertex.Count; ++k)
            {
                if (k % 4 == 0)
                {
                    m_akIndexs.Add(k + 0);
                    m_akIndexs.Add(k + 1);
                    m_akIndexs.Add(k + 2);

                    m_akIndexs.Add(k + 2);
                    m_akIndexs.Add(k + 0);
                    m_akIndexs.Add(k + 3);
                }
            }
        }

        void _DrawEffectiveTextVertex(VertexHelper toFill)
        {
            if (toFill != null)
            {
                toFill.Clear();
                toFill.AddUIVertexStream(m_akTempVertex, m_akIndexs);
            }
        }
        #endregion

        #region pubic

        public override string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (System.String.IsNullOrEmpty(value))
                {
                    if (System.String.IsNullOrEmpty(m_Text))
                        return;
                    m_Text = "";
                    SetVerticesDirty();
                }
                else// if (m_Text != value)
                {
                    m_Text = value;
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        public void SetLinkText(string value, List<LinkParse.LinkInfo> links)
        {
            m_akLinkInfos = links;
            text = value;
            if (null != m_spriteGraphic)
            {
                m_spriteGraphic.BeginDraw();
                m_spriteGraphic.EndDraw();
                m_spriteGraphic.SetVerticesDirty();
            }
            if (null != m_underLineGraphic)
            {
                m_underLineGraphic.BeginDraw();
                m_underLineGraphic.EndDraw();
                m_underLineGraphic.SetVerticesDirty();
            }
            _SetDynamicImages();
            _SetDynamicFrameSprites();
        }

        public void AddListener(OnClickItem listener)
        {
            onClickItem += listener;
        }

        public void RemoveListener(OnClickItem listener)
        {
            onClickItem -= listener;
        }

        public void AddListener(OnClickPlayerName listener)
        {
            onClickPlayerName += listener;
        }

        public void RemoveListener(OnClickPlayerName listener)
        {
            onClickPlayerName -= listener;
        }

        public void AddListener(OnClickLocalLink listener)
        {
            onClickLocalLink += listener;
        }

        public void RemoveListener(OnClickLocalLink listener)
        {
            onClickLocalLink -= listener;
        }

        public void AddListener(OnClickRetinueLink listener)
        {
            onClickRetinueLink += listener;
        }

        public void RemoveListener(OnClickRetinueLink listener)
        {
            onClickRetinueLink -= listener;
        }

        public void AddListener(OnClickWrapStoneLink listener)
        {
            onClickWrapStoneLink += listener;
        }

        public void RemoveListener(OnClickWrapStoneLink listener)
        {
            onClickWrapStoneLink -= listener;
        }

        public void RemoveAllDelegate()
        {
            onClickItem = null;
            onClickPlayerName = null;
            onClickLocalLink = null;
            onClickRetinueLink = null;
            onClickWrapStoneLink = null;
        }
        #endregion

        #region event
        public void OnPointerClick(PointerEventData eventData)
        {
            bool bHasClicked = false;
            if (m_akLinkInfos.Count > 0)
            {
                Vector2 lp;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, eventData.position, eventData.pressEventCamera, out lp);
                for (int i = 0; i < m_akLinkInfos.Count; ++i)
                {
                    var current = m_akLinkInfos[i];
                    if (current == null || current.bNeedRemove || current.allBounds.Count <= 0)
                    {
                        continue;
                    }

                    for (int j = 0; j < current.allBounds.Count; ++j)
                    {
                        var box = new Rect(current.allBounds[j].min, current.allBounds[j].size);
                        if (box.Contains(lp))
                        {
                            bHasClicked = true;
                            _OnClickLink(current);
                            break;
                        }
                    }
                }
            }
            if (!bHasClicked)
            {
                if (onFailed != null)
                {
                    onFailed.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //OnPointerClick(eventData);
        }

        void _OnClickLink(LinkParse.LinkInfo linkInfo)
        {
            var currentSystem = ClientSystemManager.GetInstance().GetCurrentSystem();
            if (currentSystem != null)
            {
                if (currentSystem is ClientSystemBattle)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("jump_smithshopframe_tips"));
                    return;
                }
            }

            if (linkInfo == null)
            {
                return;
            }

            switch (linkInfo.eRegexType)
            {
                case LinkParse.RegexType.RT_NET_ITEM:
                    {
                        Logger.LogProcessFormat("[link => item] id={0} dataid={1}", linkInfo.guid0, linkInfo.iParam0);

                        if (onClickItem != null)
                        {
                            onClickItem.Invoke(linkInfo.guid0, linkInfo.iParam0, linkInfo.iParam1);
                        }
                        else
                        {
                            GameClient.LinkManager.GetInstance().AttachDatas = null;
                            Parser.ItemParser.OnItemLink(linkInfo.guid0, linkInfo.iParam0);
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_PLAYER:
                    {
                        Logger.LogProcessFormat("[link => player] id={0} name={1} role={2} level={3}",
                            linkInfo.guid0,
                            linkInfo.str0,
                            linkInfo.iParam0,
                            linkInfo.iParam1);

                        if (onClickPlayerName != null)
                        {
                            onClickPlayerName.Invoke(linkInfo.guid0, linkInfo.str0, (byte)linkInfo.iParam0, (uint)linkInfo.iParam1);
                        }
                        else
                        {
                            Parser.Common.NameParse(linkInfo.guid0, (byte)linkInfo.iParam0, linkInfo.str0);
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_RETINUE:
                    {
                        Logger.LogProcessFormat("[link => retinue] id={0} dataid={1}",
                            linkInfo.guid0,
                            linkInfo.iParam0);

                        if (onClickRetinueLink != null)
                        {
                            onClickRetinueLink.Invoke(linkInfo.guid0, linkInfo.iParam0);
                        }
                        else
                        {
                            Parser.Common.RetinueParse(linkInfo.guid0, linkInfo.iParam0);
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_ENERGY_STONE:
                    {
                        Logger.LogProcessFormat("[link => wrapstone] dataid={0}", linkInfo.iParam0);

                        if (onClickWrapStoneLink != null)
                        {
                            onClickWrapStoneLink.Invoke(linkInfo.iParam0);
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_LOCAL:
                    {
                        Logger.LogProcessFormat("[link => RT_LOCAL] id={0} name={1}",
                            linkInfo.iParam0,
                            linkInfo.str0);

                        if (onClickLocalLink != null)
                        {
                            onClickLocalLink.Invoke(linkInfo.iParam0, linkInfo.str0);
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_RED_PACKET:
                    {
                        Logger.LogProcessFormat("[link => RT_RED_PACKET] id={0}", linkInfo.guid0);

                        Protocol.RedPacketBaseEntry data = GameClient.RedPackDataManager.GetInstance().GetRedPacketBaseInfo(linkInfo.guid0);
                        if (data != null)
                        {
                            if (data.status == (byte)Protocol.RedPacketStatus.WAIT_RECEIVE && data.opened == 0)
                            {
                                GameClient.RedPackDataManager.GetInstance().OpenRedPacket(data.id);
                            }
                            else
                            {
                                GameClient.RedPackDataManager.GetInstance().CheckRedPacket(data.id);
                            }
                        }
                        else
                        {
                            GameClient.SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_redpacket_invalid"));
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_GUILD_SHOP:
                    {
                        GuildMyData myData = GuildDataManager.GetInstance().myGuild;
                        if (myData != null)
                        {
                            if(myData.dictBuildings != null)
                            {
                                GuildBuildingData buildData = null;
                                myData.dictBuildings.TryGetValue(Protocol.GuildBuildingType.SHOP, out buildData);

                                if (buildData != null)
                                {
                                    int nShopLevel = buildData.nLevel;
                                    GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(nShopLevel);
                                    if (buildingTable != null)
                                    {
                                        ShopDataManager.GetInstance().OpenShop(buildingTable.ShopId, 0, -1, () =>
                                        {
                                            ClientSystemManager.instance.OpenFrame<GuildMainFrame>(FrameLayer.Middle, EOpenGuildMainFramData.OpenBuildingShop);
                                        });

                                        RedPointDataManager.GetInstance().ClearRedPoint(ERedPoint.GuildShop);
                                    }
                                }
                            }
                        }

                        break;
                    }
                case LinkParse.RegexType.RT_JOIN_TABLE:
                    {
                        //GameClient.GuildDataManager.GetInstance().JoinTable(-1);
                        if (GameClient.GuildDataManager.GetInstance().HasSelfGuild())
                        {

                            GameClient.ClientSystemManager.GetInstance().OpenFrame<GameClient.GuildMainFrame>(GameClient.FrameLayer.Middle);
                        }
                        else
                        {
                            GameClient.SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_TEAM_INVITE:
                    {
                        //{T 00}
                        if (TeamDataManager.GetInstance().HasTeam())
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_already_has_team"));
                            break;
                        }

                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (systemTown == null)
                        {
                            return;
                        }

                        CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                        if (scenedata == null)
                        {
                            return;
                        }

                        if (scenedata.SceneType == CitySceneTable.eSceneType.PK_PREPARE && PkWaitingRoom.bBeginSeekPlayer)
                        {
                            SystemNotifyManager.SystemNotify(4004);
                            return;
                        }

                        if (linkInfo.guid0 == PlayerBaseData.GetInstance().RoleID)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("队伍不存在");
                            break;
                        }

                        TeamDataManager.GetInstance().JoinTeam(linkInfo.guid0);
                        break;
                    }
                case LinkParse.RegexType.RT_GUILD_MANOR:
                    {
                        if(GuildDataManager.GetInstance().GetGuildBattleType() == Protocol.GuildBattleType.GBT_CROSS)
                        {
                            ClientSystemManager.instance.OpenFrame<GuildMainFrame>(FrameLayer.Middle,EOpenGuildMainFramData.OpenGuildCrossManor);
                        }
                        else
                        {
                            ClientSystemManager.instance.OpenFrame<GuildMainFrame>(FrameLayer.Middle, EOpenGuildMainFramData.OpenManor);
                        }
    
                        break;
                    }
                case LinkParse.RegexType.RT_SUPER_GROUP:
                    {
                        var superLinkItem = TableManager.GetInstance().GetTableItem<ProtoTable.SuperLinkTable>(linkInfo.iParam0);
                        if (null != superLinkItem)
                        {
                            if (!string.IsNullOrEmpty(superLinkItem.LinkInfo))
                            {
                                try
                                {
                                    bool bUnLocked = true;
                                    int iNeedLevel = 1;
                                    if (superLinkItem.OpenLevel.Count == 3)
                                    {
                                        try
                                        {
                                            System.Type type = System.Type.GetType(superLinkItem.OpenLevel[0]);
                                            if (null != type)
                                            {
                                                var string_id = string.Format(superLinkItem.OpenLevel[2], linkInfo.guid0, linkInfo.guid1, linkInfo.str0, linkInfo.str1);
                                                int data_id = 0;
                                                if (int.TryParse(string_id, out data_id))
                                                {
                                                    var tableItem = TableManager.GetInstance().GetTableItem(type, data_id);
                                                    if (null != tableItem)
                                                    {
                                                        var property = type.GetProperty(superLinkItem.OpenLevel[1], BindingFlags.Instance | BindingFlags.Public);
                                                        if (null != property)
                                                        {
                                                            iNeedLevel = (int)property.GetValue(tableItem, null);
                                                            bUnLocked = PlayerBaseData.GetInstance().Level >= iNeedLevel;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (System.Exception e)
                                        {
                                            bUnLocked = true;
                                            Logger.LogErrorFormat("READ UNLOCKLEVEL FAILED:\n {0},{1},{2},{3}", superLinkItem.OpenLevel[0], superLinkItem.OpenLevel[1], superLinkItem.OpenLevel[2], superLinkItem.OpenLevel[3]);
                                        }
                                    }

                                    if (bUnLocked)
                                    {
                                        var linkString = string.Format(superLinkItem.LinkInfo, linkInfo.guid0, linkInfo.guid1, linkInfo.str0, linkInfo.str1);
                                        if (!string.IsNullOrEmpty(linkString))
                                        {
                                            ActiveManager.GetInstance().OnClickLinkInfo(linkString);
                                        }
                                    }
                                    else
                                    {
                                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("common_can_not_goto", iNeedLevel));
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    Logger.LogErrorFormat("superLinkItem ID = {0} info={1} p1={2} p2={3} p3={4} p4={5}",
                                        linkInfo.iParam0,
                                        superLinkItem.LinkInfo,
                                        linkInfo.guid0,
                                        linkInfo.guid1,
                                        linkInfo.str0,
                                        linkInfo.str1);
                                }
                            }
                        }
                        break;
                    }
                case LinkParse.RegexType.RT_WEB_URL:
                    if (ClientSystemManager.GetInstance().IsFrameOpen<OperateAdsBoardFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<OperateAdsBoardFrame>();
                    }
                    string titleName = PluginManager.GetInstance().IsMGSDKChannel() ? TR.Value("operateAds_website") : TR.Value("operateAds_community");
                    ClientSystemManager.GetInstance().OpenFrame<OperateAdsBoardFrame>(FrameLayer.TopMoreMost, linkInfo.str0, titleName);
                    break;
            }
        }
        #endregion
    }
}