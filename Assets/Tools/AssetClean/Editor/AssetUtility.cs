using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    static class AssetUtility
    {
        public static string m_RecentSavePath = "D:";
        public static List<string> m_ResourceDirs;

        public static string[] GetDependencies(string assetName, bool recursive = true)
        {
            return AssetDatabase.GetDependencies(assetName, recursive);
        }

        public static string[] FindAllAssets(string dir, string filter)
        {
            string[] guids = AssetDatabase.FindAssets(filter, new string[] { dir });

            string[] names = new string[guids.Length];
            for (int i = 0; i < guids.Length; ++i)
            {
                names[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }


            return names;
        }

        public static string[] FindAllPrafabs(string dir)
        {
            return FindAllAssets(dir, "t:Prefab");
        }

        public static string[] FindAllScriptableObjects(string dir)
        {
            return FindAllAssets(dir, "t:ScriptableObject");
        }

        public static string[] FindAllTextures(string dir)
        {
            return FindAllAssets(dir, "t:texture");
        }

        public static string[] FindAllMaterials(string dir)
        {
            return FindAllAssets(dir, "t:material");
        }

        public static string[] FindAllShaders(string dir)
        {
            return FindAllAssets(dir, "t:shader");
        }

        public static string[] FindAllAnimationClips(string dir)
        {
            return FindAllAssets(dir, "t:animationclip");
        }

        public static string[] FindAllMeshs(string dir)
        {
            return FindAllAssets(dir, "t:mesh");
        }

        public static string[] FindAllScripts(string dir)
        {
            return FindAllAssets(dir, "t:Script");
        }

        public static string[] FindAllAssets(string dir)
        {
            return FindAllAssets(dir, "");
        }

        public static void SetSelectionInProjectWindow(string assetName)
        {
            if (assetName.StartsWith("Assets/"))
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
        }

        /// <summary>
        /// 判断一个字符串是否是资源名，如果是，将pathName规范为以"Assets/Resources"开头的带后缀的名称。
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static bool IsAssetPath(ref string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
                return false;

            if (m_ResourceDirs == null)
            {
                m_ResourceDirs = new List<string>();

                string[] dirs = Directory.GetDirectories(Application.dataPath + "/Resources");

                foreach (var dir in dirs)
                {
                    m_ResourceDirs.Add(Path.GetFileName(dir) + "/");
                }
            }

            bool bAssetCandidate = false;

            if (pathName.StartsWith("Assets/Resources/"))
            {
                bAssetCandidate = true;
                pathName = pathName.Replace("Assets/Resources/", null);
            }
            else
            {
                foreach (var dir in m_ResourceDirs)
                {
                    if (pathName.StartsWith(dir))
                    {
                        bAssetCandidate = true;
                        break;
                    }
                }
            }

            if (bAssetCandidate)
            {
                // 去掉sprite路径后面的sprite名称
                int split = pathName.LastIndexOf(':');
                if (split > 0)
                {
                    pathName = pathName.Substring(0, split);
                }

                string extension = "";

                try
                {
                    extension = Path.GetExtension(pathName);
                }
                catch (ArgumentException ex) // 非法路径会抛这个异常
                {
                    WarningWindow.PushWarning("疑似资源路径被忽略：{0}", pathName);
                    return false;
                }

                // 如果有扩展名，用AssetDatabase加载，更准确。否则用Resources load检测。
                if (!string.IsNullOrEmpty(extension))
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

                    // AI和SkillData的json fileList是拼凑出来的
                    string jsonFile = GenSkillFileList2(pathName);
                    asset = Resources.Load(jsonFile);
                    if (asset != null)
                    {
                        pathName = AssetDatabase.GetAssetPath(asset);
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GenSkillFileList2(string fullPath)
        {
            string folderName = Utility.GetPathLastName(fullPath);
            string jsonFileName = fullPath + "/" + folderName + "_FileList";

            return jsonFileName;
        }
    }
}
