using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EffectPrefabCooker : Editor
{
    [MenuItem("Assets/标准化特效资源")]
    static void NormalizeEffectPrefab()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;
            if (null == curPrefab)
                continue;

            GameObject instance = GameObject.Instantiate(curPrefab) as GameObject;
            float effectLen = AnimatorDowngradeConverter._DowngradeAnimator(instance);

            GeEffectProxy proxy = instance.GetComponent<GeEffectProxy>();
            if (null == proxy)
                proxy = instance.AddComponent<GeEffectProxy>();

            if(null != proxy)
                proxy.DoCookData(false);

            ComAnimatorAutoClose autoClose = instance.GetComponent<ComAnimatorAutoClose>();
            if(null != autoClose)
            {
                autoClose.mAnimator = null;
                autoClose.mDefTimeLen = effectLen;
            }

            Object prefabParrent = PrefabUtility.GetPrefabParent(curPrefab);
            if (null == prefabParrent)
                prefabParrent = curPrefab;
            PrefabUtility.ReplacePrefab(instance, prefabParrent, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.SaveAssets();
            GameObject.DestroyImmediate(instance);

            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("[TM工具集]/ArtTools/标准化所有特效资源")]
    static void NormalizeAllEffectPrefab()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/Scene", "Assets/Resources/Effects" });
        foreach (var guid in str)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject curPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (null == curPrefab)
                continue;

            GameObject instance = GameObject.Instantiate(curPrefab) as GameObject;
            float effectLen = AnimatorDowngradeConverter._DowngradeAnimator(instance);

            GeEffectProxy proxy = instance.GetComponent<GeEffectProxy>();
            if (null == proxy)
                proxy = instance.AddComponent<GeEffectProxy>();

            if (null != proxy)
                proxy.DoCookData(false);

            ComAnimatorAutoClose autoClose = instance.GetComponent<ComAnimatorAutoClose>();
            if (null != autoClose)
            {
                autoClose.mAnimator = null;
                autoClose.mDefTimeLen = effectLen;
            }

            Object prefabParrent = PrefabUtility.GetPrefabParent(curPrefab);
            if (null == prefabParrent)
                prefabParrent = curPrefab;
            PrefabUtility.ReplacePrefab(instance, prefabParrent, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.SaveAssets();
            GameObject.DestroyImmediate(instance);

            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
    }

}
