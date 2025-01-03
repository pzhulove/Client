using UnityEditor;
using UnityEngine;

public class ComTabType : MonoBehaviour
{
    //注意枚举若要添加记得往后面加，不要插进去

    private enum SizeType
    {
        最小,
        中等,
        超级大,
    }

    [SerializeField] private SizeType mSizeType;
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

        int sizeIndex = (int)mSizeType;
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

    private void Awake()
    {
        SetSize();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComTabType))]
public class ComTabTypeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("设置标签", GUILayout.Width(200)))
        {
            ComTabType comTabType = target as ComTabType;
            if (comTabType == null)
            {
                return;
            }

            bool isChanged = _SetTabType(comTabType);
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (isChanged && prefabStage != null && prefabStage.IsPartOfPrefabContents(comTabType.gameObject))
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty((prefabStage.scene));
            }
        }
    }

    private bool _SetTabType(ComTabType tabType)
    {
        if (null == tabType)
        {
            return false;
        }

        return tabType.SetSize();
    }
}
#endif
