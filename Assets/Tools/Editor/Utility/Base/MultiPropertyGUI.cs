using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Tenmove.Runtime.Unity;

/// <summary>
/// 多选情况下的编辑器GUI
/// 支持一个接口根据Attribute绘制数据
/// </summary>
class MultiPropertyGUI
{
    /// <summary>
    /// 绘制多选数据，绘制所有Public的属性/字段交集
    /// 支持使用Attribute：MultiPropertyGUIAttribute，FontStyleAttribute，SpaceAttribute, ConditionDrawAttribute
    /// </summary>
    /// <param name="infos">绘制数据</param>
    public static void DrawMultiProperty(object[] infos)
    {
        if(infos == null)
            return;
        
        var fields = GetSameMemberList(infos);
        for (int i = 0; i < fields.Count; i++)
        {
            var curField = fields[i];
            if(!CheckCondition(curField, infos))
                continue;
            
            if (!curField.multiPropertyGUIAtt.needDraw)
                continue;
            
            if (!curField.multiPropertyGUIAtt.canEdit)
            {
                GUIStyle style = GetGUIStyle(curField.mFontStyleAtt);
                LabelField(infos, curField.name, curField.multiPropertyGUIAtt.description, style);
            }
            else
            {
                if (curField.baseType == typeof(Enum))
                {
                    Popup(infos, curField.name, curField.multiPropertyGUIAtt.description, curField.memberType.GetDescriptions());
                }
                else
                {
                    if (curField.memberType == typeof(string))
                    {
                        TextField(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(Vector2))
                    {
                        Vector2Field(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(Vector3))
                    {
                        Vector3Field(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(int))
                    {
                        Type attrType = curField.multiPropertyGUIAtt.type;
                        
                        if (null != attrType && typeof(FlatBuffers.IFlatbufferObject).IsAssignableFrom(attrType))
                        {
                            var desc = curField.multiPropertyGUIAtt.description;
                            var ins = TableInspetor.Get(attrType, desc);
                            var filedname = curField.name;
                            foreach (var cur in infos)
                            {
                                int targetValue = (int)GetMemberValue(cur, filedname);
                                var newID = ins.OnGUIWithID(targetValue);
                                if (newID != targetValue)
                                {
                                    OnChange<int>(infos, filedname, newID, null);
                                }
                            }
                        }
                        else 
                        {
                            IntField(infos, curField.name, curField.multiPropertyGUIAtt.description);
                        }
                    }
                    else if (curField.memberType == typeof(Color))
                    {
                        ColorField(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(float))
                    {
                        FloatField(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(bool))
                    {
                        Toggle(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if (curField.memberType == typeof(Quaternion))
                    {
                        QuaternionField(infos, curField.name, curField.multiPropertyGUIAtt.description);
                    }
                    else if(curField.memberType == typeof(DAssetObject))
                    {
                        var ObjectType = typeof(UnityEngine.Object);
                        Action<object, DAssetObject> action = null;
                        if(curField.name.Equals("materialAsset"))
                        {
                            ObjectType = typeof(Material);
                            action = (info, fieldValue) =>
                            {
                                var doorInfo = info as DTransportDoor;
                                if (doorInfo != null && fieldValue.m_AssetObj != null)
                                {
                                    var materialReplacer = doorInfo.obj.GetComponent<MaterialReplacerComponent>();
                                    var material = fieldValue.m_AssetObj as Material;
                                    if (materialReplacer != null && material != null)
                                    {
                                        materialReplacer.SetDoorMaterial(material);
                                    }
                                }
                            };
                        }
                        DAssetObject(infos, curField.name, curField.multiPropertyGUIAtt.description, typeof(Material), action);
                    }
                }
            }
            
            for (int j = 0; j < curField.spaceCount; j++)
            {
                EditorGUILayout.Space();
            }
        }
    }

    private static bool CheckCondition(MemberData member, object[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < member.mConditionList.Count; j++)
            {
                var condition = member.mConditionList[j];
                var value = GetMemberValue(data[i], condition.name);
                if (!ConditionFunction(condition.option, value, condition.value))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static object GetMemberValue(object data, string name)
    {
        object obj = null;
        var curType = data.GetType();
        //字段
        var field = curType.GetField(name);
        if(field != null)
        {
            obj = field.GetValue(data);
        }
        //属性(get.set)
        var property = curType.GetProperty(name);
        if (property != null)
        {
            obj = property.GetValue(data, null);
        }

        return obj;
    }

    private static bool ConditionFunction(ConditionDrawAttribute.Option option, object l, object r)
    {
        if (option == ConditionDrawAttribute.Option.Equal)
            return l.Equals(r);

        if (option == ConditionDrawAttribute.Option.UnEqual)
        {
            return !l.Equals(r);
        }

        return false;
    }
    
    private static GUIStyle GetGUIStyle(FontStyleAttribute style)
    {
        if (style == null)
            return null;
        
        var fontStyle = new GUIStyle();
        fontStyle.fontSize = style.fontSize;
        fontStyle.normal.textColor = style.textColor;
        fontStyle.hover.textColor = style.textColor;

        return fontStyle;
    }
    
    private static T GetAttribute<T>(MemberInfo member)
    {
        if (member == null)
            return default(T);

        var attributes = member.GetCustomAttributes(typeof(T), false);
        if (attributes != null && attributes.Length > 0)
        {
            T att = (T)attributes[0];
            return att;
        }

        return default(T);
    }
    
    
    private static List<T> GetAttributes<T>(MemberInfo member)
    {
        if (member == null)
            return new List<T>();

        var attributes = member.GetCustomAttributes(typeof(T), false);
        return new List<T>(attributes as T[]);
    }
    
    class MemberData
    {
        public string name;
        public bool isField;

        public MultiPropertyGUIAttribute multiPropertyGUIAtt;
        public FontStyleAttribute mFontStyleAtt;
        public List<ConditionDrawAttribute> mConditionList; 
        public Type memberType;
        public Type baseType;
        public int spaceCount;

        public MemberData(string _name, bool _isField)
        {
            name = _name;
            isField = _isField;
            memberType = null;
            baseType = null;
            multiPropertyGUIAtt = null;
            mFontStyleAtt = null;
            mConditionList = new List<ConditionDrawAttribute>();
            spaceCount = 0;
        }

        public void Init(Type info)
        {
            MemberInfo mem;
            if (isField)
            {
                var field = info.GetField(name);
                memberType = field.FieldType;
                baseType = memberType.BaseType;
                mem = field;
            }
            else
            {
                var property = info.GetProperty(name);
                memberType = property.PropertyType;
                baseType = memberType.BaseType;
                mem = property;
            }
            var att = GetAttribute<MultiPropertyGUIAttribute>(mem);
            if (att != null)
            {
                multiPropertyGUIAtt = att;
            }
            else
            {
                multiPropertyGUIAtt = new MultiPropertyGUIAttribute(name, 1, false, false);
            }
            
            mFontStyleAtt = GetAttribute<FontStyleAttribute>(mem);

            var spaceAtt = GetAttribute<SpaceAttribute>(mem);
            if (spaceAtt != null)
            {
                spaceCount = (int)spaceAtt.height;
            }

            mConditionList = GetAttributes<ConditionDrawAttribute>(mem);
        }
        
        public bool Equals(MemberData other)
        {
            return string.Equals(name, other.name, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MemberData && Equals((MemberData) obj);
        }

        public override int GetHashCode()
        {
            return (name != null ? StringComparer.InvariantCulture.GetHashCode(name) : 0);
        }
    }
    
    private static List<MemberData> GetSameMemberList(object[] infos)
    {
        if (infos == null)
            return null;

        if (infos.Length <= 0)
            return null;
        
        List<MemberData> sameList = new List<MemberData>();
        sameList.AddRange(GetFieldMemberData(infos));
        sameList.AddRange(GetPropertyMemberData(infos));
        foreach (var data in sameList)
        {
            data.Init(infos[0].GetType());
        }
        sameList.Sort((l, r) => l.multiPropertyGUIAtt.order - r.multiPropertyGUIAtt.order);
        return sameList;
    }

    private static List<MemberData> GetFieldMemberData(object[] infos)
    {
        if (infos == null)
            return null;

        if (infos.Length <= 0)
            return null;
                
        List<MemberData> sameFieldList = GetMemberData(infos[0].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public), true);
        for (int i = 1; i < infos.Length; i++)
        {
            var curType = infos[i].GetType();
            List<MemberData> theList = GetMemberData(curType.GetFields(BindingFlags.Instance | BindingFlags.Public), true);
            sameFieldList = sameFieldList.Intersect(theList);
        }

        return sameFieldList;
    }
    
    private static List<MemberData> GetPropertyMemberData(object[] infos)
    {
        if (infos == null)
            return null;

        if (infos.Length <= 0)
            return null;
                
        List<MemberData> sameFieldList = GetMemberData(infos[0].GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public), false);
        for (int i = 1; i < infos.Length; i++)
        {
            var curType = infos[i].GetType();
            List<MemberData> theList = GetMemberData(curType.GetProperties(BindingFlags.Instance | BindingFlags.Public), false);
            sameFieldList = sameFieldList.Intersect(theList);
        }

        return sameFieldList;
    }
    
    private static List<MemberData> GetMemberData(MemberInfo[] infos, bool isField)
    {
        List<MemberData> theList = new List<MemberData>();
        for (int i = 0; i < infos.Length; i++)
        {
            theList.Add(new MemberData(infos[i].Name, isField));
        }
        return theList;
    }

    public static int IntField(object[] infos, string property, string label, Action<object, int> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<int>(infos, property, 0, content =>
        {
            var fieldValue = EditorGUILayout.IntField(label, content, options);
            if (fieldValue != content)
            {
                OnChange<int>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }

    public static float FloatField(object[] infos, string property, string label, Action<object, float> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<float>(infos, property, 0f, content =>
        {
            var fieldValue = EditorGUILayout.FloatField(label, content, options);
            if (fieldValue != content)
            {
                OnChange<float>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }
    
    public static void LabelField(object[] infos, string property, string label, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        MultiPropertyAction<string>(infos, property, "-", content =>
        {
            if (style == null)
            {
                EditorGUILayout.LabelField(label, content, options);
            }
            else
            {
                EditorGUILayout.LabelField(label, content, style, options);
            }

            return "";
        });
    }

    public static string TextField(object[] infos, string property, string label,
        Action<object, string> OnChange = null, params GUILayoutOption[] options)
    {
        return MultiPropertyAction<string>(infos, property, "-", content =>
        {
            var fieldValue = EditorGUILayout.TextField(label, content, options);
            if (fieldValue != content)
            {
                OnChange<string>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }

    public static Vector2 Vector2Field(object[] infos, string property, string label,
        Action<object, Vector2> OnChange = null, params GUILayoutOption[] options)
    {
        return MultiPropertyAction<Vector2>(infos, property, Vector2.zero, content =>
        {
            var fieldValue = EditorGUILayout.Vector2Field(label, content, options);
            if (fieldValue != content)
            {
                OnChange<Vector2>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }

    public static Vector3 Vector3Field(object[] infos, string property, string label,
        Action<object, Vector3> OnChange = null, params GUILayoutOption[] options)
    {
        return MultiPropertyAction<Vector3>(infos, property, Vector3.zero, content =>
        {
            var fieldValue = EditorGUILayout.Vector3Field(label, content, options);
            if (fieldValue != content)
            {
                OnChange<Vector3>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }

    public static Quaternion QuaternionField(object[] infos, string property, string label,
        Action<object, Quaternion> OnChange = null, params GUILayoutOption[] options)
    {
        return MultiPropertyAction<Quaternion>(infos, property, Quaternion.identity, content =>
        {
            var fieldValue = EditorGUILayout.Vector3Field(label, content.eulerAngles, options);
            if (fieldValue != content.eulerAngles)
            {
                OnChange<Quaternion>(infos, property, Quaternion.Euler(fieldValue), OnChange);
            }

            return Quaternion.Euler(fieldValue);
        });
    }

    public static Color ColorField(object[] infos, string property, string label, Action<object, Color> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<Color>(infos, property, Color.black, content =>
        {
            var fieldValue = EditorGUILayout.ColorField(label, content, options);
            if (fieldValue != content)
            {
                OnChange<Color>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }

    public static DAssetObject DAssetObject(object[] infos, string property, string label, Type ObjectType, Action<object, DAssetObject> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<DAssetObject>(infos, property, default(DAssetObject), content =>
        {
            if (content.m_AssetObj == null)
            {
                content.m_AssetObj = null;
            }
            if (content.m_AssetObj == null && !string.IsNullOrEmpty(content.m_AssetPath))
            {
                content.m_AssetObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(content.m_AssetPath);
            }
            var obj = EditorGUILayout.ObjectField("Object", content.m_AssetObj, ObjectType, false);
            var fieldValue = content;
            if (obj != content.m_AssetObj)
            {
                DAssetObject assetObject = new DAssetObject(obj, FileTools.GetAssetFullPath(obj));
                fieldValue = assetObject;
                OnChange<DAssetObject>(infos, property, assetObject, OnChange);
            }
            EditorGUILayout.LabelField("Path", fieldValue.m_AssetPath);

            return fieldValue;
        });
    }

    public static int Popup(object[] infos, string property, string label, string[] enumDescs,
        Action<object, int> OnChange = null, params GUILayoutOption[] options)
    {
        return MultiPropertyAction<int>(infos, property, default(int), content =>
        {
            var fieldValue = EditorGUILayout.Popup(label, content, enumDescs, options);
            if (fieldValue != content)
            {
                OnChange<int>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }
    
    // 接口废弃
    // default(Enum)得到的null，不知道怎么得到默认值
    // 可使用 Popup 接口
    /*public static Enum EnumPopup(object[] infos, string property, string label, Action<object, Enum> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<Enum>(infos, property, default(Enum), content =>
        {
            var fieldValue = EditorGUILayout.EnumPopup(label, content, options);
            if (fieldValue != content)
            {
                OnChange<Enum>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }*/
    
    public static bool Toggle(object[] infos, string property, string label, Action<object, bool> OnChange = null,
        params GUILayoutOption[] options)
    {
        return MultiPropertyAction<bool>(infos, property, default(bool), content =>
        {
            var fieldValue = EditorGUILayout.Toggle(label, content, options);
            if (fieldValue != content)
            {
                OnChange<bool>(infos, property, fieldValue, OnChange);
            }

            return fieldValue;
        });
    }


    public static bool IsSame<T>(object[] infos, string propertyName, ref T fieldValue)
    {
        bool isSame = true;
        bool isFirst = true;
        fieldValue = default(T);
        foreach (var item in infos)
        {
            object obj = GetMemberValue(item, propertyName);
            
            T v;
            if (obj != null)
            {
                if (typeof(T) == typeof(string) && obj.GetType() != typeof(string))
                {
                    var m = obj.ToString();
                    v = (T) (object) m;
                }
                else
                {
                    v = (T) obj;
                }

                if (isFirst)
                {
                    fieldValue = v;
                    isFirst = false;
                }
                else
                {
                    if (!fieldValue.Equals(v))
                    {
                        isSame = false;
                        break;
                    }
                }
            }
        }

        return isSame;
    }

    private static T MultiPropertyAction<T>(object[] infos, string property, T differentValue, Func<T, T> OnAction)
    {
        T fieldValue = default(T);
        if (IsSame(infos, property, ref fieldValue))
        {
            if (OnAction != null)
            {
                GUI.color = Color.white;
                return OnAction(fieldValue);
            }
        }
        else
        {
            if (OnAction != null)
            {
                var preColor = GUI.color;
                GUI.color = Color.grey;
                return OnAction(differentValue);
                GUI.color = Color.white;
            }
        }
        return default(T);
    }

    private static void OnChange<T>(object[] infos, string propertyName, T fieldValue, Action<object, T> onChange)
    {
        foreach (var info in infos)
        {
            GUI.changed = true;
            var curType = info.GetType();
            
            //字段
            var field = curType.GetField(propertyName);
            if(field != null)
            {
                field.SetValue(info, fieldValue);
            }
            //属性(get.set)
            var property = curType.GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(info, fieldValue, null);
            }
            
            if (onChange != null)
            {
                onChange.Invoke(info, fieldValue);
            }
        }
    }

    public static T[] FindProperty<T>(object[] infos, string property)
    {
        List<T> ret = new List<T>();
        foreach (var info in infos)
        {
            var proValue = info.GetType().GetField(property).GetValue(info);
            if (proValue != null)
            {
                var vl = (T)proValue;
                ret.Add(vl);
            }
        }

        return ret.ToArray();
    }

    public static T[] ConvertTo<T>(object[] infos)
    {
        if (infos != null)
        {
            if (infos.Length > 0)
            {
                List<T> ret = new List<T>();
                foreach (var info in infos)
                {
                    var isT = info is T;
                    if (isT)
                    {
                        ret.Add((T)info);    
                    }
                }
                return ret.ToArray();
            }
            return null;
        }

        return null;
    }
}
