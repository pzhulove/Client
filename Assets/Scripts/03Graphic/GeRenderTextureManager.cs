using UnityEngine;
using System.Collections.Generic;

public interface IGeRenderTexture
{
    GameObject GetModelRoot();

    Camera GetCamera();

    Light GetLight();

    RenderTexture GetRenderTexture();

    int GetMaskLayer();
}

public class GeRenderTexture : IGeRenderTexture
{
    protected GameObject m_RendTexRoot = null;
    protected Camera m_RendTexCamera = null;
    protected GameObject m_RendModelRoot = null;
    protected Light m_RendTexLight = null;
    protected string m_RendTexName = null;
    protected RenderTexture m_RenderTexture = null;
    protected int m_MaskLayer = 0;

    protected int m_RefCnt = 0;

    public int refCount
    {
        get { return m_RefCnt; }
    }

    public string name
    {
        get { return m_RendTexName; }
    }

    public bool Init(string name ,GameObject attachRoot,int masklayer,int width,int height)
    {
        if (null != attachRoot)
        {
            m_RendTexRoot = new GameObject(name);
            if (null != m_RendTexRoot)
            {
                m_RendTexRoot.transform.SetParent(attachRoot.transform);
                Vector3 pos = m_RendTexRoot.transform.position;
                pos.y += 1;
                m_RendTexRoot.transform.position = pos;

                m_RendTexCamera = m_RendTexRoot.AddComponent<Camera>();
                if (null != m_RendTexCamera)
                {
                    Vector3 euler = new Vector3();
                    euler.x = 25;
                    euler.y = 0;
                    euler.z = 0;
                    m_RendTexCamera.transform.eulerAngles = euler;
                    Vector3 posCam = m_RendTexCamera.transform.position;
                    posCam.z -= 0.5f;
                    m_RendTexCamera.transform.position = posCam;

                    m_RendTexCamera.clearFlags = CameraClearFlags.SolidColor;
                    Color color = Color.black;
                    color.a = 0;
                    m_RendTexCamera.backgroundColor = color;
                    m_RendTexCamera.cullingMask = 1 << masklayer;
                    m_RendTexCamera.orthographic = true;
                    m_RendTexCamera.orthographicSize = 1.9f;
                    m_RendTexCamera.depth = 0;
                    m_RendTexCamera.nearClipPlane = -5.0f;
                    m_RendTexCamera.farClipPlane = 5.0f;
                    m_RendTexCamera.useOcclusionCulling = false;
                    m_RendTexCamera.allowHDR = false;
                    m_RendTexCamera.allowMSAA = false;

                    GameObject rendTexLightRoot = new GameObject("Directional Light");
                    if (null != rendTexLightRoot)
                    {
                        rendTexLightRoot.transform.SetParent(m_RendTexRoot.transform, false);
                        m_RendTexLight = rendTexLightRoot.AddComponent<Light>();
                        if (null != m_RendTexLight)
                        {
                            if (null != global::Global.Settings)
                                m_RendTexLight.transform.eulerAngles = global::Global.Settings.avatarLightDir;

                            m_RendTexLight.type = LightType.Directional;
                            m_RendTexLight.cullingMask = masklayer;
                            m_RendTexLight.color = Color.white;
                            m_RendTexLight.intensity = 0.5f;
                            m_RendTexLight.shadows = LightShadows.None;
                            m_RendTexLight.renderMode = LightRenderMode.Auto;

                            m_RendModelRoot = new GameObject("Model Root");
                            if (null != m_RendModelRoot)
                            {
                                m_RendModelRoot.transform.SetParent(m_RendTexRoot.transform, false);

                                m_RenderTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
                                if (null != m_RenderTexture)
                                {
                                    m_RenderTexture.wrapMode = TextureWrapMode.Clamp;
                                    m_RenderTexture.filterMode = FilterMode.Bilinear;

                                    m_RendTexName = name;
                                    m_MaskLayer = masklayer;

                                    m_RendTexCamera.targetTexture = m_RenderTexture;

                                    return true;
                                }
                                else
                                    Logger.LogErrorFormat("Create temporary render texture with w:{0} h:{1} fmt:{2} depth:{3} has failed!", width, height, RenderTextureFormat.ARGB32.ToString(), 16);
                            }
                            else
                                Logger.LogError("Create model root node has failed!");
                        }
                        else
                            Logger.LogError("Add light component has failed!");
                    }
                    else
                        Logger.LogError("Create light root node has failed!");
                }
                else
                    Logger.LogError("Add camera component has failed!");

                GameObject.Destroy(m_RendTexRoot);

                m_RendTexLight = null;
                m_RendTexCamera = null;
                m_RendModelRoot = null;
                m_RendTexRoot = null;
            }
            else
                Logger.LogError("Allocate game object has failed!");
        }
        else
            Logger.LogError("Attach root can not be null!");

        return false;
    }

    public void Deinit()
    {
        if(null != m_RenderTexture)
        {
            RenderTexture.ReleaseTemporary(m_RenderTexture);
            m_RenderTexture = null;
        }

        if(null != m_RendTexRoot)
        {
            GameObject.Destroy(m_RendTexRoot);
            m_RendTexRoot = null;

            m_RendTexCamera = null;
            m_RendModelRoot = null;
            m_RendTexLight = null;
        }
    }

    public void AddRef()
    {
        ++ m_RefCnt;
    }

    public void Release()
    {
        -- m_RefCnt;
    }

    public GameObject GetModelRoot()
    {
        return m_RendModelRoot;
    }

    public Camera GetCamera()
    {
        return m_RendTexCamera;
    }

    public Light GetLight()
    {
        return m_RendTexLight;
    }

    public RenderTexture GetRenderTexture()
    {
        return m_RenderTexture;
    }

    public int GetMaskLayer()
    {
        return m_MaskLayer;
    }
}

public class GeRenderTextureManager : Singleton<GeRenderTextureManager>
{
    protected readonly string ROOT_NODE_NAME = "Environment";

    public Dictionary<string, GeRenderTexture> m_RendTexNameTable = new Dictionary<string, GeRenderTexture>();
    public GameObject m_RootNode = null;
    public bool m_CreateRoot = false;

    protected GeRenderTexture m_DefaultRendTex = null;

    public GameObject RootNode
    {
        get
        {
            return m_RootNode;
        }
    }

    public override void Init()
    {
        m_RootNode = GameObject.Find(ROOT_NODE_NAME);
        m_CreateRoot = null == m_RootNode;
        if (m_CreateRoot)
        {
            m_CreateRoot = true;
            m_RootNode = new GameObject(ROOT_NODE_NAME);
            if (null == m_RootNode)
                Logger.LogError("Render texture manager init has failed, can not find root node");
        }

        GameObject.DontDestroyOnLoad(m_RootNode);
        m_DefaultRendTex = new GeRenderTexture();
        if (null != m_DefaultRendTex)
        {
            if (m_DefaultRendTex.Init("NULL", m_RootNode, 0, 4, 4))
            {
                m_DefaultRendTex.AddRef();
                Camera defCam = m_DefaultRendTex.GetCamera();
                if (null != defCam)
                {
                    defCam.cullingMask = 0;
                    defCam.enabled = false;
                }
            }
            else
            {
                Logger.LogError("Init default render texture has failed!");
                m_DefaultRendTex = null;
            }
        }
        else
            Logger.LogError("New render texture has failed!");
    }

    public override void UnInit()
    {
        _ClearAll();

        if (null != m_DefaultRendTex)
        {
            m_DefaultRendTex.Deinit();
            m_DefaultRendTex = null;
        }

        if (m_CreateRoot && null != m_RootNode)
            GameObject.Destroy(m_RootNode);

        m_RootNode = null;
    }

    public IGeRenderTexture FindRenderTextureByName(string name)
    {
        GeRenderTexture rendTex = null;
        if (m_RendTexNameTable.TryGetValue(name, out rendTex))
        {
            return rendTex;
        }

        return m_DefaultRendTex;
    }

    public IGeRenderTexture CreateRenderTexture(string name, int masklayer, int width, int height)
    {
        if(string.IsNullOrEmpty(name))
        {
            Logger.LogError("Texture name can not be null!");
            return null;
        }

        GeRenderTexture rendTex = null;
        if (m_RendTexNameTable.TryGetValue(name, out rendTex))
        {
            RenderTexture rt = rendTex.GetRenderTexture();
            if (rt.width != width || rt.height != height || rendTex.GetMaskLayer() != masklayer)
            {
                Logger.LogErrorFormat("There is already exist a render texture named {0} with different parameters[w:{1} h:{2} layer:{3}]", name, rt.width, rt.height, rendTex.GetMaskLayer());
                return null;
            }
            else
            {
                rendTex.AddRef();
                return rendTex;
            }
        }

        rendTex = new GeRenderTexture();
        if (null != rendTex)
        {
            if(rendTex.Init(name,m_RootNode,masklayer,width,height))
            {
                rendTex.AddRef();
                m_RendTexNameTable.Add(name, rendTex);
                return rendTex;
            }
            else
                Logger.LogErrorFormat("Init render texture named {0} with parameters[w:{1} h:{2} layer:{3}] has failed!", name, width, height, rendTex.GetMaskLayer());

            rendTex = null;
        }
        else
            Logger.LogError("New render texture has failed!");

        return null;
    }

    public void DestroyRenderTexture(IGeRenderTexture rendTex)
    {
        if (rendTex == m_DefaultRendTex as IGeRenderTexture)
            return;

        GeRenderTexture curRendTex = rendTex as GeRenderTexture;
        if(null != curRendTex )
        {
            curRendTex.Release();
            if (0 == curRendTex.refCount)
            {
                curRendTex.Deinit();
                m_RendTexNameTable.Remove(curRendTex.name);
            }
        }
    }

    protected void _ClearAll()
    {
        Dictionary<string, GeRenderTexture>.Enumerator it = m_RendTexNameTable.GetEnumerator();
        while (it.MoveNext())
        {
            GeRenderTexture value = it.Current.Value;
            if (0 != value.refCount)
                Logger.LogWarningFormat("Render texture {0} remain reference {1} yet", value.name , value.refCount );

            value.Deinit();
        }

        m_RendTexNameTable.Clear();
    }
}
