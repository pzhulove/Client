using UnityEditor;
using UnityEngine;

public class ComButtonFixedSizeType : MonoBehaviour
{
    //注意枚举若要添加记得往后面加，不要插进去

    private enum BgType
    {
        背景图1,
        背景图2,
        背景图3,
        背景图4,
    }

    private enum TextColorType
    {
        颜色1,
        颜色2,
    }

    [SerializeField] private BgType mBgType;
    [SerializeField] private TextColorType mTextColorType;
    [SerializeField] private ComComponentTypeData mComBtnTypeData;

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

        int index = (int)mBgType;
        if (mComBtnTypeData.bgPaths.Count <= index)
        {
            index = mComBtnTypeData.bgPaths.Count - 1;
        }

        if (mComBtnTypeData.imageBg.sprite == null 
            || !mComBtnTypeData.bgPaths[index].EndsWith(mComBtnTypeData.imageBg.sprite.name))
        {
            mComBtnTypeData.imageBg.SafeSetImage(mComBtnTypeData.bgPaths[index]);
            mComBtnTypeData.imageBg.SetNativeSize();

            return true;
        }

        return false;
    }

    public bool SetText()
    {
        if (mComBtnTypeData == null || mComBtnTypeData.text == null)
        {
            return false;
        }

        if (mComBtnTypeData.textColors == null || mComBtnTypeData.textColors.Count <= 0)
        {
            return false;
        }

        int index = (int)mTextColorType;
        if (mComBtnTypeData.textColors.Count <= index)
        {
            index = mComBtnTypeData.textColors.Count - 1;
        }

        if (mComBtnTypeData.text.color != mComBtnTypeData.textColors[index])
        {
            mComBtnTypeData.text.SafeSetColor(mComBtnTypeData.textColors[index]);
            return true;
        }

        return false;
    }

    private void Awake()
    {
        SetImage();
        SetText();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComButtonFixedSizeType))]
public class ComButtonFixedSizeTypeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("设置按钮", GUILayout.Width(200)))
        {
            ComButtonFixedSizeType comBtnType = target as ComButtonFixedSizeType;
            if (comBtnType == null)
            {
                return;
            }

            bool isChanged = _SetBtnType(comBtnType);

            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (isChanged && prefabStage != null && prefabStage.IsPartOfPrefabContents(comBtnType.gameObject))
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty((prefabStage.scene));
            }
        }
    }

    private bool _SetBtnType(ComButtonFixedSizeType btnType)
    {
        if (null == btnType)
        {
            return false;
        }

        bool isChanged = false;
        isChanged = btnType.SetImage();
        isChanged = isChanged || btnType.SetText();

        return isChanged;
    }
}
#endif
