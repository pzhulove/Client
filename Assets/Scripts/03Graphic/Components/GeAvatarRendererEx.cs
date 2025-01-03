using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(CanvasRenderer))]
public class GeAvatarRendererEx : MonoBehaviour, IDragHandler,IGeAvatarActor
{
    private static readonly string m_FXAAShaderName = "HeroGo/UI/ImageFxaa";
    protected struct EffectLayerBackup
    {
        public GeEffectEx effect;
        public int layer;
    }

    public string m_RenderTexName = "DeftAvatarRend";

    public Vector3 m_LightRot = new Vector3();
    public Color m_LightCol = Color.white;
    public Color m_AmbientCol = Color.black;
    public Color m_OriginAmbient = Color.black;

    public float m_OrthoSize = 1.2f;
    public Vector3 m_CameraRot = new Vector3(0, 0, 0);
    public Vector3 m_CameraPos = new Vector3(0, 1, -1);
    public float m_NearPlane = 0.3f;
    public float m_FarPlane = 10.0f;
    public float m_FOV = 45.0f;
    public bool m_IsPersp = false;

    public RawImage m_RawImage = null;
    public Material m_OriginMaterial;
    public Material m_FxaaMaterial;

    private int m_Layer
    {
        get
        {
            if (layer == -1)
            {
                // 外界赋值无效，只是保持代码不动，所有的layer现在都是通过UseGeAvatarLayer动态获取, by Wangbo 2020.04.28
                if(!bHasSetLayer)
                {
                    layer = GameClient.PlayerBaseData.GetInstance().UseGeAvatarLayer();
                    bHasSetLayer = true;
                }
                else
                {
                    // 检查有效性
                    if(GameClient.PlayerBaseData.GetInstance().IsLayerValid(m_Layer))
                    {
                        // 检查是否被管理
                        if(!GameClient.PlayerBaseData.GetInstance().IsUsingLayer(m_Layer))
                        {
                            GameClient.PlayerBaseData.GetInstance().AddLayer(m_Layer);
                        }
                    }
                    else
                    {
                        // 容错处理
                        layer = GameClient.PlayerBaseData.GetInstance().UseGeAvatarLayer();
                    }
                }
            }
            return layer;
        }
      
    } // 不再暴露，已经是动态分配，如果动态分配的有问题，那么就解决动态分配的问题，而不是再改回public。 by wangbo 2020.05.06
    
    private int layer = -1;
    public int m_TextureWidth = 619;
    public int m_TextureHight = 817;

    public string m_AvatarPath = "";
    public string m_AvatarName = "";

    //-------------------------阴影----------------------------
    public bool m_EnableShadow = false;
    public Vector3 m_ShadowCameraPos;
    public Vector3 m_ShadowCameraRotation;
    public float m_ShadowCameraSize;
    public float m_ShadowBias;

    private Camera m_ShadowCamera;
    private RenderTexture m_ShadowRT;
    private Matrix4x4 m_LightSpaceMatrix;
    //-------------------------阴影----------------------------

    protected Light m_Light = null;
    protected GameObject m_Root = null;
    protected Camera m_Camera = null;

    protected IGeRenderTexture m_RenderTexture = null;

    protected GameObject m_AvatarRoot = null;
    protected GeAvatar m_Avatar = null;
    //protected GeAnimationManager m_Animation = null;
    //protected GeAttachManager m_Attachment = null;

	protected GeEffectManager m_Effect = null;

    protected AssetInst m_ModelDataAsset = null;
    protected DModelData m_ModelData = null;

    protected float m_CurRotate = 0.0f;
    protected Color m_LastAmbient = Color.black;
    private bool m_NeedInitAfterLoad = false;
    private List<EffectLayerBackup> m_EffectLayerBackUpList = new List<EffectLayerBackup>();
    private bool bHasSetLayer = false;

    public bool m_FxaaEffectOn = true;

    protected RawImage RawImage 
    {
        get 
        {
            if (null == m_RawImage)
                m_RawImage = GetComponent<RawImage>();
            return m_RawImage;
        }
    }


    protected void _Init()
    {
        //m_CameraRot = Vector3.zero;


        if (null == m_AvatarRoot)
            m_AvatarRoot = new GameObject("AvatarRender");

		if (null == m_Effect)
			m_Effect = new GeEffectManager();

        if (null == m_Avatar)
            m_Avatar = new GeAvatar();
        //if (null == m_Animation)
        //    m_Animation = new GeAnimationManager();
        //if (null == m_Attachment)
        //    m_Attachment = new GeAttachManager();

        if (m_TextureWidth < 1)
            m_TextureWidth = 1;
        if (m_TextureHight < 1)
            m_TextureHight = 1;

        m_OriginMaterial = RawImage.material;
        if (m_FxaaMaterial != null)
            GeMaterialPool.instance.RecycleMaterialInstance(m_FXAAShaderName, m_FxaaMaterial);
        else
            m_FxaaMaterial = GeMaterialPool.instance.CreateMaterialInstance(m_FXAAShaderName);
        EnableFXAA(m_FxaaEffectOn);
        if(null == m_RenderTexture)
        {
            if (m_Layer == -1)
            {
                Logger.LogErrorFormat("Load avatar failed because m_Layer == -1, avatarRes = {0}", m_AvatarPath);
                return;
            }

            RectTransform rectTransform = transform as RectTransform;
            if(rectTransform != null)
                m_RenderTexture = GeRenderTextureManager.instance.CreateRenderTexture(gameObject.name + gameObject.GetHashCode(), m_Layer, (int)rectTransform.rect.width, (int)rectTransform.rect.height);
            else
                m_RenderTexture = GeRenderTextureManager.instance.CreateRenderTexture(gameObject.name + gameObject.GetHashCode(), m_Layer, m_TextureWidth, m_TextureHight);

            if (null != m_RenderTexture)
            {
                m_Light = m_RenderTexture.GetLight();
                m_Camera = m_RenderTexture.GetCamera();
                m_Root = m_RenderTexture.GetModelRoot();
                if (null != m_Camera)
                {
                    Camera.onPreRender += AvatarPreRender;
                    Camera.onPostRender += AvatarPostRender;

                    RawImage canvas = gameObject.GetComponent<RawImage>();
                    if (null != canvas)
                        canvas.texture = m_RenderTexture.GetRenderTexture();

                    _RefreshValues();

                    // Shadow
                    if(m_EnableShadow)
                    {
                        m_ShadowRT = new RenderTexture(512, 512, 16, RenderTextureFormat.Default)
                            { name = "AvatarShadowMap" };

                        GameObject shadowCameraGo = new GameObject() { name = "AvatarShadowCamera" };
                        shadowCameraGo.transform.SetParent(GeRenderTextureManager.instance.RootNode.transform, false);
                        shadowCameraGo.transform.localPosition = m_ShadowCameraPos;
                        shadowCameraGo.transform.localRotation = Quaternion.Euler(m_ShadowCameraRotation);
                        m_ShadowCamera = shadowCameraGo.AddComponent<Camera>();

                        m_ShadowCamera.clearFlags = CameraClearFlags.SolidColor;
                        m_ShadowCamera.backgroundColor = Color.white;
                        m_ShadowCamera.cullingMask = m_Camera.cullingMask;
                        m_ShadowCamera.depth = m_Camera.depth - 0.1f;
                        m_ShadowCamera.targetTexture = m_ShadowRT;
                        m_ShadowCamera.orthographic = true;
                        m_ShadowCamera.aspect = 1.0f;
                        m_ShadowCamera.orthographicSize = m_ShadowCameraSize;
                        m_ShadowCamera.farClipPlane = 10;
                        m_ShadowCamera.allowHDR = false;
                        m_ShadowCamera.allowMSAA = false;
                        m_ShadowCamera.useOcclusionCulling = false;

                        Shader shadowShader = AssetShaderLoader.Find("Hidden/ViewDepthTexture");
                        if (shadowShader != null)
                            m_ShadowCamera.SetReplacementShader(shadowShader, "RenderType");
                        else
                            Debugger.LogError("Can not find DepthTexture shader.");
                    }

                    return;
                }
                else
                    Logger.LogWarning("Get camera form render texture has failed!");
            }
            else
                Logger.LogWarning("create render texture has failed!");
        }
    }

    public void EnableFXAA(bool enable)
    {
        
        if (enable)
            RawImage.material = m_FxaaMaterial;
        else
            RawImage.material = m_OriginMaterial;
    }

    void Update()
    {
		float timeElapsed = Time.deltaTime;

        if (null != m_Avatar) 
        {
            int delta = (int)(timeElapsed * 1000);

            if (!m_Avatar.IsLoadFinished())
            {
                m_NeedInitAfterLoad = true;
                m_Avatar.UpdateAsyncLoading();
            }
            else
            {
                if(m_NeedInitAfterLoad)
                    _OnAvatarLoaded();

				if(null != m_Effect)
				{
					m_Effect.Update(delta,GeEffectType.EffectMultiple);
					m_Effect.Update(delta, GeEffectType.EffectUnique);
				}
            }
            m_Avatar.Update(delta, (uint)GeAvatar.EAvatarRes.Attach);
        }

    }

    protected void _OnAvatarLoaded()
    {
        GameObject avatarGo = m_Avatar.GetAvatarRoot();
        if (null != avatarGo)
        {
            avatarGo.transform.SetParent(m_AvatarRoot.transform, false);
            avatarGo.SetActive(true);
            m_Avatar.SetAvatarVisible(true);
            m_Avatar.SetAmbientColor(m_AmbientCol);
            m_NeedInitAfterLoad = false;
        }
    }

    protected void _RefreshValues()
    {
        if(null != m_Camera)
        {
            m_Camera.transform.eulerAngles = m_CameraRot;
            m_Camera.transform.position = m_CameraPos;

            m_Camera.orthographic = !m_IsPersp;
            m_Camera.orthographicSize = m_OrthoSize;
            m_Camera.fieldOfView = m_FOV;
            m_Camera.nearClipPlane = m_NearPlane;
            m_Camera.farClipPlane = m_FarPlane;
        }

        if(null != m_Light)
        {
            m_Light.color = m_LightCol;
            m_Light.transform.eulerAngles = m_LightRot;
        }
    }

    protected void _Deinit()
    {
#if !LOGIC_SERVER
        ResetEffectLayer();
#endif

        if (null != m_RenderTexture)
        {
            GeRenderTextureManager.instance.DestroyRenderTexture(m_RenderTexture);

            m_RenderTexture = null;
            m_Light = null;
            m_Camera = null;
            m_Root = null;
        }

		if(null != m_Effect)
		{
			m_Effect.Deinit();
			m_Effect = null;
		}
        if(m_OriginMaterial!= null)
            RawImage.material = m_OriginMaterial;
        if (m_FxaaMaterial != null)
            GeMaterialPool.instance.RecycleMaterialInstance(m_FXAAShaderName, m_FxaaMaterial);
        //if (null != m_Animation)
        //{
        //    m_Animation.Deinit();
        //    m_Animation = null;
        //}
        //
        //if (null != m_Attachment)
        //{
        //    m_Attachment.Deinit();
        //    m_Attachment = null;
        //}

        if (null != m_Avatar)
        {
            GameObject avatarNode = m_Avatar.GetAvatarRoot();
            if (avatarNode)
                avatarNode.transform.SetParent(null, true);
            m_Avatar.Deinit();
            m_Avatar = null;
        }

        if (m_AvatarRoot)
        {
            GameObject.Destroy(m_AvatarRoot);
            m_AvatarRoot = null;
        }

        if (null != m_ModelDataAsset)
        {
            m_ModelDataAsset = null;
            m_ModelData = null;
        }

        if (m_ShadowCamera != null)
        {
            m_ShadowCamera.targetTexture = null;
            if (m_ShadowRT != null)
                Destroy(m_ShadowRT);
            Destroy(m_ShadowCamera.gameObject);
        }

        Camera.onPreRender -= AvatarPreRender;
        Camera.onPostRender -= AvatarPostRender;
    }

    static readonly GeAvatarChannel[] avatarChanTbl = new GeAvatarChannel[]
    {
        GeAvatarChannel.Head,       /// eModelHead
        GeAvatarChannel.UpperPart,  /// eModelUpperPart
        GeAvatarChannel.LowerPart,  /// eModelLowerPart
        GeAvatarChannel.Bracelet,   /// eModelBracelet
        GeAvatarChannel.Headwear,   /// eModelHeadwear
        GeAvatarChannel.Wings,      /// eModelWings
    };


    public void LoadAvatar(string avatarRes,int layer = -1)
    {
#if !LOGIC_SERVER
        if (null == m_AvatarRoot)
        {
            Logger.LogWarningFormat("Load avatar with out initialize, did you load avatar under the inactive state?");
            return;
        }

        if (-1 != layer)
        {
            //m_Layer = layer;
        }
            
        ClearAvatar();
        _Deinit();

     
        _Init();

        if (!string.IsNullOrEmpty(avatarRes))
        {
            AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.SceneActor);
            m_AvatarPath = avatarRes;
            m_AvatarName = Path.GetFileNameWithoutExtension(m_AvatarPath);

            m_ModelDataAsset = AssetLoader.instance.LoadRes(GetModelDataPath(), typeof(ScriptableObject), false);
            if (null != m_ModelDataAsset)
            {
                m_ModelData = m_ModelDataAsset.obj as DModelData;
                if (m_ModelData)
                {
                    //string avatarValidPath = m_AvatarPath.Substring(0, m_AvatarPath.Length - m_AvatarName.Length - 1);
                    //if (m_Avatar.Init(avatarValidPath + "/Animations/Avatar", m_Layer))
                    if (m_Avatar.Init(m_ModelData.modelAvatar.m_AssetPath, m_Layer))
                    {
                        RemoveAvatarFixRotate(m_Avatar);
                        //m_Avatar.GetAvatarRoot().transform.SetParent(m_AvatarRoot.transform, false);
                        m_NeedInitAfterLoad = true;
                        _OnAvatarLoaded();

                        int remapIndex = 0;
                        for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
                        {
                            remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
                            if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
                                m_Avatar.AddDefaultAvatar(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset);
                            else
                                Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
                        }

                        m_Avatar.ChangeAction("Anim_Show_Idle", 1, true);
                        m_Avatar.SetLoadCallback(OnLoaded);
                        return;
                    }
                    else
                        Logger.LogErrorFormat("Init avatar has failed with resource path {0}", avatarRes);
                }
                else
                {
                    if (m_Avatar.Init(avatarRes, m_Layer,true,true))
                    {
                        RemoveAvatarFixRotate(m_Avatar);
                        // m_Avatar.GetAvatarRoot().transform.SetParent(m_AvatarRoot.transform, false);
                        m_Avatar.ChangeLayer(m_Layer);
                        m_Avatar.ChangeAction("Anim_Idle", 1, true);
                        m_NeedInitAfterLoad = true;
                        return;
                    }
                    else
                        Logger.LogErrorFormat("Init avatar has failed with resource path {0}", avatarRes);
                }
            }

            // 加载失败要第一时间释放被占用的layer
            GameClient.PlayerBaseData.GetInstance().ReleaseGeAvatarLayer(m_Layer);
            Logger.LogWarningFormat("Invalid avatar resource path:{0}!", avatarRes);
        }
        else
        {
            // 加载失败要第一时间释放被占用的layer
            GameClient.PlayerBaseData.GetInstance().ReleaseGeAvatarLayer(m_Layer);
            Logger.LogWarning("avatarRes can not be null or a null string!");
        }           
#endif
    }

    /// <summary>
    /// 禁用模型强制旋转脚本
    /// </summary>
    private void RemoveAvatarFixRotate(GeAvatar avatar)
    {
        //if (avatar == null) return;
        //var rootObj = avatar.GetAvatarRoot();
        //if (rootObj == null) return;
        //var component = rootObj.GetComponent<GeFixedRotate>();
        //if (component != null)
        //    component.enabled = false;
    }

    public void ClearAvatar()
    {
        if (null != m_Avatar)
            m_Avatar.ClearAll();

        //if(null != m_Attachment)
        //    m_Attachment.ClearAll();

        AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.SceneActor);
    }

    public string GetModelDataPath()
    {
        // return m_AvatarPath + "/" + m_AvatarName;
        return Path.ChangeExtension(m_AvatarPath, "asset");
    }

    public void ChangeAvatar(GeAvatarChannel eChannel, string modulePath, bool isAsync = true,bool highPriority = false)
    {
        DAssetObject assert = new DAssetObject(null, modulePath);
        ChangeAvatar(eChannel, assert,isAsync, highPriority);
    }

    public void ChangeAvatar(GeAvatarChannel eChannel, DAssetObject asset, bool isAsync = true, bool highPriority = false)
    {
        m_NeedInitAfterLoad = true;
        if (null != m_Avatar)
            m_Avatar.ChangeAvatarObject(eChannel, asset,isAsync, highPriority);
    }

    public void SuitAvatar(bool isAsync = true,bool highPriority = false)
    {
        m_NeedInitAfterLoad = true;
        if (null != m_Avatar)
            m_Avatar.SuitAvatar( isAsync, highPriority);
    }

    public GeAttach AttachAvatar(string attachmentName, string attachRes, string attachNode, bool needClear=true,bool asyncLoad = true,bool highPriority = false, float fAttHeight = 0.0f)
    {
        if (needClear)
        {
            if(m_Avatar != null)
            {
                m_Avatar.ClearAttachmentOnNode(attachNode);
            }
        }

        if(m_Avatar != null)
        {
            GeAttach att = m_Avatar.CreateAttachment(attachmentName, attachRes, attachNode, false, asyncLoad, highPriority);
            if (null != att)
            {
                att.SetLayer(m_Layer);

                if (fAttHeight != 0.0f)
                {
                    Vector3 pos = att.root.transform.localPosition;
                    pos.y = fAttHeight;
                    att.root.transform.localPosition = pos;
                }
            }

            /*特殊逻辑，如果有光环的话，Aureole隐藏*/
            if (attachmentName == "Aureole")
            {
                var att1 = m_Avatar.GetAttachment(Global.HALO_ATTACH_NAME);
                if (att1 != null && !att1.NeedDestroy)
                {
                    if (att != null)
                    {
                        att.SetVisible(false);
                    }
                }
            }

            return att;
        }

        return null;
    }

    public void RemoveAttach(GeAttach attachment)
    {
        m_Avatar.DestroyAttachment(attachment);
    }


    public GeEffectEx CreateEffect(string effectRes, string attachNode, float fTime, EffectTimeType timeType, Vector3 localPos, Quaternion localRot, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false)
    {
    return null;
    }
    
    public GeEffectEx CreateEffect(string effectPath, float fTime, Vector3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false, bool faceLeft = false,bool needResetLayer = false)
	{
		//effectInfo.effecttype

		DAssetObject asset;

		asset.m_AssetObj = null;
		asset.m_AssetPath = effectPath;

		return CreateEffect(asset, EffectsFrames.Default, fTime, initPos, initScale, fSpeed, isLoop, faceLeft, needResetLayer);
	}

	public GeEffectEx CreateEffect(DAssetObject effectRes,EffectsFrames info,float fTime,Vector3 initPos,float initScale = 1.0f,float fSpeed = 1.0f,bool isLoop = false,bool faceLeft = false, bool needResetLayer = false)
	{
		if (null != effectRes.m_AssetObj || (null != effectRes.m_AssetPath && "" != effectRes.m_AssetPath))
		{
			Vector3 graphicPos = initPos;
			GeEffectEx newEffect = m_Effect.AddEffect(effectRes, info,fTime, graphicPos,null,faceLeft);
			if (null != newEffect)
			{
#if !LOGIC_SERVER
                if (needResetLayer && newEffect.GetRootNode() != null)
                {
                    EffectLayerBackup effectLayerBackUp = new EffectLayerBackup();
                    effectLayerBackUp.effect = newEffect;
                    effectLayerBackUp.layer = newEffect.GetRootNode().layer;
                    m_EffectLayerBackUpList.Add(effectLayerBackUp);
                }
#endif

                newEffect.SetLayer(m_Layer);

				bool bLoop = false;
				if (info.timetype == EffectTimeType.GLOBAL_ANIMATION)
					bLoop = true;
				else
				{
					if (newEffect.GetDefaultTimeLen() < fTime)
						bLoop = true;
				}

				newEffect.SetSpeed(fSpeed);
				newEffect.Play(bLoop || isLoop);
				return newEffect;
			}
			else
				Logger.LogWarningFormat("Create effect [{0}] has failed!", effectRes.m_AssetPath);
		}
		else
			Logger.LogWarning("Effect resource path can not be null!");

		return null;
	}

    public void OnUpdate(float fDelta)
    {
    }

    public GeAttach GetAttachment(string name)
    {
        // if (m_Attachment != null)
        // {
        //     return m_Attachment.GetAttachment(name);
        // }
        // 
        // return null;
        if (m_Avatar == null)
            return null;

        return m_Avatar.GetAttachment(name);
    }

    public void ChangeAction(string actionName, float speed = 1.0f, bool loop = false)
    {
        if (m_Avatar == null)
            return;
        m_Avatar.ChangeAction(actionName, speed, loop);

		// if (m_Attachment != null)
		// 	m_Attachment.ChangeAction(actionName, speed, loop);
        // 
        // if (null != m_Animation)
        //     m_Animation.PlayAction(actionName, speed, loop);
    }

	public string GetCurActionName()
	{
		if (null != m_Avatar)
			return m_Avatar.GetCurActionName ();

		return "";
	}

    public bool IsCurActionEnd()
    {
        //if (null != m_Animation)
        //    return m_Animation.IsCurAnimFinished();
        //
        //return true;

        return m_Avatar.IsActionFinished();
    }

    public void RotateY(float degree)
    {
        Vector3 eular = m_AvatarRoot.transform.localEulerAngles;
        eular.y = degree;
        m_AvatarRoot.transform.localEulerAngles = eular;
    }

    public void AvatarPreRender(Camera cam)
    {
        if (cam == m_Camera)
        {
            if(m_LastAmbient != m_AmbientCol)
            {
                m_LastAmbient = m_AmbientCol;
                if (null != m_Avatar)
                    m_Avatar.SetAmbientColor(m_AmbientCol);
            }

            m_OriginAmbient = RenderSettings.ambientLight;
            RenderSettings.ambientLight = m_AmbientCol;

            if (m_EnableShadow)
            {
                Shader.SetGlobalInt("_EnableShadow", 1);
                if (m_ShadowCamera != null)
                {
                    Matrix4x4 lightSpaceMatrix = GL.GetGPUProjectionMatrix(m_ShadowCamera.projectionMatrix, true) * m_ShadowCamera.worldToCameraMatrix;
                    Shader.SetGlobalMatrix("_LightSpaceMatrix", lightSpaceMatrix);
                    Shader.SetGlobalVector("_LightViewThirdRow", m_ShadowCamera.worldToCameraMatrix.GetRow(2));
                    Shader.SetGlobalFloat("_LightFarPlane", m_ShadowCamera.farClipPlane);
                }

                if (m_ShadowRT != null)
                {
                    Shader.SetGlobalTexture("_ShadowMap", m_ShadowRT);
                }
            }
        }
    }
    public void AvatarPostRender(Camera cam)
    {
        if (cam == m_Camera)
        {
            RenderSettings.ambientLight = m_OriginAmbient;

            if (m_EnableShadow)
            {
                Shader.SetGlobalInt("_EnableShadow", 0);
            }
        }
    }

    // Use this for initialization
    void Start () {
	
	}

    void OnDestroy()
    {
        bHasSetLayer = false;
        GameClient.PlayerBaseData.GetInstance().ReleaseGeAvatarLayer(m_Layer);
        layer = -1;
        _Deinit();
    }

    void OnEnable()
    {
        _Deinit();
        _Init();
    }

    public void OnValidate()
    {
        _RefreshValues();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            m_CurRotate -= eventData.delta.x * 0.6f;
            RotateY(m_CurRotate);
        }
    }

    public Vector3 avatarPos
    {
        get { return m_AvatarRoot.transform.position; }
        set { if(null != m_AvatarRoot) m_AvatarRoot.transform.position = value; }
    }

    public Quaternion avatarRoation
    {
        get { return m_AvatarRoot.transform.rotation; }
        set { if (null != m_AvatarRoot) m_AvatarRoot.transform.rotation = value; }
    }

    public Vector3 avatarScale
    {
        get { return m_AvatarRoot.transform.localScale; }
        set { if (null != m_AvatarRoot) m_AvatarRoot.transform.localScale = value; }
    }

    public void OnLoaded(GeAvatar avatar)
    {
        GameObject avatarGo = avatar.GetAvatarRoot();
        if (null != avatarGo)
        {
            avatarGo.transform.SetParent(m_AvatarRoot.transform, false);
            avatarGo.SetActive(true);
            avatar.SetAvatarVisible(true);
            avatar.SetAmbientColor(m_AmbientCol);
        }
    }
#if !LOGIC_SERVER
    /// <summary>
    /// 销毁的时候重置特效的层级
    /// </summary>
    protected void ResetEffectLayer()
    {
        for(int i=0;i< m_EffectLayerBackUpList.Count; i++)
        {
            var backUpData = m_EffectLayerBackUpList[i];
            var effect = backUpData.effect;
            var backUplayer = backUpData.layer;
            if (effect != null && effect.GetRootNode() != null)
                effect.GetRootNode().SetLayer(backUplayer);
        }
        m_EffectLayerBackUpList.Clear();
    }
#endif
}
