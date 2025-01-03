using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class ComButtonType : MonoBehaviour
{
    //注意枚举若要添加记得往后面加，不要插进去

    public enum BgType
    {
        背景图1,
        背景图2,
    }

    public enum SizeType
    {
        最小,
        中等,
        超级大,
    }


    [HideInInspector] public BgType bgType;
    [HideInInspector] public SizeType sizeType;

    [SerializeField] private ComComponentTypeData mComBtnTypeData;

    /// <summary>
    /// 设置尺寸 成功返回true 否则返回false
    /// </summary>
    /// <returns></returns>
    public bool SetSize()
    {
        if (mComBtnTypeData == null || mComBtnTypeData.btnSizes == null || mComBtnTypeData.btnSizes.Count <= 0)
        {
            return false;
        }

        if (transform.rectTransform().anchorMin != transform.rectTransform().anchorMax)     //锚框点不在一起的不设置尺寸，否则尺寸会出问题
        {
            return false;
        }

        int sizeIndex = (int)sizeType;
        if (mComBtnTypeData.btnSizes.Count <= sizeIndex)
        {
            sizeIndex = mComBtnTypeData.btnSizes.Count - 1;
        }

        if (transform.rectTransform().rect.size != mComBtnTypeData.btnSizes[sizeIndex]) //这里用rect.size比较，不能用sizedelta，sizedelta会因为锚点改变而表达不同的意思
        {
            transform.rectTransform().sizeDelta = mComBtnTypeData.btnSizes[sizeIndex];      //按钮预置物anchormin和anchormax要在同一点上，不然设置尺寸会出问题
            return true;
        }

        return false;
    }

    /// <summary>
    /// 设置图片 成功返回true 否则返回false
    /// </summary>
    /// <returns></returns>
    public bool SetImage()
    {
        if (mComBtnTypeData == null || mComBtnTypeData.imageBg == null)
        {
            return false;
        }

        if (mComBtnTypeData.bgPaths == null || mComBtnTypeData.bgPaths.Count <= 0)
        {
            return false;
        }

        int index = (int)bgType;
        if (mComBtnTypeData.bgPaths.Count <= index)
        {
            index = mComBtnTypeData.bgPaths.Count - 1;
        }

        if (mComBtnTypeData.imageBg.sprite == null 
            || !mComBtnTypeData.bgPaths[index].EndsWith(mComBtnTypeData.imageBg.sprite.name))
        {
            mComBtnTypeData.imageBg.SafeSetImage(mComBtnTypeData.bgPaths[index]);

            return true;
        }

        return false;
    }

    private void Awake()
    {
        SetImage();
        SetSize();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComButtonType))]
public class ComButtonTypeInspector : Editor
{
    //public enum
    private List<string> mStrsBgPath = null;
    private List<string> mStrsSize = null;
    private ComComponentTypeData mComComponentTypeData = null;
    private ComButtonType mComBtnType = null;
    private GUIStyle mStyle = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _InitData();

        if (mComBtnType == null || mComComponentTypeData == null)
        {
            return;
        }

        GUILayout.Space(10);
        if (mStyle == null)
        {
            mStyle = new GUIStyle();
            mStyle.normal.textColor = Color.green;
        }
        GUILayout.Label("以下为操作项:", mStyle);

        _CreateEnumPop();

        if (GUILayout.Button("设置按钮", GUILayout.Width(200)))
        {
            if (mComBtnType == null)
            {
                return;
            }

            bool isChanged = _SetBtnType(mComBtnType);

            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (isChanged && prefabStage != null && prefabStage.IsPartOfPrefabContents(mComBtnType.gameObject))
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty((prefabStage.scene));
            }
        }
    }

    private void _InitData()
    {
        if (mComBtnType == null)
        {
            mComBtnType = target as ComButtonType;
        }
        if (mComComponentTypeData == null)
        {
            var comData = typeof(ComButtonType);
            var field = comData.GetField("mComBtnTypeData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            mComComponentTypeData = (ComComponentTypeData)field.GetValue(mComBtnType);
        }
    }

    private void _CreateEnumPop()
    {
        if (mStrsBgPath == null)
        {
            mStrsBgPath = new List<string>();
        }
        if (mStrsBgPath.Count <= 0 && mComComponentTypeData.bgPaths != null)
        {
            for (int i = 0; i < mComComponentTypeData.bgPaths.Count; i++)
            {
                string[] strs = mComComponentTypeData.bgPaths[i].Split(':');
                string str = string.Empty;
                if (strs != null && strs.Length > 0)
                {
                    str = strs[strs.Length - 1];
                }

                mStrsBgPath.Add(str);
            }
        }
        object objBgType = Utility.EnumPopup("设置背景图： ", mComBtnType.bgType, mStrsBgPath, false);
        if (objBgType != null)
        {
            mComBtnType.bgType = (ComButtonType.BgType)objBgType;
        }

        if (mStrsSize == null)
        {
            mStrsSize = new List<string>();
        }
        if (mStrsSize.Count <= 0 && mComComponentTypeData.btnSizes != null)
        {
            for (int i = 0; i < mComComponentTypeData.btnSizes.Count; i++)
            {
                string str = string.Format("{0}x{1}", mComComponentTypeData.btnSizes[i].x, mComComponentTypeData.btnSizes[i].y);

                mStrsSize.Add(str);
            }
        }
        object objSizeType = Utility.EnumPopup("设置尺寸： ", mComBtnType.sizeType, mStrsSize, false);
        if (objSizeType != null)
        {
            mComBtnType.sizeType = (ComButtonType.SizeType)objSizeType;
        }
    }

    private bool _SetBtnType(ComButtonType btnType)
    {
        if (null == btnType)
        {
            return false;
        }

        bool isChanged = false;
        isChanged = btnType.SetImage();
        isChanged = isChanged || btnType.SetSize();

        return isChanged;
    }
}
#endif
