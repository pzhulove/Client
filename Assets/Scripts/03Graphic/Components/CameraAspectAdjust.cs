using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAspectAdjust : MonoBehaviour
{
    Camera mCameraObj;

    public float mOffsetAdjust 	= 0.0f;

    private int mScreenHeight 	= 0;
    private int mScreenWidth  	= 0;

    string sdkDeviceModelName = "DEFAULT";

	public enum eScreenType
	{
		None                    = 0,
		R_16_9                  = 1,
		R_2_1,
		/// <summary>
		/// 三星 note8
		/// </summary>
		R_18_5_9,
		/// <summary>
		/// 21:9
		/// </summary>
		R_21_9,

        //从这里开始带刘海全面屏手机分辨率
        /// <summary>
        /// iPhoneX
        /// </summary>
        R_iPhoneX_2436_1125,
        /// <summary>
        /// Vivo X21 
        /// Oppo R15
        /// </summary>
        R_BangFullScreen_2280_1080,

        /// <summary>
        /// HuaWei p20 pro
        /// </summary>
        R_BangFullScreen_2240_1080,

        /// <summary>
        /// HuaWei p20 
        /// </summary>
        R_BangFullScreen_2244_1080,

        /// <summary>
        /// vivo x21 导航栏 / 状态栏 占用后的分辨率
        /// </summary>
        R_BangFullScreen_2208_1080,

        /// <summary>
        /// 大于16：9的情况
        /// </summary>
        R_16_9_2,

        R_Other,
	}

	static eScreenType mScreenType = eScreenType.None;

    const float kScreen16_9Rate = (16.0f / 9.0f);
    const float kScreen9_16Rate = 1.0f / kScreen16_9Rate;
    const float kDelta          = 0.5f;

    private uint mDirtyCount 	= 0;
    private static uint mDirtyPublicCount = 0;

    public static void MarkDirty()
    {
        mDirtyPublicCount++;
    }

	public static eScreenType GetScreenType()
	{
		return mScreenType;
	}
    
	void Start()
    {
        mCameraObj = GetComponent<Camera>();
        _TryGetDeviceModelName();
        _onUpdate();
    }

    private void _onUpdate()
    {
        if (null != mCameraObj && (mScreenHeight !=  mCameraObj.pixelHeight) || (mScreenWidth != mCameraObj.pixelWidth))
        {
			mScreenType = eScreenType.None;

            //Debug.LogFormat("sw {0}, sh{1}, {2}", Screen.width, Screen.height, gameObject.name);
            //Debug.LogFormat("cw {0}, ch{1}, {2}", mCameraObj.pixelWidth, mCameraObj.pixelHeight, gameObject.name);

            if(Screen.width == Screen.height * 2)
            {
                var rect 		= mCameraObj.rect;

                rect.xMin 		= 0.0f;
                rect.xMax 		= 1.0f;
                rect.yMin 		= 0.0f;
                rect.yMax 		= 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight 	= mCameraObj.pixelHeight;
                mScreenWidth 	= mCameraObj.pixelWidth;
				mScreenType     = eScreenType.R_2_1;
            }
			else if(Screen.width * 18 == Screen.height * 37)
            {
                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
				mScreenType = eScreenType.R_18_5_9;
            } 
            else if(Screen.width * 9 == Screen.height * 21)
			{
				var rect = mCameraObj.rect;

				rect.xMin = 0.0f;
				rect.xMax = 1.0f;
				rect.yMin = 0.0f;
				rect.yMax = 1.0f;
				mCameraObj.rect = rect;
				mScreenHeight = mCameraObj.pixelHeight;
				mScreenWidth = mCameraObj.pixelWidth;
				mScreenType = eScreenType.R_21_9;
			} 
			else if(Screen.width * 1125 == Screen.height * 2436)
			{
				var rect = mCameraObj.rect;

				rect.xMin = 0.0f;
				rect.xMax = 1.0f;
				rect.yMin = 0.0f;
				rect.yMax = 1.0f;
				mCameraObj.rect = rect;
				mScreenHeight = mCameraObj.pixelHeight;
				mScreenWidth = mCameraObj.pixelWidth;
				mScreenType = eScreenType.R_iPhoneX_2436_1125;
			} 
            else if(Screen.width * 27 == Screen.height * 56)
            {
                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
                mScreenType = eScreenType.R_BangFullScreen_2240_1080;
            }
            else if (Screen.width * 90 == Screen.height * 187)
            {
                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
                mScreenType = eScreenType.R_BangFullScreen_2244_1080;
            }
            else if (Screen.width * 9 == Screen.height * 19)
            {
                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
                mScreenType = eScreenType.R_BangFullScreen_2280_1080;
            }
            else if (Screen.width * 45 == Screen.height * 92)
            {
                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
                mScreenType = eScreenType.R_BangFullScreen_2208_1080;
            }
            //大于16:9的情况
            else if (Screen.width * 9 > 16 * Screen.height)
            {
                /*
                int width = (int)(Screen.height / kScreen9_16Rate + kDelta);

                float delta = ((float)Screen.width - (float)width) / (float)Screen.width / 2.0f;

                var rect = mCameraObj.rect;

                rect.xMin = delta - mOffsetAdjust / Screen.height;
                rect.xMax = 1 - delta + mOffsetAdjust / Screen.height;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                Debug.LogFormat("ViewPort set post mCameraObj.pixelHeight{0}  {1}", mCameraObj.pixelHeight, gameObject.name);
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;*/

                var rect = mCameraObj.rect;

                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = 0.0f;
                rect.yMax = 1.0f;
                mCameraObj.rect = rect;
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;

#if !UNITY_IOS
                //默认有刘海
                mScreenType = eScreenType.R_16_9_2;
#endif

#if UNITY_IOS
                //iphone x xs xr ... 如果有刘海 在这里添加 对应分辨率 默认都用ipx的偏移值
                if (Screen.width == 2688 && Screen.height == 1242 ||
                    Screen.width == 1792 && Screen.height == 828)
                {
                    mScreenType = eScreenType.R_iPhoneX_2436_1125;
                }
#endif

                /*
                先处理这3种没有刘海
                VIVO NEX (没刘海）2316 1080
                OPPO FindX（没刘海）
                */
                //if (Screen.width == 2316 && Screen.height == 1080 ||
                //    Screen.width == 2340 && Screen.height == 1080)
                //{
                //    if (!string.IsNullOrEmpty(sdkDeviceModelName) &&
                //            (
                //                sdkDeviceModelName.Equals("XIAOMI") ||
                //                sdkDeviceModelName.Equals("VIVO")
                //            )
                //        )
                //    {
                //        mScreenType = eScreenType.R_16_9_2;
                //    }
                //    else
                //    {
                //        mScreenType = eScreenType.R_16_9;
                //    }
                //}
            }
            //小于等于16:9的情况
            else
            {
                int height = (int)(Screen.width / kScreen16_9Rate + kDelta);
                float delta = ((float)Screen.height - (float)height) / (float)Screen.height / 2.0f;
                var rect = mCameraObj.rect;
                rect.xMin = 0.0f;
                rect.xMax = 1.0f;
                rect.yMin = delta - mOffsetAdjust / Screen.height;
                rect.yMax = 1 - delta + mOffsetAdjust / Screen.height;
                //Debug.LogFormat("ViewPort set {0} {1} {2} {3} height{4} mCameraObj.pixelHeight{5} {6}", rect.yMin, rect.yMax, Screen.width, Screen.height, height, mCameraObj.pixelHeight, gameObject.name);
                mCameraObj.rect = rect;
                //Debug.LogFormat("ViewPort set post mCameraObj.pixelHeight{0}  {1}", mCameraObj.pixelHeight, gameObject.name);
                mScreenHeight = mCameraObj.pixelHeight;
                mScreenWidth = mCameraObj.pixelWidth;
                mScreenType = eScreenType.R_16_9;
            }
        }

    }

    void Update()
    {
#if UNITY_EDITOR
        _onUpdate();
#else
        if (mDirtyCount != mDirtyPublicCount)
        {
            _onUpdate();
            mDirtyCount = mDirtyPublicCount;
        }
#endif
    }

    private void _TryGetDeviceModelName()
    {
		sdkDeviceModelName = "DEFAULT";
        try
        {
            string model = SystemInfo.deviceModel;
            if (!string.IsNullOrEmpty(model))
            {
                string[] modelarray = model.Split(' ');
                if (modelarray != null && modelarray.Length > 0)
                {
                    if (!string.IsNullOrEmpty(modelarray[0]))
                    {
                        sdkDeviceModelName = modelarray[0].ToUpper();
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat("[SDKInterface] try get device model name failed , device model is {0}, error is {1}", SystemInfo.deviceModel, e.ToString());
            sdkDeviceModelName = "DEFAULT";
        }
    }

}
