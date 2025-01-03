using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using Tenmove.Runtime.Unity;


namespace Tenmove.Editor.Unity
{
    [CustomEditor(typeof(PostprocessProfile))]
    public class PostprocessProfileEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/创建后处理")]
        public static void CreatePostprocessProfile()
        {
            if (Selection.objects == null || Selection.objects.Length < 1 || AssetDatabase.GetAssetPath(Selection.objects[0]) == string.Empty)
            {
                UnityEngine.Debug.LogError("请选择要处理的文件夹");
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            string absolutePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
            // 不是个path
            if (!System.IO.Directory.Exists(absolutePath))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            var a = ScriptableObject.CreateInstance<PostprocessProfile>();
            AssetDatabase.CreateAsset(a, path + "/PostprocessProfile.asset");
            AssetDatabase.SaveAssets();
        }

        PostprocessProfile m_Profile;

        List<Type> settingTypes = new List<Type>();

        PostprocessLayer m_Layer;

        Type effectTypeToAdd;

        private void OnEnable()
        {
            m_Profile = target as PostprocessProfile;
            Assert.IsNotNull(m_Profile);

            //反射获取所有PostprocessEffectSettings的子类
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(PostprocessEffectSettings))
                        && !type.IsSealed)
                    {
                        settingTypes.Add(type);
                    }
                }
            }
        }

        private void OnDisable()
        {
            m_Profile = null;
        }

        public override void OnInspectorGUI()
        {
            if (m_Profile.Effects.Count > 0)
            {
                for (int i = 0; i < m_Profile.Effects.Count; i++)
                {
                    var effect = m_Profile.Effects[i];

                    Type type = effect.GetType();
                    GUI.color = Color.yellow;
                    EditorGUILayout.LabelField(type.Name, EditorStyles.toolbarButton);
                    GUI.color = Color.white;

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            var fields = type.GetFields();
                            foreach (FieldInfo field in fields)
                            {
                                object value = field.GetValue(effect);
                                Type fieldType = field.FieldType;
                                if (fieldType == typeof(bool))
                                {
                                    field.SetValue(effect, EditorGUILayout.Toggle(field.Name, (bool)value));
                                }
                                if (fieldType == typeof(int))
                                {
                                    field.SetValue(effect, EditorGUILayout.IntField(field.Name, (int)value));
                                }
                                if (fieldType.BaseType == typeof(Enum))
                                {
                                    field.SetValue(effect, EditorGUILayout.EnumPopup(field.Name, (Enum)value));
                                }
                                if (fieldType == typeof(float))
                                {
                                    object[] rangeAttributes = field.GetCustomAttributes(false);
                                    if (rangeAttributes != null && rangeAttributes.Length > 0 && rangeAttributes[0] is RangeAttribute)
                                    {
                                        RangeAttribute range = (RangeAttribute)rangeAttributes[0];
                                        field.SetValue(effect, EditorGUILayout.Slider(field.Name, (float)value, range.min, range.max));
                                    }
                                    else
                                        field.SetValue(effect, EditorGUILayout.FloatField(field.Name, (float)value));
                                }
                                if (fieldType == typeof(Color))
                                {
                                    field.SetValue(effect, EditorGUILayout.ColorField(field.Name, (Color)value));
                                }
                                if (fieldType == typeof(Vector2))
                                {
                                    field.SetValue(effect, EditorGUILayout.Vector2Field(field.Name, (Vector2)value));
                                }
                                if (fieldType == typeof(Vector3))
                                {
                                    field.SetValue(effect, EditorGUILayout.Vector3Field(field.Name, (Vector3)value));
                                }
                                if (fieldType == typeof(Vector4))
                                {
                                    field.SetValue(effect, EditorGUILayout.Vector4Field(field.Name, (Vector4)value));
                                }
                                if (fieldType == typeof(AnimationCurve))
                                {
                                    field.SetValue(effect, EditorGUILayout.CurveField(field.Name, (AnimationCurve)value));
                                }
                                if (fieldType == typeof(Texture2D))
                                {
                                    field.SetValue(effect, EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)value, typeof(Texture2D), false));
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.color = Color.red;
                    if (GUILayout.Button("移除"))
                    {
                        RemoveEffect(i);
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.Space();
                }
            }

            GUI.color = Color.green;
            if (GUILayout.Button("添加后处理"))
            {
                var menu = new GenericMenu();

                foreach (var type in settingTypes)
                {
                    bool exists = _HasEffect(type);

                    if (!exists)
                        menu.AddItem(new GUIContent(type.Name), false, () => AddEffectMenuOnClick(type));
                    else
                        menu.AddDisabledItem(new GUIContent(type.Name));
                }
                menu.ShowAsContext();
            }
            GUI.color = Color.white;


            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_Profile);
                OverriderLayerSettings();
            }

            if(effectTypeToAdd != null)
            {
                AddEffect(effectTypeToAdd);
                effectTypeToAdd = null;
            }
        }

        public void AddEffectMenuOnClick(Type type)
        {
            effectTypeToAdd = type;
        }

        public void AddEffect(Type type)
        {
            if (_HasEffect(type))
            {
                Debug.Log(type.Name + "效果已经存在");
                return;
            }

            var effect = (PostprocessEffectSettings)CreateInstance(type);
            effect.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            effect.name = type.Name;

            m_Profile.Effects.Add(effect);
            m_Profile.Effects.Sort((x, y) => { return ((x.EffectType)).CompareTo(y.EffectType); });

            UnityEditor.AssetDatabase.AddObjectToAsset(effect, m_Profile);
            UnityEditor.EditorUtility.SetDirty(m_Profile);
            UnityEditor.AssetDatabase.SaveAssets();

            OverriderLayerSettings();
        }

        public void RemoveEffect(int index)
        {
            if (m_Profile.Effects.Count - 1 >= index)
            {
                DestroyImmediate(m_Profile.Effects[index], true);
                m_Profile.Effects.RemoveAt(index);
                UnityEditor.EditorUtility.SetDirty(m_Profile);
                UnityEditor.AssetDatabase.SaveAssets();

            }
            else
            {
                Debug.Log("删除后处理错误:Out Of Range~");
            }
        }

        private bool _HasEffect(Type type)
        {
            foreach (var tempEffect in m_Profile.Effects)
            {
                if (type == tempEffect.GetType())
                {
                    return true;
                }
            }
            return false;
        }

        private void OverriderLayerSettings()
        {
            if(m_Layer == null)
            {
                List<Camera> cameras = new List<Camera>(Camera.allCameras);
                foreach(var camera in cameras)
                {
                    PostprocessLayer postprocessLayer = camera.gameObject.GetComponent<PostprocessLayer>();
                    if(postprocessLayer != null)
                    {
                        m_Layer = postprocessLayer;
                        break;
                    }
                }
            }

            if(m_Layer != null)
            {
                m_Layer.OverrideSettings(m_Profile);
            }
        }
    }
}


