using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ComUIiPhoneXAspect))]
public class ComUIiPhoneXAspectInspector : Editor
{
	public enum eDebugOption
	{
		eNone,
		eForceLandRight,
		eForceLandLeft
	}

    public enum eDefaultParams
    {
        eNone,
        eiPhoneX
    }

	private bool mDirty = false;
    private bool mDirty2 = false;
	private eDebugOption mDebugOption = eDebugOption.eNone;
    private eDefaultParams mDefaultParams = eDefaultParams.eNone;

	ComUIiPhoneXAspect   mCurrentAspectCom = null;

	public override void OnInspectorGUI ()
	{
		eDebugOption option = (eDebugOption)EditorGUILayout.EnumPopup("调试-屏幕转向",mDebugOption);
        eDefaultParams param = (eDefaultParams)EditorGUILayout.EnumPopup("新增分辨率时-用iPhoneX参数作为默认值",mDefaultParams);

		if (option != mDebugOption)
		{
			mDirty = true;
			mDebugOption = option;
			mCurrentAspectCom = this.target as ComUIiPhoneXAspect;
		}

		if (mDirty && null != mCurrentAspectCom)
		{
			mDirty = false;

			switch (mDebugOption)
			{
			case eDebugOption.eForceLandLeft:
				mCurrentAspectCom.ForceUpdateHandLeft();
				break;
			case eDebugOption.eForceLandRight:
				mCurrentAspectCom.ForceUpdateHandRight();
				break;
			case eDebugOption.eNone:
				mCurrentAspectCom.ForceUpdateHandNone();
				break;
			}
		}

        if (param != mDefaultParams)
        {
            mDirty2 = true;
            mDefaultParams = param;
            mCurrentAspectCom = this.target as ComUIiPhoneXAspect;
        }

        if (mDirty2 && null != mCurrentAspectCom)
        {
            mDirty2 = false;
            if (mDefaultParams == eDefaultParams.eiPhoneX)
            {
                mCurrentAspectCom.InitOriginaliPhoneXParam();
            }
        }

		base.OnInspectorGUI ();
	}
}
