using UnityEngine;
using UnityEditor;
using System.Collections;

public class EffectParticleNormalizer : Editor
{
    static protected readonly string RES_PATH = "Assets/Resources/Effects";
    //static protected readonly string RES_PATH = "Assets/Resources/Effects/Equipment";

    [MenuItem("[TM工具集]/ArtTools/Normalize Max Particles")]
    static public void NormalizeMaxParticles()
    {
        string[] resTable = AssetDatabase.FindAssets("t:prefab", new string[] { RES_PATH });

        int cnt = 0;
        float total = resTable.Length;
        foreach (string guid in resTable)
        {
            EditorUtility.DisplayProgressBar("正在标准化粒子特效", "正在标准化第" + cnt + "个资源...", ((cnt++) / total));

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                ParticleSystem[] aps= temp.GetComponentsInChildren<ParticleSystem>();
                for(int i = 0,icnt = aps.Length;i<icnt;++i)
                {
                    ParticleSystem curPartSys = aps[i];
                    if(null == curPartSys) continue;

                    ParticleSystem.Burst[] burstArray = new ParticleSystem.Burst[curPartSys.emission.burstCount];
                    int burstCnt = curPartSys.emission.GetBursts(burstArray);
                    int maxBurstCnt = 0;
                    for (int j = 0,jcnt = curPartSys.emission.burstCount;j<jcnt;++j)
                        maxBurstCnt = burstArray[j].maxCount > maxBurstCnt ? burstArray[j].maxCount: maxBurstCnt;

                    curPartSys.maxParticles = (int)(curPartSys.startLifetime * curPartSys.emission.rate.constantMax) + 1 + maxBurstCnt;
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.SaveAssets();
    }
}
