using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using Spine.Unity;
using System.Collections.Generic;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(CanvasRenderer))]
public class GeObjectRenderer : MonoBehaviour
{
    public string m_RenderTexName = "DeftAvatarRend";

    public Vector3 m_LightRot = new Vector3();
    public Color m_LightCol = Color.white;
    public Color m_AmbientCol = Color.black;
    public Color m_OriginAmbient = Color.black;

    public float m_OrthoSize = 1.2f;
    public Vector3 m_CameraRot = new Vector3(15, 0, 0);
    public Vector3 m_CameraPos = new Vector3(0, 1, -1);
    public float m_NearPlane = 0.3f;
    public float m_FarPlane = 10.0f;
    public float m_FOV = 45.0f;
    public bool m_IsPersp = false;
    public int m_Layer = 30;

    public int m_TextureWidth = 619;
    public int m_TextureHight = 817;

    public float m_RotateDegree = 10;

    public string m_AvatarPath = "";
    public string m_AvatarName = "";

    protected Light m_Light = null;
    protected GameObject m_Root = null;
    protected Camera m_Camera = null;

    protected IGeRenderTexture m_RenderTexture = null;

    protected GameObject m_AvatarRoot = null;
    protected GameObject m_Object = null;
	protected GameObject m_ObjectWrap = null;
    protected GeAttachManager m_Attachment = null;

    private string m_spinePath = string.Empty;

    public GameObject LoadedGameObject
    {
        get {return m_Object;}
    }


    protected float m_CurRotate = 0.0f;

    protected void _Init()
    {
        if (null == m_AvatarRoot)
            m_AvatarRoot = new GameObject("AvatarRender");
        
        if (null == m_Attachment)
            m_Attachment = new GeAttachManager();

        if (m_TextureWidth < 1)
            m_TextureWidth = 1;
        if (m_TextureHight < 1)
            m_TextureHight = 1;

        if (null == m_RenderTexture)
        {
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

                    return;
                }
                else
                    Logger.LogWarning("Get camera form render texture has failed!");
            }
            else
                Logger.LogWarning("create render texture has failed!");
        }
    }

    protected void _RefreshValues()
    {
        if (null != m_Camera)
        {
            m_Camera.transform.eulerAngles = m_CameraRot;
            m_Camera.transform.position = m_CameraPos;

            m_Camera.orthographic = !m_IsPersp;
            m_Camera.orthographicSize = m_OrthoSize;
            m_Camera.fieldOfView = m_FOV;
            m_Camera.nearClipPlane = m_NearPlane;
            m_Camera.farClipPlane = m_FarPlane;
        }

        if (null != m_Light)
        {
            m_Light.color = m_LightCol;
            m_Light.transform.eulerAngles = m_LightRot;
        }
    }

    protected void _Deinit()
    {
        if (null != m_RenderTexture)
        {
            GeRenderTextureManager.instance.DestroyRenderTexture(m_RenderTexture);

            m_RenderTexture = null;
            m_Light = null;
            m_Camera = null;
            m_Root = null;
        }

        if (null != m_Attachment)
        {
            m_Attachment.Deinit();
            m_Attachment = null;
        }

        if (null != m_Object)
        {
            GameObject.Destroy(m_Object);
            m_Object = null;
        }

		if (m_ObjectWrap != null)
		{
			GameObject.Destroy(m_ObjectWrap);
			m_ObjectWrap = null;
		}

        if (m_AvatarRoot)
        {
            GameObject.Destroy(m_AvatarRoot);
            m_AvatarRoot = null;
        }

        Camera.onPreRender -= AvatarPreRender;
        Camera.onPostRender -= AvatarPostRender;
    }

    public void SetCameraPos(Vector3 vector3)
    {
        if (m_CameraPos != null)
        {
            m_CameraPos = vector3;
        }
    }

	public void LoadObject(string objRes, int layer = -1, string wrapObjectPath=null)
    {
        if (gameObject.activeSelf == false)
            return;

        if (-1 != layer)
            m_Layer = layer;
        
        _Init();
        if (!string.IsNullOrEmpty(objRes))
        {
            m_spinePath = objRes;
            m_Object = AssetLoader.instance.LoadResAsGameObject(objRes);
            if (null != m_Object)
            {
                Renderer[] ar = m_Object.GetComponentsInChildren<Renderer>();
                for(int i = 0 , icnt = ar.Length;i<icnt;++i)
                {
                    if(null == ar[i]) continue;
                    ar[i].gameObject.layer = m_Layer;
                }

                m_Attachment.RefreshAttachNode(m_Object);

				Animation comAnimation = m_Object.GetComponent<Animation>();
				if (comAnimation != null)
					comAnimation.Stop();

				if (wrapObjectPath != null)
				{
					var wrapObject = AssetLoader.instance.LoadResAsGameObject(wrapObjectPath);
					if (wrapObject != null)
					{
						Utility.AttachTo(m_Object, Utility.FindGameObject(wrapObject, "root", false));
						m_ObjectWrap = wrapObject;
					}
				}



                return;
            }
            else
                Logger.LogErrorFormat("Load object resource has failed with resource path {0}!", objRes);
        }
        else
            Logger.LogWarning("objRes can not be null or a null string!");
    }

    public bool IsSpinePathEquals(string path)
    {
        return m_spinePath.Equals(path);
    }

    public void ClearObject()
    {
        if (null != m_Attachment)
            m_Attachment.ClearAll();

        GameObject.Destroy(m_Object);
        m_Object = null;
    }

    /// <summary>
    /// 按照名字顺序播放spine动画
    /// </summary>
    /// <param name="names"></param>
    public void AddAni(List<string> names)
    {
        if (m_Object == null)
        {
            return;
        }

        if (names == null)
        {
            return;
        }

        SkeletonAnimation skeletonAnimation = m_Object.GetComponent<SkeletonAnimation>();
        
        if (skeletonAnimation == null)
        {
            return;
        }

        Spine.AnimationState state = skeletonAnimation.state;
        if (state == null)
        {
            return;
        }

        bool isSet = true;
        foreach (var name in names)
        {
            if (skeletonAnimation.HasAnimation(name))
            {
                if (isSet)
                {
                    bool isLoop = names.Count == 1;
                    state.SetAnimation(0, name, isLoop);
                    isSet = false;
                }
                else
                {
                    state.AddAnimation(0, name, true, 0);
                }
            }
        }
    }

    public GeEffectEx CreateEffect(string effectRes, string attachNode, float fTime, EffectTimeType timeType, Vector3 localPos, Quaternion localRot, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false)
    {
        return null;
    }

    public void OnUpdate(float fDelta)
    {
        RotateY(m_RotateDegree);
    }

    public void SetLocalScale(float scale)
    {
        if (null == m_Object)
        {
            return;
        }

        Vector3 originScale = m_Object.transform.localScale;
        originScale *= scale;
        m_Object.transform.localScale = originScale;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        if (null == m_Object)
        {
            return;
        }

        m_Object.transform.localPosition = pos;
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
            m_OriginAmbient = RenderSettings.ambientLight;
            RenderSettings.ambientLight = m_AmbientCol;
        }
    }
    public void AvatarPostRender(Camera cam)
    {
        if (cam == m_Camera)
        {
            RenderSettings.ambientLight = m_OriginAmbient;
        }
    }

	public void ChangePhase(string phaseEffect,int phaseIdx,bool forceAddtive = false)
	{
		GeAttach attach = new GeAttach(m_Object);
		if (attach != null)
		{
			attach.ChangePhase(phaseEffect, phaseIdx);
			m_Attachment.GetAttachList().Add(attach);
		}
	}

    // Use this for initialization
    void Start()
    {

    }

    void OnDestroy()
    {
        _Deinit();
    }

    void OnEnable()
    {
    }

    public void OnValidate()
    {
        _RefreshValues();
    }
}
