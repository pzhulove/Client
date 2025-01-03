using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EffectParticleRemover : Editor
{
    static protected readonly string RES_PATH = "Assets/Resources/Effects";

    /*
     * 删除Standard材质
     * 替换Particle到Mobile材质
     */
    [MenuItem("[TM工具集]/ArtTools/清理特效粒子系统")]
    static public void RemoveParticleSystem()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { RES_PATH });
        int cnt = 0;
        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("正在清理特效粒子", string.Format("正在整理第{0}个特效！（共{1}个）", cnt++, str.Length), (float)cnt / str.Length);
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (null == data)
                continue;

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                ParticleSystem[] aps = temp.GetComponentsInChildren<ParticleSystem>();
                if (null != aps)
                {
                    for (int j = 0, jcnt = aps.Length; j < jcnt; ++j)
                    {
                        ParticleSystem ps = aps[j];
                        if (null == ps) continue;
                        Object.DestroyImmediate(ps);
                    }
                }
            }

            EditorUtility.SetDirty(temp);
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);

            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
    }
}
