using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class HGJoyStick : UIBehaviour,IBeginDragHandler,IDragHandler,IPointerEnterHandler,IPointerDownHandler,IPointerUpHandler
{
    public RectTransform thumbTransform;
	public RectTransform dirIndicator;
    protected RectTransform cachedTransform;
    protected Canvas cachedRootCanvas;
    bool bInMoving;
	bool hasDrag = false;

    public delegate void Del(int degree);
	public delegate void Del3(int degree, bool hasDrag);
	public delegate void Del2(int v1, int v2, int degree);

	public Del2 onMove;
	public Del3 onRelease;
	public Del onUpdate;

    public float radius = 200f;

    public bool onPointerEnter = false;
    public bool canRemoveJoystick = true;         //能否移除技能摇杆

    int degree = 0;
    float timeAcc = 0f;
    private CustomPointerEventData customEventData = null;

    private VInt2 _defaultOffset = VInt2.zero;
    public VInt2 DefaultOffset
    {
        set { _defaultOffset = value; }
    }

    public void Awake()
    {
        cachedRootCanvas = gameObject.GetComponentInParent<Canvas>();
        cachedTransform  = transform as RectTransform;
        customEventData = new CustomPointerEventData(EventSystem.current);
    }

    public void OnEnable()
    {
        onPointerEnter = false;
        timeAcc = 0;
        _defaultOffset = VInt2.zero;
        InitDirUI(0);
    }

    private void InitDirUI(int degree)
    {
        if (dirIndicator != null)
        {
            dirIndicator.localRotation = Quaternion.AngleAxis(degree, Vector3.forward);
        }
    }

    public void OnDisabled()
    {
        _defaultOffset = VInt2.zero;
    }

	void Update()
	{
        if (!onPointerEnter)
        {
            float delta = Time.deltaTime;
            timeAcc += delta;

            if (timeAcc >= 0.25f)
            {
                onPointerEnter = true;
                if (onRelease != null)
                {
                    onRelease(0, true);
                }
            }
        }

        

		if (onUpdate != null)
			onUpdate(1);
	}

    public void OnBeginDrag(PointerEventData data)
    {
        //UnityEngine.Debug.LogWarningFormat("OnBeginDrag");
    }
    
    public void OnDrag(PointerEventData data)
    {
        //UnityEngine.Debug.LogWarningFormat("OnDrag");
        //Logger.LogErrorFormat("onDrag");

        hasDrag = true;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedTransform,data.position,data.pressEventCamera,out pos);
        if(thumbTransform != null)
        {
            //添加技能摇杆的默认偏移地址 这边的计算和BeSkill里面的换算相关
            pos.x += _defaultOffset.x /GlobalLogic.VALUE_10000 * radius /GlobalLogic.VALUE_10;
            pos.y += _defaultOffset.y / GlobalLogic.VALUE_10000 * radius / GlobalLogic.VALUE_10;

            thumbTransform.anchoredPosition = pos;
            Vector2 dir = pos - Vector2.zero;
            

            Vector2 del = dir.normalized;
            degree = CalculateDegree(del);
			//Logger.LogErrorFormat("del({0},{1}) degree:{2} sqrMagnitude:{3}", del.x, del.y, degree, del.sqrMagnitude);

			if (dirIndicator != null)
			{
				dirIndicator.localRotation = Quaternion.AngleAxis(degree, Vector3.forward);
			}

			//Logger.LogErrorFormat("dir({0},{1}), magnitude:{2}, sqrMagnitude:{3} radius{4}", dir.x/radius, dir.y/radius, dir.magnitude, dir.sqrMagnitude, radius);
            if (dir.magnitude > radius)
            {

                float l = del.magnitude;
                float redians = Mathf.Acos(del.x / l);

                pos.y = Mathf.Sin(redians) * radius;
                pos.x = Mathf.Cos(redians) * radius;

                if (del.y < 0)
                    pos.y *= -1;

                thumbTransform.anchoredPosition = pos;
            }

			dir = thumbTransform.anchoredPosition;
			dir.x = Mathf.Clamp(dir.x, -radius, radius);
			dir.y = Mathf.Clamp(dir.y, -radius, radius);

			int vx = (int)(dir.x/radius * GlobalLogic.VALUE_1000);
			int vy = (int)(dir.y/radius * GlobalLogic.VALUE_1000);

			if (onMove != null)
			{
				onMove(vx, vy, degree);
				//Logger.LogErrorFormat("onmove({0},{1})", vx, vy);
			}
        }
    }
    
    public void OnPointerEnter(PointerEventData data)
    {
        onPointerEnter = true;

        //UnityEngine.Debug.LogWarningFormat("OnPointerEnter");
        if (data.pointerPress != null)
        {
            customEventData.CopyFrom(data);
            customEventData.customData = 1;
            ExecuteEvents.Execute(data.pointerPress, customEventData, ExecuteEvents.pointerUpHandler);
        }


        data.pointerDrag = gameObject;
        data.pointerPress = gameObject;

		hasDrag = false;
// 
        if (!data.eligibleForClick)
        {
            if (onRelease != null)
            {
                onRelease(0, true);
            }
        }

        //UnityEngine.Debug.LogWarningFormat("[HGJoyStick]OnPointerEnter touch id {0}\n",data.pointerId);
    }
    
    public void OnPointerDown(PointerEventData data)
    {
       // UnityEngine.Debug.LogWarningFormat("OnPointerDown");
    }
    
    public void OnPointerUp(PointerEventData data)
    {
        CustomPointerEventData customData = data as CustomPointerEventData;
        if (customData != null && customData.customData == 1)
        {
            return;
        }
        Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedTransform,data.position,data.pressEventCamera,out pos);
		if(thumbTransform != null)
		{
			Vector2 dir = pos;
			Vector2 del = dir.normalized;
			degree = CalculateDegree(del);
		}

        // UnityEngine.Debug.LogWarningFormat("[HGJoyStick]OnPointerUp touch id {0}\n",data.pointerId);

        //UnityEngine.Debug.LogWarningFormat("OnPointerUp");

        //���һ���Ƿ�����Ƴ��ı�־
        if (canRemoveJoystick)
        {
            thumbTransform.anchoredPosition = Vector2.zero;       
        }

        if (onRelease != null)
        {
            onRelease(degree, hasDrag);
        }

        _defaultOffset = VInt2.zero;
    }
    public short CalculateDegree(Vector2 dir)
    {
        short nDegree = 0;
        float mx = dir.x;
        float my = dir.y;
        if (Mathf.Abs(mx) > 0.00001f || Mathf.Abs(my) > 0.00001f)
        {
            bInMoving = true;
        }

        if (Mathf.Abs(mx) > 0.00001f || Mathf.Abs(my) > 0.00001f)
        {
            float l = Mathf.Sqrt(mx * mx + my * my);
            float redians = Mathf.Acos(mx / l);
            nDegree = (short)(Mathf.Rad2Deg * redians);
            if (my < -0f)
            {
                nDegree = (short)(360 - nDegree);
            }
            bInMoving = true;
        }

        return nDegree;
    }
}
