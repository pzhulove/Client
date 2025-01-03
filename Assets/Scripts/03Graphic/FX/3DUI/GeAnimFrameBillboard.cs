using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeAnimFrameBillboard : MonoBehaviour
{
    [SerializeField]
    public int m_FrameRate = 10;

    [SerializeField]
    public string m_AtlasName;
    [SerializeField]
    public int m_FrameCount = 0;

    private Texture2D m_AtlasTexture;
    private uint m_AtlasLoadHandle = uint.MaxValue;
    private MaterialPropertyBlock m_MaterialParams;
    private SpriteRenderer m_SpirteRenderer;
    private int m_UVScaleOffPropertyID = -1;
    private Vector4 m_UVScaleOff = new Vector4(1, 1, 0, 0);
    private Vector2 m_AtlasSize = new Vector2(0,0);
    private int m_FrameColumn = 1;
    private Vector2 m_FrameTexSize = new Vector2(256, 64);

    [System.NonSerialized]
    protected int m_CurFrame = 0;
    [System.NonSerialized]
    protected float m_CurTime = 0;
    [System.NonSerialized]
    protected float m_FrameTime = 0;

    [System.NonSerialized]
    protected bool m_IsPuased = false;
    
    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        if (null == userData)
        {
#if UNITY_EDITOR
            Tenmove.Runtime.Debugger.LogError("User data can not be null!");
#endif
            return;
        }

        if (null == asset)
        {
#if UNITY_EDITOR
            Tenmove.Runtime.Debugger.LogError("asset object is null!");
#endif
            return;
        }

        GeAnimFrameBillboard _this = userData as GeAnimFrameBillboard;
        if (null == _this)
        {
#if UNITY_EDITOR
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT GeAnimFrameBillboard!", userData.GetType());
#endif
            return;
        }

        Texture2D atlasTexture = asset as Texture2D;
        if (null == atlasTexture)
        {
#if UNITY_EDITOR
            Tenmove.Runtime.Debugger.LogError("Asset type '{0}' error!", asset.GetType());
#endif
            return;
        }

        if (AssetLoader.INVILID_HANDLE != _this.m_AtlasLoadHandle)
        {
            if ((uint)grpID == _this.m_AtlasLoadHandle)
            {
                _this._OnTextureLoadFinished(atlasTexture);
                _this.m_AtlasLoadHandle = AssetLoader.INVILID_HANDLE;
                return;
            }
        }
    }

    static void _OnLoadAssetFailure(string assetPath, int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
#if UNITY_EDITOR
        Logger.LogErrorFormat("[GeAnimFrameBillboard]Load texture '{0}' has failed![Error:{1}]",assetPath,errorMessage);
#endif
    }

    public int frameRate
    {
        get { return m_FrameRate; }
        set { m_FrameRate = value; }
    }

    // Use this for initialization
    void Start()
    {
        
    }

    public void Init()
    {
        m_SpirteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_UVScaleOffPropertyID = Shader.PropertyToID("_UVScaleOff");
        m_MaterialParams = new MaterialPropertyBlock();
        if (m_SpirteRenderer != null)
        {
            m_SpirteRenderer.enabled = false; // 贴图未加载成功时禁用
            if (m_SpirteRenderer.sprite != null && m_SpirteRenderer.sprite.texture != null)
            {
                m_FrameTexSize.Set(m_SpirteRenderer.sprite.texture.width, m_SpirteRenderer.sprite.texture.height);
            }
        }
        
        m_FrameTime = 1.0f / m_FrameRate;


        if (m_AtlasLoadHandle == uint.MaxValue)
        {
            m_AtlasLoadHandle = AssetLoader.LoadResAsync(Path.ChangeExtension(m_AtlasName, null), typeof(Texture2D), m_AssetLoadCallbacks, this);
        }

        Play(true);

#if UNITY_EDITOR
        if(m_SpirteRenderer == null)
        {
            Debug.LogError("GeAnimFrameBillboard attached on gameobject without SpriteRenderer: " + gameObject.name);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPuased)
            return;

        if (!EngineConfig.useTMEngine)
        {
            if (m_AtlasLoadHandle != uint.MaxValue)
            {
                if (AssetLoader.instance.IsRequestDone(m_AtlasLoadHandle))
                {
                    Texture2D atlasTexture = AssetLoader.instance.Extract(m_AtlasLoadHandle).obj as Texture2D;
                    _OnTextureLoadFinished(atlasTexture);
                    m_AtlasLoadHandle = uint.MaxValue;
                    /// if(m_AtlasTexture != null && m_SpirteRenderer != null && m_UVScaleOffPropertyID != -1)
                    /// {
                    ///     m_SpirteRenderer.enabled = true;
                    ///     m_AtlasSize.Set(m_AtlasTexture.width, m_AtlasTexture.height);
                    ///     m_FrameColumn = (int)m_AtlasSize.x / (int)m_FrameTexSize.x;
                    /// 
                    ///     m_UVScaleOff.x = m_FrameTexSize.x / m_AtlasSize.x;
                    ///     m_UVScaleOff.y = m_FrameTexSize.y / m_AtlasSize.y;
                    /// 
                    ///     if (m_MaterialParams != null)
                    ///     {
                    ///         m_MaterialParams.SetTexture("_MainTex", m_AtlasTexture);
                    ///         m_SpirteRenderer.SetPropertyBlock(m_MaterialParams);
                    ///     }
                    /// }
                    /// 
                    /// m_AtlasLoadHandle = uint.MaxValue;
                }
            }
        }

        m_CurTime += Time.deltaTime;
        if(m_CurTime > m_FrameTime)
        {
            m_CurTime -= m_FrameTime;
            ++m_CurFrame;

            if (m_CurFrame >= m_FrameCount)
                m_CurFrame = 0;

            if (m_SpirteRenderer != null && m_SpirteRenderer.enabled)
            {
                m_UVScaleOff.z = ((float)(m_CurFrame % m_FrameColumn) * m_FrameTexSize.x) / m_AtlasSize.x;
                m_UVScaleOff.w = 1.0F - (((int)(m_CurFrame / m_FrameColumn) + 1) * m_FrameTexSize.y) / m_AtlasSize.y;

                if (m_MaterialParams != null)
                {
                    m_MaterialParams.SetVector(m_UVScaleOffPropertyID, m_UVScaleOff);
                    m_SpirteRenderer.SetPropertyBlock(m_MaterialParams);
                }
            }
        }
    }

    public void SetVisible(bool isVisible)
    {
        if(null != m_SpirteRenderer)
        {
            Color col = m_SpirteRenderer.color;
            col.a = isVisible ? 1 : 0;
            m_SpirteRenderer.color = col;
        }
    }

    public void SetPause(bool pause)
    {
        m_IsPuased = pause;
    }

    public float GetTimeLength()
    {
        return m_FrameTime * m_FrameCount;
    }

    public void Play(bool isReset)
    {
        m_IsPuased = false;
        if (isReset)
        {
            m_CurFrame = 0;
            m_CurTime = 0;
        }
    }

    void OnDestroy()
    {
        if (EngineConfig.useTMEngine)
        {
            if (m_AtlasLoadHandle != uint.MaxValue)
            {
                AssetLoader.instance.AbortRequest(m_AtlasLoadHandle);
            }
        }
    }

    void Preload()
    {

        if (m_AtlasLoadHandle == uint.MaxValue)
        {
            m_AtlasLoadHandle = AssetLoader.LoadResAsync(Path.ChangeExtension(m_AtlasName, null), typeof(Texture2D), m_AssetLoadCallbacks, this);
        }
    }

    protected void _OnTextureLoadFinished(Texture2D texture)
    {
        if (null == texture)
        {
            Logger.LogWarning("Texture can not be null!");
            return;
        }

        m_AtlasTexture = texture;

        if (m_AtlasTexture != null && m_SpirteRenderer != null && m_UVScaleOffPropertyID != -1)
        {
            m_SpirteRenderer.enabled = true;
            m_AtlasSize.Set(m_AtlasTexture.width, m_AtlasTexture.height);
            m_FrameColumn = (int)m_AtlasSize.x / (int)m_FrameTexSize.x;

            m_UVScaleOff.x = m_FrameTexSize.x / m_AtlasSize.x;
            m_UVScaleOff.y = m_FrameTexSize.y / m_AtlasSize.y;

            if (m_MaterialParams != null)
            {
                m_MaterialParams.SetTexture("_MainTex", m_AtlasTexture);
                m_SpirteRenderer.SetPropertyBlock(m_MaterialParams);
            }
        }

    }
}
