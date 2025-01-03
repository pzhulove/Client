using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CommonComponentRefreshEditor : EditorWindow
{
    private string fullPath = "Assets/Resources/UIFlatten/Prefabs/";
    private static CommonComponentRefreshEditor window;

    private bool mIsRefreshComBtnType = false;
    private bool mIsRefreshComTabType = false;


    [MenuItem("[TM工具集]/打开刷新通用组件预置物窗口")]
    static void Execute()
    {
        if (window == null)
        {
            window = (CommonComponentRefreshEditor)GetWindow(typeof(CommonComponentRefreshEditor),false, "刷新通用按钮窗口");
        }
        window.Show();
    }
    private void OnGUI()
    {
        //---------------------------------------------------
        mIsRefreshComBtnType = EditorGUILayout.Toggle("是否刷新通用按钮（挂载了ComButtonType的预置物）", mIsRefreshComBtnType, GUILayout.Width(1600));
        mIsRefreshComTabType = EditorGUILayout.Toggle("是否刷新通用标签（挂载了ComTabType的预置物）", mIsRefreshComTabType);
        if (GUILayout.Button("刷新校正"))
        {
            Debug.Log("开始查找刷新校正.");
            _FindAndRefreshScript();
        }
    }

    //获取指定路径下面的所有资源文件
    private void _FindAndRefreshScript()
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*.prefab", SearchOption.AllDirectories);
            if (files == null)
            {
                return;
            }

            Debug.Log("PrefabCount: " + files.Length);
            int count = 0;
            for (int i = 0; i < files.Length; i++)
            {
                string dirName = files[i].DirectoryName.Replace(direction.FullName, fullPath);
                dirName = dirName.Replace("\\", "/");
                string path = dirName + "/" + files[i].Name;
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(object)) as GameObject;
                if (go == null)
                {
                    continue;
                }

                if (mIsRefreshComBtnType)
                {
                    _RefreshCommonBtn(go, ref count);
                }

                if (mIsRefreshComTabType)
                {
                    _RefreshCommonType(go, ref count);
                }
            }

            if (count > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("<color=red>" + count + "个预置物校正完毕</color>");
            }
            else
            {
                Debug.Log("<color=green>无预置物改动</color>");
            }
        }
    }

    /// <summary>
    /// 刷新通用按钮组件（挂载了ComButtonType的组件）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="count"></param>
    private void _RefreshCommonBtn(GameObject go, ref int count)
    {
        if (go.name.StartsWith("CommonButton"))
        {
            return;
        }

        Component[] co = go.GetComponentsInChildren<ComButtonType>(true);
        foreach (var _child in co)
        {
            if (_child == null)
            {
                continue;
            }

            ComButtonType comButtonType = _child as ComButtonType;
            bool isChangeTemp = false;
            isChangeTemp = comButtonType.SetImage();
            isChangeTemp = isChangeTemp || comButtonType.SetSize();

            string desc = "<color=yellow> 通用按钮(挂载了ComButtonType的组件) </color> Find it: " + go.name + "--->> " + _child.name + " {0}";
            desc = isChangeTemp ? string.Format(desc, "<color=red>Changed</color>") : string.Format(desc, "<color=green>NoChnage</color>");
            Debug.Log(desc);

            if (isChangeTemp)
            {
                count++;
            }
        }
    }

    /// <summary>
    /// 刷新通用标签组件（挂载了ComTabType的组件）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="count"></param>
    private void _RefreshCommonType(GameObject go, ref int count)
    {
        if (go.name.StartsWith("CommonButton"))
        {
            return;
        }

        Component[] co = go.GetComponentsInChildren<ComTabType>(true);
        foreach (var _child in co)
        {
            if (_child == null)
            {
                continue;
            }

            ComTabType comTabType = _child as ComTabType;
            bool isChangeTemp = comTabType.SetSize();

            string desc = "<color=yellow> 通用标签组(挂载了ComTabType的组件) </color> Find it: " + go.name + "--->> " + _child.name + " {0}";
            desc = isChangeTemp ? string.Format(desc, "<color=red>Changed</color>") : string.Format(desc, "<color=green>NoChnage</color>");
            Debug.Log(desc);

            if (isChangeTemp)
            {
                count++;
            }
        }
    }
}
