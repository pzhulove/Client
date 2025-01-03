using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class CodeAnalyser
    {
        public static List<string> m_ResourceDirs;

        public static List<string> GetAllCandidateAssetPath(string scriptName)
        {
            if(CleanAssetsEditor.projectionVersion == CleanAssetsEditor.ProjectionVersion.One)
            {
                return _GetAllCandidateAssetPath1(scriptName);
            }
            else if(CleanAssetsEditor.projectionVersion == CleanAssetsEditor.ProjectionVersion.Two)
            {
                return _GetAllCandidateAssetPath2(scriptName);
            }
        }

        private static List<string> _GetAllCandidateAssetPath1(string scriptName)
        {
            List<string> ret = new List<string>();

            if (m_ResourceDirs == null)
            {
                m_ResourceDirs = new List<string>();

                string[] dirs = Directory.GetDirectories(Application.dataPath + "/Resources");

                foreach (var dir in dirs)
                {
                    m_ResourceDirs.Add("\"" + Path.GetFileName(dir) + "/");
                }
            }

            TextAsset txtAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptName);
            string strScript = txtAsset.text;

            foreach (string prefix in m_ResourceDirs)
            {
                int pos = 0;

                while (pos >= 0)
                {
                    pos = strScript.IndexOf(prefix, pos);
                    if (pos >= 0)
                    {
                        int nextQuotMark = strScript.IndexOf("\"", pos + 1);
                        if (nextQuotMark < 0)
                        {
                            WarningWindow.PushError("Script:{0}, nextQuotMark not found: {1}", scriptName, strScript.Substring(pos, 50));
                        }

                        string assetPath = strScript.Substring(pos + 1, nextQuotMark - pos - 1);

                        // 查看是否是合成路径
                        int bracePos = assetPath.IndexOf("{");
                        if (bracePos > 0)
                        {
                            // 有冒号且在大括号之前，说明是sprite路径，前面的图片路径是完整的。
                            int colonMark = assetPath.IndexOf(":");
                            if (colonMark > 0 && colonMark < bracePos)
                            {
                                assetPath = assetPath.Substring(0, colonMark);
                                if (IsValidAssetPath(ref assetPath))
                                {
                                    ret.Add(assetPath);
                                }
                            }
                            else
                            {
                                // 合成路径直接返回，外面处理。
                                ret.Add(assetPath);
                            }
                        }
                        else
                        {
                            if (IsValidAssetPath(ref assetPath))
                            {
                                ret.Add(assetPath);
                            }
                        }

                        pos += nextQuotMark - pos;
                    }
                }
            }

            return ret;
        }

        private static List<string> _GetAllCandidateAssetPath2(string scriptName)
        {
            List<string> ret = new List<string>();

            if (m_ResourceDirs == null)
            {
                m_ResourceDirs = new List<string>();

                string[] dirs = Directory.GetDirectories(Application.dataPath + "/Resources");

                foreach (var dir in dirs)
                {
                    m_ResourceDirs.Add("\"" + Path.GetFileName(dir) + "/");
                    m_ResourceDirs.Add("\"" + Path.GetFileName(dir) + "\\");
                }
            }

            TextAsset txtAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptName);
            string strScript = txtAsset.text;

            foreach(string prefix in m_ResourceDirs)
            {
                int pos = 0;

                while (pos >= 0)
                {
                    pos = strScript.IndexOf(prefix, pos);
                    if(pos >= 0)
                    {
                        int nextQuotMark = strScript.IndexOf("\"", pos + 1);
                        if(nextQuotMark < 0)
                        {
                            WarningWindow.PushError("Script:{0}, nextQuotMark not found: {1}", scriptName, strScript.Substring(pos, 50));
                        }

                        string assetPath = strScript.Substring(pos + 1, nextQuotMark - pos - 1);
                        assetPath = NormalizePath(assetPath);

                        // 查看是否是合成路径
                        int bracePos = assetPath.IndexOf("{");
                        if(bracePos > 0)
                        {
                            // 有冒号且在大括号之前，说明是sprite路径，前面的图片路径是完整的。
                            int colonMark = assetPath.IndexOf(":");
                            if (colonMark > 0 && colonMark < bracePos)
                            {
                                assetPath = assetPath.Substring(0, colonMark);
                                if (IsValidAssetPath(ref assetPath))
                                {
                                    ret.Add(assetPath);
                                }
                            }
                            else
                            { 
                                // 合成路径直接返回，外面处理。
                                ret.Add(assetPath);
                            }
                        }
                        else
                        {
                            if(IsValidAssetPath(ref assetPath))
                            {
                                ret.Add(assetPath);
                            }
                        }

                        pos += nextQuotMark - pos;
                    }
                }
            }

            return ret;
        }

        private static string NormalizePath(string pathName)
        {
            pathName = pathName.Replace("\\\\\\\\", "/");
            pathName = pathName.Replace("\\\\", "/");
            pathName = pathName.Replace("\\", "/");

            if (pathName.EndsWith("/"))
            {
                pathName = pathName.Substring(0, pathName.Length - 1);
            }

            return pathName;
        }

        private static bool IsValidAssetPath(ref string pathName)
        {
            // 去掉sprite路径后面的sprite名称
            int split = pathName.LastIndexOf(':');
            if (split > 0)
            {
                pathName = pathName.Substring(0, split);
            }

            try
            {
                // 如果有扩展名，用AssetDatabase加载，更准确。否则用Resources load检测。
                if (!string.IsNullOrEmpty(Path.GetExtension(pathName)))
                {
                    UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + pathName);
                    if (asset != null)
                    {
                        pathName = AssetDatabase.GetAssetPath(asset);
                        return true;
                    }
                }
                else
                {
                    UnityEngine.Object asset = Resources.Load(pathName);
                    if (asset != null)
                    {
                        pathName = AssetDatabase.GetAssetPath(asset);
                        return true;
                    }
                }
            }
            catch (ArgumentException ex) // 非法路径会抛这个异常
            {
                WarningWindow.PushWarning("疑似资源路径被忽略：{0}", pathName);
            }

            return false;
        }
    }
}
