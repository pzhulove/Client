using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComUIFullScreenAspectUtility 
{
	private const int kWidth  = 1920;
	private const int kHeight = 1080;

	private static Vector2 originScreenSize = new Vector2(kWidth, kHeight);

    public static float GetScreenDeltaRate(CameraAspectAdjust.eScreenType type)
    {
        //         if (CameraAspectAdjust.eScreenType.R_16_9_2 == type 
        //         ||  CameraAspectAdjust.eScreenType.R_2_1 == type
        // 		||	CameraAspectAdjust.eScreenType.R_18_5_9 == type
        // 		||	CameraAspectAdjust.eScreenType.R_iPhoneX_2436_1125 == type
        // 		||	CameraAspectAdjust.eScreenType.R_21_9 == type
        //         ||  CameraAspectAdjust.eScreenType.R_BangFullScreen_2240_1080 == type
        //         ||  CameraAspectAdjust.eScreenType.R_BangFullScreen_2280_1080 == type
        //         ||  CameraAspectAdjust.eScreenType.R_BangFullScreen_2244_1080 == type
        //         ||  CameraAspectAdjust.eScreenType.R_BangFullScreen_2208_1080 == type
        // 		)
        // 		{
        // 			int weight = Screen.width;
        // 			int height = Screen.height; 
        // 
        // 			return (weight * 1.0f / height * kHeight / kWidth);
        // 		} 
        // 		else 
        // 		{
        // 			return 1.0f;
        // 		}

        int weight = Screen.width;
        int height = Screen.height;

        //如果宽：高 比例小于16：9 则屏幕分辨率以屏幕宽作为参照 
        if ((weight * 1.0f / height * kHeight / kWidth) < 1.0f)
        {
            return 1.0f;
        }

        return (weight * 1.0f / height * kHeight / kWidth);

    }

    public static Vector2 GetScreenDeltaSize(CameraAspectAdjust.eScreenType type)
    {
        return originScreenSize * GetScreenDeltaRate(type);
    }

    public static Vector2 GetScreenDeltaSizeEachSizeX(CameraAspectAdjust.eScreenType type)
    {
        Vector2 p = (GetScreenDeltaSize(type) - originScreenSize) / 2;
        return new Vector2(p.x, originScreenSize.y);
    }

}
