using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MaterialNormailizer : EditorWindow
{
    static protected readonly string RES_PATH = "Assets/Resources";

    static protected List<string> matAbandonList = new List<string>();

    /*
     * 删除Standard材质
     * 替换Particle到Mobile材质
     */
    [MenuItem("[TM工具集]/ArtTools/材质清理替换")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MaterialNormailizer window = (MaterialNormailizer)EditorWindow.GetWindow(typeof(MaterialNormailizer));
        window.titleContent = new GUIContent("材质清理替换");
        window.Show();
    }

    protected class ShaderReplaceDesc
    {
        public Shader m_Src;
        public Shader m_Dst;
    }

    static protected List<ShaderReplaceDesc> m_ReplaceSet = new List<ShaderReplaceDesc>();

    void OnGUI()
    {
        for(int i = 0,icnt= m_ReplaceSet.Count;i<icnt;++i)
        {
            ShaderReplaceDesc cur = m_ReplaceSet[i];
            GUILayout.BeginHorizontal();
            cur.m_Src = EditorGUILayout.ObjectField("目标着色器：", cur.m_Src, typeof(Shader)) as Shader;
            cur.m_Dst = EditorGUILayout.ObjectField("->替换成:", cur.m_Dst, typeof(Shader)) as Shader;
            GUILayout.EndHorizontal();
        }

        if(GUILayout.Button("添加替换目标"))
        {
            ShaderReplaceDesc newRep = new ShaderReplaceDesc();
            m_ReplaceSet.Add(newRep);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("开始整理材质"))
        {
            var str = AssetDatabase.FindAssets("t:material", new string[] { "Assets/Resources" });
            int cnt = 0;
            foreach (var guid in str)
            {
                EditorUtility.DisplayProgressBar("正在整理材质", string.Format("正在整理第{0}个材质！（共{1}个）", cnt++, str.Length), (float)cnt / str.Length);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Material curMat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (null == curMat)
                    continue;

                for(int i = 0,icnt = m_ReplaceSet.Count;i<icnt;++i)
                {
                    ShaderReplaceDesc cur = m_ReplaceSet[i];
                    if(null == cur) continue;

                    if (curMat.shader.name == cur.m_Src.name)
                    {
                        if (null == cur.m_Dst)
                            matAbandonList.Add(path);
                        else
                            curMat.shader = cur.m_Dst;
                    }
                }

                EditorUtility.SetDirty(curMat);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();

            if (matAbandonList.Count > 0)
                AssetDatabase.ExportPackage(matAbandonList.ToArray(), "UnusedMaterial.unitypackage");

            foreach (var path in matAbandonList)
                File.Delete(path);
        }
    }
}
