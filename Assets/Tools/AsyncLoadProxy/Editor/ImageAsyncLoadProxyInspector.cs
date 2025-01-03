using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ImageAsyncLoadProxyInspector : Editor
{
    [MenuItem("Assets/Add Image Async Proxy")]
    static public void GenerateImageAsyncProxy()
    {
        string topLevelPath = "";
        Object[] top = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        if (null != top && null != top[0])
            topLevelPath = AssetDatabase.GetAssetPath(top[0]);

        string[] selection = AssetDatabase.FindAssets("t:prefab", new string[] { topLevelPath });
        for (int i = 0,icnt = selection.Length; i < icnt; ++i)
        {
            EditorUtility.DisplayProgressBar("添加图片控件预加载代理", "正在添加第" + i + "个资源...", ((i++) / icnt));

            string guid = selection[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject curPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (null == curPrefab)
                continue;

            GameObject temp = PrefabUtility.InstantiatePrefab(curPrefab) as GameObject;
            Image[] aImg = temp.GetComponentsInChildren<Image>();
            if (null != aImg)
            {
                for(int j = 0,jcnt = aImg.Length;j<jcnt;++j)
                {
                    Image curImg = aImg[j];
                    if(null == curImg) continue;

                    ImageAsyncLoadProxy newProxy = curImg.gameObject.GetComponent<ImageAsyncLoadProxy>();
                    if (null == newProxy)
                        newProxy = curImg.gameObject.AddComponent<ImageAsyncLoadProxy>();

                    if (null != newProxy)
                        newProxy.m_Image = curImg;
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, curPrefab, ReplacePrefabOptions.ConnectToPrefab);

            Editor.DestroyImmediate(curPrefab, false);
        }

        AssetDatabase.SaveAssets();

        EditorUtility.ClearProgressBar();
    }
}
