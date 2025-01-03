using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    [ExecuteAlways][RequireComponent(typeof(Image))]
    class SpriteAniRenderChenghao : MonoBehaviour
    {
        public string orgPath;
        public string orgName;
        public int count;
        public string prefabPath;
        int iIndex;
        Image image;
        float fScale = 1.0f;
        public SpeedString speedString = new SpeedString(256);

        private Texture2D m_AtlasTexture;
        private Vector4 m_UVScaleOff = new Vector4(1, 1, 0, 0);
        private Vector2 m_AtlasSize = new Vector2(0, 0);
        private Vector2 m_FrameTexSize = new Vector2(256, 64);
        private int m_UVScaleOffPropertyID = -1;
        private int m_FrameColumn = 1;

        [System.NonSerialized]
        private List<Sprite> m_AnimSpirteList = new List<Sprite>();

        [System.NonSerialized]
        protected float m_CurTime = 0;
        [System.NonSerialized]
        protected float m_FrameTime = 0.1F; // 1 / 10

        public void SetEnable(bool bEnable)
        {
            if (image != null)
            {
                if(bEnable)
                {
                    InitRes(image, false);
                }
                else
                {
                    image.sprite = null;
                    image.material = null;
                    m_AtlasTexture = null;
                }
                image.enabled = bEnable;
            }
        }

        private void LoadAtlas(ref Texture2D atlasTexture, ref Sprite uniqueSprite)
        {
            atlasTexture = null;
            uniqueSprite = null;

            if (orgPath != "")
            {
                speedString.Clear();
                speedString.Append(orgPath);
                int lastIndex = orgPath.LastIndexOf('/');
                int lastPreIndex;
                if (lastIndex > 0)
                {
                    lastPreIndex = orgPath.LastIndexOf('/', lastIndex - 1);
                }
                else
                {
                    lastIndex = orgPath.LastIndexOf('\\');
                    lastPreIndex = orgPath.LastIndexOf('\\', lastIndex - 1);
                }

                string atlasName = orgPath.Substring(lastPreIndex + 1, lastIndex - lastPreIndex - 1);
                speedString.Append(atlasName);
                speedString.Append(".png");

                var textBaseStringWithTrimEnd = speedString.GetBaseStringWithEndTrim();
                atlasTexture = AssetLoader.instance.LoadRes(textBaseStringWithTrimEnd, typeof(Texture2D)).obj as Texture2D;

                speedString.Append(":");
                speedString.Append(atlasName);
                speedString.Append("_0");

                var spriteBaseStringWithTrimEnd = speedString.GetBaseStringWithEndTrim();
                uniqueSprite = AssetLoader.instance.LoadRes(spriteBaseStringWithTrimEnd, typeof(Sprite)).obj as Sprite;
            }
        }
        
        public void Reset(string orgPath,string orgName,int count,float fScale, string prefabPath=null)
        {
            m_AnimSpirteList.Clear();
            this.orgPath = orgPath;
            this.orgName = orgName;
            this.count = count;
            this.prefabPath = prefabPath;
            iIndex = 0;
            image = GetComponent<Image>();
            if(image.enabled)
            {
                InitRes(image, true);
            }
        }

        void InitRes(Image image, bool forceReload)
        {
            if (image)
            {
                if (forceReload || image.sprite == null || image.material == null)
                {
                    Sprite sprite = null;

                    
					//这里改成读预制体，但是预制体不会默认加载对应的贴图，从预制体力读到frame的rect和frame count，总帧数不读表了
                    if (string.IsNullOrEmpty(this.prefabPath))
                    {
                        sprite = AssetLoader.instance.LoadRes("UI/Image/ChenghaoDefault/ChenghaoSprite.png:ChenghaoSprite", typeof(Sprite)).obj as Sprite;
                    }
                    else 
                    {
                        var go = AssetLoader.instance.LoadResAsGameObject(this.prefabPath, false);
                        if (go != null)
                        {
                            var comAniFrame = go.GetComponent<GeAnimFrameBillboard>();
                            if (comAniFrame != null)
                            {
                                this.count = comAniFrame.m_FrameCount;
                            }
                            var comSpriteRenderer = go.GetComponent<SpriteRenderer>();
                            if (comSpriteRenderer != null)
                            {
                                sprite = GameObject.Instantiate(comSpriteRenderer.sprite) as Sprite;
                            }

                            GameObject.Destroy(go);
                        }
                    }

                    Material mat = AssetLoader.instance.LoadRes("UI/Material/ChenghaoMatImage.mat", typeof(Material)).obj as Material;
                    if (sprite && mat)
                    {
                        image.rectTransform.sizeDelta = new Vector2(sprite.bounds.size.x * 100.0f * fScale, sprite.bounds.size.y * 100.0f * fScale);
                        image.sprite = sprite;
                        image.material = new Material(mat);

                        m_UVScaleOffPropertyID = Shader.PropertyToID("_AtalsTex_ST");

                        Sprite uniqueSprite = null;
                        LoadAtlas(ref m_AtlasTexture, ref uniqueSprite);

                        if (m_AtlasTexture)
                        {
                            m_AtlasSize.Set(m_AtlasTexture.width, m_AtlasTexture.height);

                            // if (uniqueSprite)
                            // {
                            //     image.sprite = uniqueSprite;
                            //     m_FrameTexSize = new Vector2(uniqueSprite.rect.width, uniqueSprite.rect.height);

                            //     m_UVScaleOff.x = m_UVScaleOff.y = 1.0F;
                            // }
                            // else
                            // {
                            //     m_FrameTexSize = new Vector2(256, 64);

                            //     m_UVScaleOff.x = m_FrameTexSize.x / m_AtlasSize.x;
                            //     m_UVScaleOff.y = m_FrameTexSize.y / m_AtlasSize.y;
                            // }

                            m_FrameTexSize = new Vector2(sprite.rect.width, sprite.rect.height);
                            m_UVScaleOff.x = m_FrameTexSize.x / m_AtlasSize.x;
                            m_UVScaleOff.y = m_FrameTexSize.y / m_AtlasSize.y;

                            m_FrameColumn = (int)m_AtlasSize.x / (int)m_FrameTexSize.x;

                            image.material.SetTexture("_AtalsTex", m_AtlasTexture);

                            int iIndex = 0;
                            m_UVScaleOff.z = ((float)(iIndex % m_FrameColumn) * m_FrameTexSize.x) / m_AtlasSize.x;
                            m_UVScaleOff.w = 1.0F - (((int)(iIndex / m_FrameColumn) + 1) * m_FrameTexSize.y) / m_AtlasSize.y;

                            image.material.SetVector(m_UVScaleOffPropertyID, m_UVScaleOff);
                        }
                    }

                    image.materialForRendering.SetTexture("_AtalsTex", m_AtlasTexture);
                    image.materialForRendering.SetVector(m_UVScaleOffPropertyID, m_UVScaleOff);

                    m_CurTime = 0;
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            iIndex = 0;
            m_CurTime = 0;
            image = GetComponent<Image>();
            if(count < 1)
            {
                count = 1;
            }

            if(image.enabled)
            {
                InitRes(image, false);
            }
        }

        void OnDestroy()
        {
            m_AnimSpirteList.Clear();
            m_AtlasTexture = null;
        }

        // Update is called once per frame
        void Update()
        {
            if(null == image || !image.enabled)
            {
                return;
            }

            if(!image.material.name.Contains("ChenghaoMatImage"))
            {
                InitRes(image, true);
            }

            m_CurTime += Time.deltaTime;
            if (m_CurTime > m_FrameTime)
            {
                m_CurTime -= m_FrameTime;
                _PlayOneTimes();
            }
        }

        int _GetNumCount(int iValue)
        {
            iValue = iValue > 0 ? iValue : -iValue;
            int iNum = 0;
            if(iValue == 0)
            {
                iNum = 1;
            }
            else
            {
                while (iValue > 0)
                {
                    ++iNum;
                    iValue /= 10;
                }
            }
            return iNum;
        }

        Sprite _LoadSprite(int iIndex)
        {
            if (m_AnimSpirteList.Count <= iIndex)
            {
                for (int i = m_AnimSpirteList.Count,icnt = iIndex +1;i<icnt;++i)
                {
                    speedString.Clear();
                    speedString.Append(orgPath);
                    speedString.Append(orgName);

                    int iNumCount = _GetNumCount(i);

                    for (int j = 0; j < 5 - iNumCount;++j)
                    {
                        speedString.Append(0);
                    }
                    speedString.Append(i);

                    speedString.Append(".png:");
                    speedString.Append(orgName);

                    for (int j = 0; j < 5 - iNumCount; ++j)
                    {
                        speedString.Append(0);
                    }
                    speedString.Append(i);

                    var cur = speedString.GetBaseStringWithEndTrim();
                    m_AnimSpirteList.Add(AssetLoader.instance.LoadRes(cur, typeof(Sprite)).obj as Sprite);
                }
            }

            return m_AnimSpirteList[iIndex];
        }

        void _PlayOneTimes()
        {
            //image.sprite = _LoadSprite(iIndex);

            if (m_AtlasTexture)
            {
                m_UVScaleOff.z = ((float)(iIndex % m_FrameColumn) * m_FrameTexSize.x) / m_AtlasSize.x;
                m_UVScaleOff.w = 1.0F - (((int)(iIndex / m_FrameColumn) + 1) * m_FrameTexSize.y) / m_AtlasSize.y;

                image.materialForRendering.SetVector(m_UVScaleOffPropertyID, m_UVScaleOff);
                image.SetMaterialDirty();
            }

            iIndex = (iIndex + 1) % count;
        }
    }
}