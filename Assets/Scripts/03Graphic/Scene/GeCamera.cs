using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Tenmove.Runtime.Unity;

public struct GeCameraDesc
{
    public bool IsOrthographic { set; get; }
    public float FOV { set; get; }
    public float OrthographicSize { get; set; }
    public float NearPlane { get; set; }
    public float FarPlane { get; set; }
}

public class GeCamera
{
    // Camera节点
    private GameObject m_CameraNode;
    // Camera组件
    private Camera m_Camera;
    // 后处理组件
    private PostprocessLayer m_PostprocessLayer;

    private float m_OrthographicSize = 2;
    private float m_NearPlane = 0.1f;
    private float m_FarPlane = 50.0f;

    private GeCameraControllerScroll m_CameraController;
    private GeShockEffect m_ShockEffect = new GeShockEffect();

    private GeCameraDesc _defaultDesc;

    public GeCamera(GameObject mainCameraNode, Camera mainCamera)
    {
        m_CameraNode = mainCameraNode;
        m_Camera = mainCamera;

        if(m_CameraNode == null || m_Camera == null)
            Logger.LogError("MainCamera is null!");


        if (m_CameraNode)
        {
            if (!m_CameraNode.GetComponent<OnNpcHit>())
            {
                m_CameraNode.AddComponent<OnNpcHit>();
            }

            m_PostprocessLayer = m_CameraNode.GetComponent<PostprocessLayer>();
            if (m_PostprocessLayer != null)
                m_PostprocessLayer.Init();
        }

        if (Mathf.Abs(m_Camera.transform.localPosition.x +
            m_Camera.transform.localPosition.y +
            m_Camera.transform.localPosition.z) > 0.001f)
        {
            m_Camera.transform.localPosition = Vector3.zero;
            Logger.LogWarning("Camera's local position value must be zero!");
        }

        m_CameraController = Utility.FindComponent<GeCameraControllerScroll>("Environment/FollowPlayer");
        if (!m_CameraController.Init(this))
            Logger.LogError("Init camera controller has failed!");
    }
    
    public void InitCamera (float orthographicSize,float nearPlane,float farPlane,bool usePersp)
    {
        m_OrthographicSize = LeanTween.instance.cameraOrthoSize;//orthographicSize;
        m_NearPlane = nearPlane;
        m_FarPlane = farPlane;

        usePersp = false;
        
        if (usePersp)
        {
            m_Camera.fieldOfView = 10.0f;
            m_Camera.orthographic = false;
            m_Camera.orthographicSize = m_OrthographicSize;// levelData._CameraSize;
            m_Camera.nearClipPlane = m_NearPlane;
            m_Camera.farClipPlane = m_FarPlane;
        }
        else
        {
            /* 
            float rate2 = (float)Screen.width / (float)Screen.height;
            float size = m_OrthographicSize;
            if (!Global.Settings.heightAdoption)
                size = m_OrthographicSize * stdRatio / rate2;
            currentRatio = rate2;
            m_Camera.orthographicSize = size;// levelData._CameraSize;
            */
            float size = m_OrthographicSize;
            m_Camera.orthographicSize = size;//levelData._CameraSize;
            m_Camera.orthographic = true;
            m_Camera.nearClipPlane = m_NearPlane;
            m_Camera.farClipPlane = m_FarPlane;
        }

        m_ShockEffect.Init(m_CameraNode, 2);
    }

    public bool enabled
    {
        set
        {
            m_Camera.enabled = value;
            if (m_PostprocessLayer != null)
                m_PostprocessLayer.SetCameraEnabled(value);
        }
        get
        {
            return m_Camera.enabled;
        }
    }

    public float aspect
    {
        get { return m_Camera.aspect; }
    }

    public int pixelWidth
    {
        get { return m_Camera.pixelWidth; }
    }

    public int pixelHeight
    {
        get { return m_Camera.pixelHeight; }
    }

    public Vector3 position
    {
        get
        {
            if (m_Camera != null)
                return m_Camera.transform.position;
            else
                return Vector3.zero;
        }
    }

    public Vector3 localPosition
    {
        get
        {
            if (m_Camera != null)
                return m_Camera.transform.localPosition;
            else
                return Vector3.zero;
        }
    }

    public float orthographicSize
    {
        get { return m_Camera.orthographicSize; }
    }

    public void SetPosition(Vector3 position)
    {
        if (m_Camera != null)
        {
            m_Camera.transform.position = position;
        }
    }

    public void SetLocalPosition(Vector3 position)
    {
        if (m_Camera != null)
        {
            m_Camera.transform.localPosition = position;
        }
    }

    public void SetOrthographicSize(float size)
    {
        if (m_Camera != null)
        {
            m_OrthographicSize = size;
            m_Camera.orthographicSize = size;
        }
    }

    public void SetRenderTarget(RenderTexture renderTexture)
    {
        m_Camera.targetTexture = renderTexture;

        if(m_PostprocessLayer != null)
        {
            m_PostprocessLayer.SetRenderTarget(renderTexture);
        }
    }

    /// <summary>
    /// 通过SetTargetBuffers设置相机的Target时，使用BuiltinRenderTextureType.CameraTarget还是BackBuffer
    /// </summary>
    /// <param name="colorBuffer"></param>
    /// <param name="depthBuffer"></param>
    public void SetTargetBuffer(RenderTexture renderTexture)
    {
        if(renderTexture == null)
        {
            m_Camera.targetTexture = null;
        }
        else
        {
            m_Camera.SetTargetBuffers(renderTexture.colorBuffer, renderTexture.depthBuffer);
        }

        if(m_PostprocessLayer != null)
        {
            m_PostprocessLayer.SetRenderTarget(renderTexture);
        }
    }

    public void AddCommandBuffer(CameraEvent cameraEvent, CommandBuffer cmd)
    {
        if(cmd != null)
            m_Camera.AddCommandBuffer(cameraEvent, cmd);
    }

    public void RemoveCommandBuffer(CameraEvent cameraEvent, CommandBuffer cmd)
    {
        if (cmd != null)
            m_Camera.RemoveCommandBuffer(cameraEvent, cmd);
    }
    
    public Vector3 WorldToScreenPoint(Vector3 position)
    {
        if (m_Camera != null)
            return m_Camera.WorldToScreenPoint(position);
        else
            return position;
    }

    public Vector2 ProjectPositionToCanvas(Vector3 worldPos, RectTransform canvasRect)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_Camera, worldPos);

        Vector2 canvasPos;
        if (canvasRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, m_Camera, out canvasPos);
            return canvasPos;
        }
        else
        {
            Logger.LogWarning("Canvas object is null,project point into screen space!");
        }

        return screenPos;
    }


    public int shockTime = 0;
    // Update is called once per frame
    public void Update (int deltaTime)
    {
        if (m_ShockEffect != null)
        {
            m_ShockEffect.Update(deltaTime / 1000.0f);
        }

        if (shockTime > 0)
        {
            shockTime -= deltaTime;
        }

        /* 关闭适配
         float rate2 = (float)Screen.width / (float)Screen.height;  
         
         if( currentRatio != rate2 )
         {
             currentRatio = rate2;

            float size = m_OrthographicSize;
            if (!Global.Settings.heightAdoption)
                size = m_OrthographicSize * stdRatio / rate2;
             m_Camera.orthographicSize = size;// levelData._CameraSize;
         }
         */
    }
    
    #region 震动屏幕效果
    public void PlayShockEffect(float fTime, float fSpeed, float fXRange, float fYRange,int mode = -1,bool openShock = true)
    {
        if (!openShock) return;
        if(mode != -1)
        {
            m_ShockEffect.SetMode(mode);
        }
        m_ShockEffect.Start(fTime, fSpeed, fXRange, fYRange);
    }

    public void PlayShockEffect(float totalTime, int num, float xRange, float yRange, bool decelebrate, float xReduce, float yReduce, int mode, float radius = 1, bool openShock = true)
    {
        if (!openShock) return;
        m_ShockEffect.StartShock(totalTime, num, xRange, yRange, decelebrate, xReduce, yReduce, mode, radius);
    }

	public void PlayShockEffect(ShockData shockData)
	{
		PlayShockEffect(shockData.time, shockData.speed, shockData.xrange, shockData.yrange,2);
	}

    public int GetPlayerShockEffectMode()
    {
        if (null == m_ShockEffect)
        {
            return -1;
        }

        return m_ShockEffect.Mode;
    }

    public void SetPlayerShockEffectMode(int mode)
    {
        if (null == m_ShockEffect)
        {
            return;
        }

        m_ShockEffect.SetMode(mode);

    }
    #endregion

    public GeCameraControllerScroll GetController()
    {
        return m_CameraController;
    }

    public void SetLookDir(Vector3 orient)
    {
        m_CameraNode.transform.localRotation = Quaternion.Euler(orient);
    }

    public void SetCameraDesc(GeCameraDesc desc)
    {
        m_NearPlane = desc.NearPlane;
        m_FarPlane = desc.FarPlane;

        m_Camera.fieldOfView = desc.FOV;
        m_Camera.orthographic = desc.IsOrthographic;
        if (desc.IsOrthographic)
            m_Camera.orthographicSize = desc.OrthographicSize;
        else
            m_Camera.fieldOfView = desc.FOV;

        m_Camera.nearClipPlane = m_NearPlane;
        m_Camera.farClipPlane = m_FarPlane;
    }

    public GeCameraDesc GetCameraDesc()
    {
        return new GeCameraDesc()
        {
            NearPlane = m_Camera.nearClipPlane,
            FarPlane = m_Camera.farClipPlane,
            FOV = m_Camera.fieldOfView,
            OrthographicSize = m_Camera.orthographicSize,
            IsOrthographic = m_Camera.orthographic,
        };
    }

    public int GetCullingMask()
    {
        return m_Camera.cullingMask;
    }

    public void SetCullingMask(int mask)
    {
        m_Camera.cullingMask = mask;
    }

}
