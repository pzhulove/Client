using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public class BangFullScreenParam
{
    /// <summary>
    /// 对应的屏幕分辨率类型
    /// </summary>
    public CameraAspectAdjust.eScreenType screenType = CameraAspectAdjust.eScreenType.None;
    /// <summary>
    /// 左边刘海偏移
    /// </summary>
    public Vector3 landLeftOffset;
    /// <summary>
    /// 右边刘海偏移
    /// </summary>
    public Vector3 landRightOffset;
    /// <summary>
    /// 对比iPhoneX的刘海的叠加偏移值 左右横屏叠加偏移一致
    /// </summary>
    [Header("对比iPhoneX的刘海的叠加偏移值,对iPhoneX无效")]
    public Vector3 bangOffset;
}

[ExecuteAlways]
public class ComUIiPhoneXAspect : MonoBehaviour 
{
    [Header("左横屏-iPhoneX配置刘海的偏移值")]
    public Vector3 landLeftOffset;
    [Header("右横屏-iPhoneX配置刘海的偏移值")]
    public Vector3 landRightOffset;

    [Header("不同分辨率配置刘海偏移的参数")]
    public List<BangFullScreenParam> landScreenOffsetList = new List<BangFullScreenParam>();
	public ScreenOrientation mScreenOrientation 		  = ScreenOrientation.Unknown;

    private Vector3 mOriginLocalPostion = Vector3.zero;
	private bool 			  mIsScreenOrientationDirty  = false;
	private RectTransform     mRectTransform 	         = null;
    private DOTweenAnimation mDotweenAnimation = null;
    private CameraAspectAdjust.eScreenType mScreenType = CameraAspectAdjust.eScreenType.None;

    void Awake()
	{
        mScreenType = CameraAspectAdjust.GetScreenType();

        if (!IsFullScreenType())
            return;

        _init();
        _updatePositionByLandType();
    }

	void Start()
	{
	}
	private void _init()
	{
		mRectTransform = this.GetComponent<RectTransform>();
        mDotweenAnimation = this.GetComponent<DOTweenAnimation>();

        if (null != mRectTransform)
		{
			mOriginLocalPostion = mRectTransform.anchoredPosition;
		}
		else
		{
			mOriginLocalPostion = this.transform.localPosition;
		}

		mScreenOrientation  	  = Screen.orientation;
		mIsScreenOrientationDirty = false;
	}

	

	private void _updatePositionByLandType()
	{
        if (!IsFullScreenType())
            return;

        Vector3 targetPos = Vector3.zero;

		switch (mScreenOrientation)
		{
			case ScreenOrientation.LandscapeLeft:
                targetPos = mOriginLocalPostion + GetLandScreenOffsetByScreenType(mScreenOrientation);
				break;
			case ScreenOrientation.LandscapeRight:
                targetPos = mOriginLocalPostion + GetLandScreenOffsetByScreenType(mScreenOrientation);
				break;
			default:
				targetPos = mOriginLocalPostion;
				break;
		}

		if (null != mRectTransform)
		{
			mRectTransform.anchoredPosition = targetPos;
		}
		else
		{
			this.transform.localPosition = targetPos;
		}
	}

#if UNITY_EDITOR
	public void ForceUpdateHandLeft()
	{
		mScreenOrientation = ScreenOrientation.LandscapeLeft;
        Global.screenOrientation = mScreenOrientation;
		_updatePositionByLandType();
	}

	public void ForceUpdateHandRight()
	{
		mScreenOrientation = ScreenOrientation.LandscapeRight;
        Global.screenOrientation = mScreenOrientation;
		_updatePositionByLandType();
	}

	public void ForceUpdateHandNone()
	{
		mScreenOrientation = ScreenOrientation.Unknown;
        Global.screenOrientation = mScreenOrientation;
		_updatePositionByLandType();
    }
	
	/// <summary>
    /// 用ipx的参数初始化其他分辨率的默认值
    /// </summary>
    public void InitOriginaliPhoneXParam()
    {
        if (landScreenOffsetList == null)
        {
            return;
        }
        for (int j = (int)CameraAspectAdjust.eScreenType.R_iPhoneX_2436_1125; j < (int)CameraAspectAdjust.eScreenType.R_Other; j++)
        {
            var _screenType = (CameraAspectAdjust.eScreenType)j;
            bool hasSet = false;
            BangFullScreenParam otherScreenParam = null;
            otherScreenParam = landScreenOffsetList.Find(
                    (BangFullScreenParam param) => {

                        return param.screenType == CameraAspectAdjust.eScreenType.R_Other;
                    }
                );
            if (otherScreenParam != null)
            {
                //尝试移除无效配置
                landScreenOffsetList.Remove(otherScreenParam);
            }

            for (int i = 0; i < landScreenOffsetList.Count; i++)
            {
                if (landScreenOffsetList[i].screenType == _screenType)
                {
                    hasSet = true;
                    break;
                }
            }
            if (!hasSet)
            {
                landScreenOffsetList.Add(new BangFullScreenParam()
                {
                    screenType = _screenType,
                    landLeftOffset = this.landLeftOffset,
                    landRightOffset = this.landRightOffset
                });
            }
        }
    }
#endif
    bool IsInScreen(Vector3 pos)
    {
        return pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height;
    }

    bool IsDotweenAnimationMoving()
    {
        if (mDotweenAnimation != null && mDotweenAnimation.tween != null)
        {
            bool ret = mDotweenAnimation.tween.IsPlaying();
         //   Logger.LogErrorFormat("name:{0} ret:{1}", gameObject.name, ret);

            return ret;
        }

        return false;
    }

#if !UNITY_EDITOR
	void Update()
	{
        if (!IsFullScreenType())
            return;

        if (IsDotweenAnimationMoving())
        {
            mIsScreenOrientationDirty = true;
        }
        

		if (mScreenOrientation != Screen.orientation || mIsScreenOrientationDirty)
		{
             var camera = GameClient.ClientSystemManager.GetInstance().UICamera;
            if (camera != null)
            {
                var screenPos = camera.WorldToScreenPoint(gameObject.transform.position);
            //    Logger.LogErrorFormat("name:{3} screenPos:{0} Screen:{1},{2}", screenPos, Screen.width, Screen.height, gameObject.name);

                if (!IsInScreen(screenPos) || IsDotweenAnimationMoving())
                    return;
            }

			mScreenOrientation 		  = Screen.orientation;
			mIsScreenOrientationDirty = true;
		}

		if (mIsScreenOrientationDirty)
		{
			_updatePositionByLandType();
			mIsScreenOrientationDirty = false;
		}
	}
#else
    void Update()
	{
        if (!IsFullScreenType())
            return;

        if (IsDotweenAnimationMoving())
        {
            mIsScreenOrientationDirty = true;
        }

        if (Global.screenOrientation != mScreenOrientation || mIsScreenOrientationDirty)
        {
            var camera = GameClient.ClientSystemManager.GetInstance().UICamera;
            if (camera != null)
            {
                var screenPos = camera.WorldToScreenPoint(gameObject.transform.position);
               // Logger.LogErrorFormat("name:{3} screenPos:{0} Screen:{1},{2}", screenPos, Screen.width, Screen.height, gameObject.name);

                if (!IsInScreen(screenPos) || IsDotweenAnimationMoving())
                    return;
            }

            mScreenOrientation = Global.screenOrientation;
            mIsScreenOrientationDirty = true;
        }
        if (mIsScreenOrientationDirty)
        {
            _updatePositionByLandType();
            mIsScreenOrientationDirty = false;
        }
	}
#endif

    private bool IsFullScreenType()
    {
        if (mScreenType == CameraAspectAdjust.eScreenType.R_BangFullScreen_2240_1080 ||
            mScreenType == CameraAspectAdjust.eScreenType.R_BangFullScreen_2244_1080 ||
            mScreenType == CameraAspectAdjust.eScreenType.R_BangFullScreen_2280_1080 ||
            mScreenType == CameraAspectAdjust.eScreenType.R_BangFullScreen_2208_1080 ||
            mScreenType == CameraAspectAdjust.eScreenType.R_16_9_2 ||
            mScreenType == CameraAspectAdjust.eScreenType.R_iPhoneX_2436_1125)
        {
            return true;
        }
        return false;
    }

    private bool IsiPhoneXScreenType()
    {
        if (mScreenType == CameraAspectAdjust.eScreenType.R_iPhoneX_2436_1125)
        {
            return true;
        }
        return false;
    }

    private Vector3 GetLandScreenOffsetByScreenType(ScreenOrientation direction)
    {
        if (landScreenOffsetList == null)
        {
            return Vector3.zero;
        }
        for (int i = 0; i < landScreenOffsetList.Count; i++)
        {
            var screenType = landScreenOffsetList[i].screenType;
            if (screenType == /*mScreenType*/CameraAspectAdjust.eScreenType.R_iPhoneX_2436_1125)
            {
                if (true || IsiPhoneXScreenType())
                {
                    landScreenOffsetList[i].bangOffset = Vector3.zero;
                }
                if (direction == ScreenOrientation.LandscapeLeft)
                {
                    return landScreenOffsetList[i].landLeftOffset + landScreenOffsetList[i].bangOffset;
                }
                else if (direction == ScreenOrientation.LandscapeRight)
                {
                    return landScreenOffsetList[i].landRightOffset + landScreenOffsetList[i].bangOffset;
                }
            }
        }
        return Vector3.zero;
    }
}


