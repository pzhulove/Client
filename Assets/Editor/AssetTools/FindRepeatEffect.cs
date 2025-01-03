using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets
{
    public class FindRepeatEffect : EditorWindow
    {
        private Vector2 _vec2Scroll;
        private int offset = 2;

        [MenuItem("[技能编辑器]/检查重复特效")]
        public static void FindMissingRes()
        {
            var window = GetWindow<FindRepeatEffect>();
            window.titleContent = new GUIContent("检查技能配置中的重复特效");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("特效开始帧间隔<=", GUILayout.Width(100));
            offset = EditorGUILayout.IntField(offset, GUILayout.Width(50));
            if (GUILayout.Button("开始查找", GUILayout.Width(100)))
            {
                CheckEffectsOfSkillConfigs();
            }
            EditorGUILayout.EndHorizontal();

            if (path2Effects.Count > 0)
            {
                GUILayout.Label("");
                GUILayout.Label("  以下列出查找到的技能配置，共" + path2Effects.Count + "个:");
            }

            _vec2Scroll = EditorGUILayout.BeginScrollView(_vec2Scroll, GUILayout.Width(1000));
            for (int i = maxCount; i > 0; --i)
            {
                if (!effectCount2Path.ContainsKey(i))
                {
                    continue;
                }
                foreach (var item in effectCount2Path[i])
                {
                    if (!string.IsNullOrEmpty(item) && path2Effects.ContainsKey(item))
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("引用", GUILayout.Width(50)))
                        {
                            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item);
                            if (obj != null)
                            {
                                EditorGUIUtility.PingObject(obj);
                            }
                        }

                        EditorGUILayout.BeginVertical();
                        GUILayout.Label(item);
                        foreach (var effect in path2Effects[item])
                        {
                            GUILayout.Label(effect);
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        Dictionary<string, List<string>> path2Effects = new Dictionary<string, List<string>>();
        Dictionary<int, List<string>> effectCount2Path = new Dictionary<int, List<string>>();
        int maxCount = 0;

        private void CheckEffectsOfSkillConfigs()
        {
            Dictionary<string, Dictionary<string, List<int>>> all = new Dictionary<string, Dictionary<string, List<int>>>();

            //string filePath = Path.Combine(Application.dataPath, "repeatEffects.txt");
            //File.Delete(filePath);

            path2Effects.Clear();
            effectCount2Path.Clear();
            maxCount = 0;

            string skillDataPath = "Assets/Resources/Data/SkillData";
            var assetGuids = AssetDatabase.FindAssets("t:DSkillData", new string[] { skillDataPath });
            foreach (var guid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var skillData = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DSkillData)) as DSkillData;
                if (skillData == null)
                {
                    continue;
                }

                if (!all.ContainsKey(assetPath))
                {
                    all.Add(assetPath, new Dictionary<string, List<int>>());
                }

                var effects = skillData.effectFrames;
                foreach (var effect in effects)
                {
                    var path = effect.effectAsset.m_AssetPath;
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }
                    if (!all[assetPath].ContainsKey(path))
                    {
                        all[assetPath].Add(path, new List<int>());
                    }
                    all[assetPath][path].Add(effect.startFrames);
                }
            }

            //using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                var enumerator = all.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    bool has = false;
                    if (enumerator.Current.Value.Count > 0)
                    {
                        var enumertor1 = enumerator.Current.Value.GetEnumerator();
                        while (enumertor1.MoveNext())
                        {
                            var startFrames = enumertor1.Current.Value;
                            if (startFrames.Count > 1)
                            {
                                bool exist = false;
                                string str = "";
                                for (int i = 0; i < startFrames.Count; i++)
                                {
                                    if (str.Length > 0)
                                    {
                                        str += ",";
                                    }
                                    str += startFrames[i].ToString();

                                    for (int j = 0; j < startFrames.Count; j++)
                                    {
                                        if (i != j && Math.Abs(startFrames[i] - startFrames[j]) <= offset)
                                        {
                                            exist = true;
                                            break;
                                        }
                                    }
                                }

                                if (exist)
                                {
                                    if (!has)
                                    {
                                        has = true;
                                        //sw.WriteLine(enumerator.Current.Key);
                                    }
                                    str = "[" + str + "] " + enumertor1.Current.Key;
                                    //sw.WriteLine(str);

                                    if (!path2Effects.ContainsKey(enumerator.Current.Key))
                                    {
                                        path2Effects.Add(enumerator.Current.Key, new List<string>());
                                    }
                                    path2Effects[enumerator.Current.Key].Add(str);

                                    if (!effectCount2Path.ContainsKey(startFrames.Count))
                                    {
                                        effectCount2Path.Add(startFrames.Count, new List<string>());
                                    }
                                    effectCount2Path[startFrames.Count].Add(enumerator.Current.Key);

                                    if(startFrames.Count > maxCount)
                                    {
                                        maxCount = startFrames.Count;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}