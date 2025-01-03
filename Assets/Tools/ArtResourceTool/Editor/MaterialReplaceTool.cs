using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MaterialReplaceTool : EditorWindow
{
    /// 替换掉所有的AlphaTest材质
    static protected readonly string RES_PATH = "Assets/Resources";
    static protected Shader m_SrcShader = null;
    static protected Shader m_DstShader = null;

    [MenuItem("[TM工具集]/ArtTools/Shader替换工具")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MaterialReplaceTool window = (MaterialReplaceTool)EditorWindow.GetWindow(typeof(MaterialReplaceTool));
        window.titleContent = new GUIContent("批量Shader替换工具");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("注意目标Shader为空表示将所有的shader替换成目标shader!");
        m_SrcShader = EditorGUILayout.ObjectField("目标Shader", m_SrcShader, typeof(Shader)) as Shader;
        m_DstShader = EditorGUILayout.ObjectField("==替换成==>", m_DstShader, typeof(Shader)) as Shader;
        GUILayout.EndVertical();

        if (null == m_DstShader)
        {
            EditorGUILayout.HelpBox(string.Format("请先选择好要替换的目标Shader！"), MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("执行替换"))
            {
                _ReplaceShader();
            }
        }
    }

    private void _ReplaceShader()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { RES_PATH });
        int cnt = 0;
        float total = str.Length;
        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("替换材质Shader资源", "正在替换第" + cnt + "个资源...", ((cnt++) / total));
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                List<Renderer> rendLst = new List<Renderer>();
                temp.GetComponentsInChildren<Renderer>(true,rendLst);

                for(int i = 0,icnt = rendLst.Count;i<icnt;++i)
                {
                    Renderer curRend = rendLst[i];
                    if(null == curRend) continue;

                    Material[] asm = curRend.sharedMaterials;
                    if(null != asm)
                    {
                        for(int j =0,jcnt = asm.Length;j<jcnt;++j)
                        {
                            Material curMat = asm[j];
                            if(null == curMat) continue;
                            if(null == curMat.shader) continue;

                            if (null == m_SrcShader || curMat.shader.name.Equals(m_SrcShader))
                                curMat.shader = m_DstShader;
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }
}
