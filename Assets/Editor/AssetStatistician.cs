using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

public class AssetStatistician : EditorWindow
{
    static string m_EffectPath = "Assets/Resources/Effects/";

    protected class TextureResDesc
    {
        public TextureResDesc(string path,float size,int width,int height)
        {
            m_TexResPath = path;
            m_TexResMemSize = size;
            m_TexResWidth = width;
            m_TexResHeight = height;
        }

        public string m_TexResPath = "";
        public float m_TexResMemSize = 0.0f;
        public int m_TexResWidth = 0;
        public int m_TexResHeight = 0;
    }

    protected class EffectResDesc
    {
        public string m_ResPath;

        public List<TextureResDesc> m_TextureResDescList = new List<TextureResDesc>();
        public float m_MemorySize = 0.0f;
        public int m_ParticleSysNum = 0;
        public int m_EmitterNum = 0;
        public int m_AnimationNum = 0;
        public int m_MeshRenderNum = 0;
    }

    static List<EffectResDesc> m_EffectResStatiList = new List<EffectResDesc>();

    [MenuItem("[TM工具集]/ArtTools/统计特效资源引用")]
    static void StatisticEffectRes()
    {
        _Clear();

        string[] pathList = null;
        List<string> fileList = new List<string>();
        pathList = Directory.GetFiles(m_EffectPath, "*.prefab", SearchOption.AllDirectories);
        fileList.AddRange(pathList);

        for(int i = 0; i < fileList.Count; ++ i )
        {
            GameObject curObject = AssetDatabase.LoadAssetAtPath<GameObject>(fileList[i]);
            if (null != curObject)
            {
                AssetDatabase.GetDependencies(fileList[i]);
                UnityEngine.Object[] deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { curObject });

                EffectResDesc kNewEffResDesc = new EffectResDesc();
                kNewEffResDesc.m_ResPath = fileList[i].Replace('\\','/');

                kNewEffResDesc.m_MemorySize = 0;
                for (int j = 0; j < deps.Length; ++j)
                {
                    if(null == deps[j])
                        continue;

					float curSize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(deps[j]) / (1024.0f * 1024.0f);
                    string path = AssetDatabase.GetAssetPath(deps[j]);
                    TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (null != ti)
                    {
                        Texture2D curTex = deps[j] as Texture2D;
                        int texWidth = 0;
                        int texHeight = 0;
                        if (null != curTex)
                        {
                            texWidth = curTex.width;
                            texHeight = curTex.height;
                        }

                        kNewEffResDesc.m_TextureResDescList.Add(new TextureResDesc(path, curSize, texWidth, texHeight));
                    }

                    kNewEffResDesc.m_MemorySize += curSize;
                }

                MeshRenderer[] amr = curObject.GetComponentsInChildren<MeshRenderer>();
                kNewEffResDesc.m_MeshRenderNum = amr.Length;
                ParticleSystem[] aps = curObject.GetComponentsInChildren<ParticleSystem>();
                kNewEffResDesc.m_ParticleSysNum = aps.Length;
				
                //ParticleEmitter[] ape = curObject.GetComponentsInChildren<ParticleEmitter>();
                //kNewEffResDesc.m_EmitterNum = ape.Length;
			   
                Animator[] aa = curObject.GetComponentsInChildren<Animator>();
                kNewEffResDesc.m_AnimationNum = aa.Length;
                Animation[] aani = curObject.GetComponentsInChildren<Animation>();
                kNewEffResDesc.m_AnimationNum += aani.Length;

                Destroy(curObject);

                m_EffectResStatiList.Add(kNewEffResDesc);
            }
        }

        /// Dump
        /// 
        string date = DateTime.Now.ToString("yyyy_MM_dd");
        FileStream streamW = new FileStream(m_EffectPath + "EffectResStatistic_" + date + ".txt", FileMode.Create, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));

        sw.WriteLine(m_EffectResStatiList.Count.ToString() + " Effect Total");
        for (int i = 0; i < m_EffectResStatiList.Count ; ++i)
        {
            EffectResDesc kCurEffResDesc = m_EffectResStatiList[i];

            sw.WriteLine(i.ToString() + ":" + Path.GetFileName(kCurEffResDesc.m_ResPath) + "(Size:" + kCurEffResDesc.m_MemorySize.ToString() + "MB )");
            sw.WriteLine("    - Mesh Render Num:" + kCurEffResDesc.m_MeshRenderNum.ToString());
            sw.WriteLine("    - Particle System Num:" + kCurEffResDesc.m_ParticleSysNum.ToString());
            sw.WriteLine("    - Particle Emitter Num:" + kCurEffResDesc.m_EmitterNum.ToString());
            sw.WriteLine("    - Animation Num:" + kCurEffResDesc.m_AnimationNum.ToString());
            sw.WriteLine("    - Texture Num:" + kCurEffResDesc.m_TextureResDescList.Count.ToString());

            for (int j = 0, texum = kCurEffResDesc.m_TextureResDescList.Count; j < texum; ++j)
            {
                TextureResDesc curTexDesc = kCurEffResDesc.m_TextureResDescList[j];
                sw.WriteLine("        - Texture:" + Path.GetFileName(curTexDesc.m_TexResPath) + "(MemSize:" + curTexDesc.m_TexResMemSize.ToString() + "MB W:" + curTexDesc.m_TexResWidth.ToString() + " H:" + curTexDesc.m_TexResHeight.ToString() + ")");
            }
            sw.WriteLine("");
        }

        streamW.Flush();
        sw.Close();
        streamW.Close();
        
        streamW = new FileStream(m_EffectPath + "EffectResStatistic_" + date + ".csv", FileMode.Create, FileAccess.Write, FileShare.Write);
        sw = new StreamWriter(streamW, Encoding.GetEncoding(936));

        sw.WriteLine("Effect Name,Total Size,Mesh Render Num,Particle System Num,Particle Emitter Num,Animation Num,Texture Num,Texture Detail");
        for (int i = 0; i < m_EffectResStatiList.Count; ++i)
        {
            EffectResDesc kCurEffResDesc = m_EffectResStatiList[i];

            string content = "";

            content += Path.GetFileName(kCurEffResDesc.m_ResPath);
            content += ",";

            content += kCurEffResDesc.m_MemorySize.ToString();
            content += ",";

            content += kCurEffResDesc.m_MeshRenderNum.ToString();
            content += ",";

            content += kCurEffResDesc.m_ParticleSysNum.ToString();
            content += ",";

            content += kCurEffResDesc.m_EmitterNum.ToString();
            content += ",";

            content += kCurEffResDesc.m_AnimationNum.ToString();
            content += ",";

            content += kCurEffResDesc.m_TextureResDescList.Count.ToString();
            content += ",";

            string subContent = "";
            for (int j = 0, texum = kCurEffResDesc.m_TextureResDescList.Count; j < texum; ++j)
            {
                TextureResDesc curTexDesc = kCurEffResDesc.m_TextureResDescList[j];
                subContent += Path.GetFileName(curTexDesc.m_TexResPath) + "(MemSize:" + curTexDesc.m_TexResMemSize.ToString() + "MB W:" + curTexDesc.m_TexResWidth.ToString() + " H:" + curTexDesc.m_TexResHeight.ToString() + ")";
                subContent += "  |  ";
            }

            content += subContent;
            content += ",";

            sw.WriteLine(content);
        }

        streamW.Flush();
        sw.Close();
        streamW.Close();

    }

    static void _Clear()
    {
        m_EffectResStatiList.Clear();
    }
}
