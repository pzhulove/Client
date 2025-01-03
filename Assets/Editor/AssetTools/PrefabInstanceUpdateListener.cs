using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetTools
{
    /// <summary>
    /// 监听Prefab修改保存事件，做一些资源检测
    /// </summary>
    class PrefabInstanceUpdateListener
    {

        [MenuItem("[TM工具集]/资源规范相关/检查GUID")]
        static void CheckGUID()
        {
            // Get existing open window or if none, make a new one:
            string assetPath = AssetDatabase.GUIDToAssetPath("93ee367bff7fff74d9cea168ca2cac06");
            Debug.LogErrorFormat(assetPath);
        }


        [InitializeOnLoadMethod]
        static void StartInitializeOnLoadMethod()
        {
            PrefabUtility.prefabInstanceUpdated += CheckMaterialTextureProperty;
        }

        /// <summary>
        /// 检测材质是否有隐藏的TextureProperties引用了Texture。
        /// </summary>
        /// <param name="instance"></param>
        static void CheckMaterialTextureProperty(GameObject instance)
        {
            List<string> m_RepairList = new List<string>();

            string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(instance));
            if (!string.IsNullOrEmpty(prefabPath))
            {
                string[] dependents = AssetDatabase.GetDependencies(prefabPath);
                if(dependents != null)
                {
                    foreach(var dep in dependents)
                    {
                        if(dep.EndsWith(".mat"))
                        {
                            Material mat = AssetDatabase.LoadAssetAtPath<Material>(dep);
                            if (mat)
                            {
                                bool bRepair = false;

                                // 通过GetTexturePropertyNames能获取到隐藏的纹理Properties，但跟当前Shader不一致的通过HasProperty判断会返回false。
                                SerializedObject so = new SerializedObject(mat);
                                SerializedProperty pp = so.GetIterator();
                                //       prop.Reset();

                                while (pp.Next(true))
                                {
                                    if (pp != null)
                                    {
                                        if (pp.name == "m_TexEnvs" && pp.isArray)
                                        {
                                            int arrayIndex = pp.arraySize;
                                            for (int i = 0; i < arrayIndex; ++i)
                                            {
                                                SerializedProperty dataProperty = pp.GetArrayElementAtIndex(i);

                                                if (dataProperty != null)
                                                {
                                                    SerializedProperty texNameProperty = dataProperty.FindPropertyRelative("first");
                                                    if (texNameProperty != null)
                                                    {
                                                        string texName = texNameProperty.stringValue;
                                                        if (!mat.HasProperty(texName) && mat.GetTexture(texName) != null)
                                                        {
                                                            mat.SetTexture(texName, null);
                                                            bRepair = true;
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }

                                if (bRepair)
                                {
                                    m_RepairList.Add(dep);
                                }
                            }
                        }
                    }
                }
            }

            if(m_RepairList.Count > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("提交SVN", "发现Prefab用到的材质有问题，已自动修复，请负责提交一下！", "确认");

                SVNCommitTool.CommitSVN(m_RepairList, "Bug(修复材质)：提交修复材质冗余Property。");
            }
        }
    }

    class SVNCommitTool
    {
        static public void CommitSVN(List<string> assetNames, string logName)
        {
            if (assetNames.Count > 0)
            {
                string commitFileNames = assetNames[0];
                if (assetNames.Count > 1)
                {
                    for (int i = 1; i < assetNames.Count; ++i)
                    {
                        commitFileNames += "*" + assetNames[i];
                    }
                }

                SvnTool.SvnCommit(commitFileNames, logName);
            }
        }
    }
}
