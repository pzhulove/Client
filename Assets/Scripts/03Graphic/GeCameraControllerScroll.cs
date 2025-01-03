using UnityEngine;
using System.Collections;

public class GeCameraControllerScroll : MonoBehaviour
{
    //public Parameter;
    public float m_From = -500;
    public float m_To = 500;
    public float m_Limit = 8;
    public float m_FollowThreshold = 1;
    public float m_UnfollowThreshold = 0.05F;
    public bool m_UsePreSetting = true;

    public float m_SmoothTime = 0.3F;

    private Vector3 Velocity = new Vector3(0.0F, 0.0F, 0.0F);

    /// <summary>
    /// 摄像机类
    /// </summary>
    private GeCamera m_Camera = null;
    /// <summary>
    /// 摄像机控制器挂载的实体
    /// </summary>
    private GeEntity m_Entity = null;
    /// <summary>
    /// 摄像机控制器当前的位置（）
    /// </summary>
    public Vector3 m_CtrlOffset = new Vector3(0.0F, 0.0F, 0.0F);
    /// <summary>
    /// 摄像机的活动范围限制
    /// </summary>
    private Vector3 m_OriginMinLimitRange = new Vector3(-100000.0F, -100000.0F, -100000.0F);    // 设置的初始范围
    private Vector3 m_OriginMaxLimitRange = new Vector3( 100000.0F,  100000.0F,  100000.0F);    // 设置的初始范围
    private Vector3 m_MinLimitRange = new Vector3(-100000.0F, -100000.0F, -100000.0F);          // 适配后的范围
    private Vector3 m_MaxLimitRange = new Vector3(100000.0F, 100000.0F, 100000.0F);             // 适配后的范围
    private float m_MinXLimitOffset = 0.0f;
    private float m_MaxXLimitOffset = 0.0f;

    private float m_CurAspect = 1.0f;

    /// <summary>
    /// 摄像机跟随限制 超过该值摄像机 开始跟随
    /// </summary>
    private Vector3 m_MotorLimit = new Vector3(0.0F, 0.0F, 0.0F);

    private float m_Distance = 10.0f;
    private float m_LookHeight = 1.0f;
    private float m_PitchAngle = 10.0f;

    protected bool[] m_LockAxis = new bool[3];
    
    protected float m_MoveTimeLen = 0.0f;
    protected float m_MoveTimePos = 0.0f;
    protected Vector3 m_TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
	protected float offset = 0f;
	protected float targetOffset = 0f;

	protected bool pauseUpdate = false;

    public bool Init(GeCamera camera)
    {
        if (null == camera)
            return false;

        for(int i = 0; i < 3; ++ i)
        {
            m_LockAxis[i] = false;
        }

        m_Camera = camera;

        pauseUpdate = false;

        return true;
    }

    public void AttachTo(GeEntity entity,float lookHeight,float pitchAngle,float distance)
    {
        m_Distance = distance;
        m_LookHeight = lookHeight;
        m_PitchAngle = pitchAngle;

        if (null != m_Camera)
            m_Camera.SetLookDir(new Vector3(pitchAngle, 0, 0));

        m_CtrlOffset = new Vector3(0, m_LookHeight, 0);
        m_CtrlOffset.y += Mathf.Sin(pitchAngle * Mathf.Deg2Rad) * distance;
        m_CtrlOffset.z -= Mathf.Cos(pitchAngle * Mathf.Deg2Rad) * distance;
        
        m_Entity = entity;

        ForceUpdate();
    }

    public void SetLimitRange(float xMinLimit, float yMinLimit, float zMinLimit, float xMaxLimit, float yMaxLimit, float zMaxLimit)
    {
        m_OriginMinLimitRange.x = xMinLimit;
        m_OriginMinLimitRange.y = yMinLimit;
        m_OriginMinLimitRange.z = zMinLimit;
        m_OriginMaxLimitRange.x = xMaxLimit;
        m_OriginMaxLimitRange.y = yMaxLimit;
        m_OriginMaxLimitRange.z = zMaxLimit;
        m_MinLimitRange.x = xMinLimit;
        m_MinLimitRange.y = yMinLimit;
        m_MinLimitRange.z = zMinLimit;
        m_MaxLimitRange.x = xMaxLimit;
        m_MaxLimitRange.y = yMaxLimit;
        m_MaxLimitRange.z = zMaxLimit;
        // 适配相机范围
        _UpdateCameraRange();

        m_LockAxis[0] = xMinLimit == xMaxLimit;
        m_LockAxis[1] = yMinLimit == yMaxLimit;
        m_LockAxis[2] = zMinLimit == zMaxLimit;
    }

    private void _UpdateCameraRange()
    {
        if (m_Camera != null)
        {
            float orthoGraphicSize = m_Camera.orthographicSize;

            float offsetY = 3.05f - orthoGraphicSize;
            float aspect = (float)m_Camera.pixelWidth / m_Camera.pixelHeight;
            float offsetX = offsetY * aspect;

            float resolutionOffsetX = ((16f / 9f) - aspect) * 3.05f;

            offsetX += resolutionOffsetX;

            m_MinLimitRange.x = m_OriginMinLimitRange.x - offsetX;
            m_MaxLimitRange.x = m_OriginMaxLimitRange.x + offsetX;

            m_MinLimitRange.y = m_OriginMinLimitRange.y - offsetY;
            m_MaxLimitRange.y = m_OriginMaxLimitRange.y + offsetY;

            if (m_MinLimitRange.x >= m_MaxLimitRange.x)
            {
                m_MinLimitRange.x = m_MaxLimitRange.x = (m_OriginMinLimitRange.x + m_OriginMaxLimitRange.x) / 2;
            }
            if (m_MinLimitRange.y >= m_MaxLimitRange.y)
            {
                m_MinLimitRange.y = m_MaxLimitRange.y = (m_OriginMinLimitRange.y + m_OriginMaxLimitRange.y) / 2;
            }
        }
    }

    public void SetMotorLimit(float xMotorLimit, float yMotorLimit, float zMotorLimit)
    {
        m_MotorLimit.x = xMotorLimit;
        m_MotorLimit.y = yMotorLimit;
        m_MotorLimit.z = zMotorLimit;
    }

    // Use this for initialization
    void Start ()
    {
	
	}

    public void MoveCamera(float off, float timelen)
    {
        if(timelen > 0.0f)
        {
            //if (offset != 0)
            //{
            //    ResetData();                //加快恢复摄像机
            //}
            targetOffset = off;

            m_MoveTimeLen = timelen;
            m_MoveTimePos = 0.0f;
        }
    }

	public float GetOffset()
	{
		return offset;
	}

    public void ForceUpdate()
    {
        LateUpdate();
    }

    // Late update is called once per frame
    private void LateUpdate()
    {
        UpdateCameraPull();
        if (pauseUpdate)
			return;
		
        if (null == m_Entity)
            return;

        _UpdateXOffsetLimit();

		float newOffset = offset;

        Vector3 targetPos;
        if (m_MoveTimePos < m_MoveTimeLen)
        {
            m_MoveTimePos += UnityEngine.Time.deltaTime;
			targetPos = gameObject.transform.localPosition;
			float tmp = m_MoveTimePos / m_MoveTimeLen * targetOffset;

			if (targetOffset > 0)
				tmp = Mathf.Min(tmp, targetOffset);
			if (targetOffset < 0)
				tmp = Mathf.Max(tmp, targetOffset);

			newOffset = offset + tmp;
        }
        else
        {
			if (m_MoveTimeLen > 0.0f)
			{
				offset += targetOffset;
				newOffset = offset;
				m_MoveTimeLen = 0.0f;
			}
				
            targetPos = gameObject.transform.localPosition;
        }

        Vector3 entPos = m_Entity.GetPosition();
		entPos.x += newOffset;

        float xdelta = (entPos.x - targetPos.x);
        xdelta = xdelta - Mathf.Clamp(xdelta, -m_MotorLimit.x, m_MotorLimit.x);
        float destx = xdelta + targetPos.x;

        float ydelta = (entPos.y - targetPos.y);
        ydelta = ydelta - Mathf.Clamp(ydelta, -m_MotorLimit.y, m_MotorLimit.y);
        float desty = ydelta + targetPos.y;

        float zdelta = (entPos.z - targetPos.z);
        zdelta = zdelta -Mathf.Clamp(zdelta, -m_MotorLimit.z, m_MotorLimit.z);
        float destz = zdelta + targetPos.z;

        if (!m_LockAxis[0])
            targetPos.x = Mathf.Clamp(destx + m_CtrlOffset.x, m_MinLimitRange.x + m_MinXLimitOffset, m_MaxLimitRange.x + m_MaxXLimitOffset);
        else
            targetPos.x = m_CtrlOffset.x;

        if (!m_LockAxis[1])
            targetPos.y = Mathf.Clamp(desty + m_CtrlOffset.y, m_MinLimitRange.y, m_MaxLimitRange.y);
        else
            targetPos.y = m_CtrlOffset.y;

        if (!m_LockAxis[2])
            targetPos.z = Mathf.Clamp(destz + m_CtrlOffset.z, m_MinLimitRange.z, m_MaxLimitRange.z);
        else
            targetPos.z = m_CtrlOffset.z;


        gameObject.transform.localPosition = targetPos;
    }

    public void SetCameraPos(Vector3  offset)
    {
        offset.x = Mathf.Clamp(offset.x, m_MinLimitRange.x + m_MinXLimitOffset, m_MaxLimitRange.x + m_MaxXLimitOffset);
        gameObject.transform.localPosition = offset;
    }
    public void SetCameraPosition(Vector3 offset)
    {
        gameObject.transform.localPosition = offset;
    }

    public Vector3 GetCameraPosition()
    {
        return gameObject.transform.localPosition;
    }

    //相机是否已经移动到场景的左边缘
    public bool IsInSceneLeftEdge()
    {
        float offsetX = gameObject.transform.localPosition.x;
        return offsetX <= m_MinLimitRange.x + m_MinXLimitOffset;
    }

    //相机是否已经移动到场景的右边缘
    public bool IsInSceneRightEdge()
    {
        float offsetX = gameObject.transform.localPosition.x;
        return offsetX >= m_MaxLimitRange.x + m_MaxXLimitOffset;
    }

    void _UpdateXOffsetLimit()
    {
        if (null != m_Camera)
        {
            if (m_Camera.aspect != m_CurAspect)
            {
                /* 
                if (Global.Settings.heightAdoption)
                {
                    float stdAspect = 16.0f / 9.0f;
                    float curAspect = camera.aspect;

                    m_MinXLimitOffset = -(stdAspect - curAspect) * camera.orthographicSize;
                    m_MaxXLimitOffset = (stdAspect - curAspect) * camera.orthographicSize;
                }
                else
                {
                    m_MinXLimitOffset = 0.0f;
                    m_MaxXLimitOffset = 0.0f;
                }
                */
                m_CurAspect = m_Camera.aspect;
            }
        }
    }

    //当前正在移动摄像头的过程中再次调用移动函数
    protected void ResetData()
    {
        if (m_MoveTimePos == 0 || m_MoveTimeLen == 0)
            return;
        m_MoveTimePos = m_MoveTimeLen - Time.deltaTime;
        //用于模拟最后一次移动
        LateUpdate();
        //用于移动完成后的数据重置
        LateUpdate();
    }

    public void SetPause(bool flag)
	{
		pauseUpdate = flag;
	}

    private Vector3 pullTargetPos = Vector3.zero;
    private float pullSpeed = 0;
    private float pullTargetSize = 0;

    private int deletaTime = 32;

    private bool isPulling = false;
    private int time = 0;
    private float originalSize = 0; 

    /// <summary>
    /// 开始镜头拉伸
    /// </summary>
    public void StartCameraPull(Vector3 targetPos,float speed, float targetSize)
    {
        if (!isPulling)
        {
            originalSize = m_Camera.orthographicSize;
        }
        
        pullTargetPos = targetPos;
        pullSpeed = speed;
        pullTargetSize = targetSize;

        time = 0;
        isPulling = true;
    }

    /// <summary>
    /// 镜头拉伸
    /// </summary>
    private void UpdateCameraPull()
    {
        if (!isPulling)
            return;
        time += deletaTime;

     //   if(pullTargetPos != Vector3.zero)
        {
            Vector3 newPosition = Vector3.Lerp(m_Camera.localPosition, pullTargetPos, time / 1000.0f * pullSpeed);
            m_Camera.SetLocalPosition(newPosition);
        }

        m_Camera.SetOrthographicSize(Mathf.Lerp(m_Camera.orthographicSize, pullTargetSize, time / 1000.0f * pullSpeed));
        _UpdateCameraRange();

        if (m_Camera.localPosition == pullTargetPos && m_Camera.orthographicSize == pullTargetSize)
        {
            isPulling = false;
        }
    }

    /// <summary>
    /// 恢复镜头拉伸
    /// </summary>
    public void RestoreCameraPull(float speed)
    {
        if (speed == 0)
        {
            time = 0;
            isPulling = false;
            m_Camera.SetOrthographicSize(originalSize);
            m_Camera.SetLocalPosition(Vector3.zero);
            _UpdateCameraRange();
        }
        else
        {
            pullTargetPos = Vector3.zero;
            pullSpeed = speed;
            pullTargetSize = originalSize;
            time = 0;
            isPulling = true;
        }
    }

    public void ResetCamera()
    {
        m_Camera.SetLocalPosition(Vector3.zero);
    }
}
