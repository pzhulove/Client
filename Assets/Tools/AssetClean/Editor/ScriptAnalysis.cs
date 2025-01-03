using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CleanAssetsTool
{
    public class TypeStringInfo
    {
        public Type m_Type;
        public Dictionary<FieldInfo, TypeStringInfo> m_CandidateField = new Dictionary<FieldInfo, TypeStringInfo>();
        public bool m_LoopNode = false;

        public string GetShowInfo(FieldInfo fieldInfo, int layer)
        {
            string ret = "";
            for(int i = 0; i < layer; ++i)
            {
                ret += "\t";
            }

            if(fieldInfo == null)
                ret += m_Type.Name + ":" + Environment.NewLine;
            else
                ret += fieldInfo.ToString() + Environment.NewLine;

            var itr = m_CandidateField.GetEnumerator();
            while(itr.MoveNext())
            {
                ret += itr.Current.Value.GetShowInfo(itr.Current.Key, layer + 1);

                if (fieldInfo == null)
                    ret += Environment.NewLine;
            }

            return ret;
        }
    }

    public static class ScriptAnalysis
    {
        public static Dictionary<Type, TypeStringInfo> m_TypeStringInfos = new Dictionary<Type, TypeStringInfo>();
        public static HashSet<Type> m_NoStringInfoTypes = new HashSet<Type>();
        public static HashSet<Type> m_CheckingTypes = new HashSet<Type>();

        private static HashSet<Type> m_StringTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(string[]),
            typeof(List<string>),
        };
        
        public static void Clear()
        {
            m_TypeStringInfos.Clear();
            m_NoStringInfoTypes.Clear();
        }

        public static void GetClassStringInfo(string scriptFilePath)
        {

        }

        public static TypeStringInfo GetClassStringInfo(MonoScript script)
        {
            Type scriptType = script.GetClass();
            if(CollectsSerilizableStringFieldType(scriptType))
            {
                return m_TypeStringInfos[scriptType];
            }

            return null;
        }

        private static bool IsSerilizable(FieldInfo field, bool bPublic)
        {
            Type serilizeType = typeof(SerializeField);
            Type nonSerilizeType = typeof(NonSerializedAttribute);

            if (bPublic)
            {
                object[] attributes = field.GetCustomAttributes(false);
                foreach (object att in attributes)
                {
                    if (att.GetType() == nonSerilizeType)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                object[] attributes = field.GetCustomAttributes(false);
                foreach (object att in attributes)
                {
                    if (att.GetType() == serilizeType)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static bool IsTypeIgnored(Type type)
        {
            // 如果是UnityEngine命名空间类型，不检测
            if (!string.IsNullOrEmpty(type.Namespace) && (type.Namespace.Contains("UnityEngine") ||
                type.Namespace.Contains("BehaviorDesigner.Runtime") ||
                type.Namespace.Contains("behaviac") ||
                type.Namespace.Contains("DG.Tweening") 
                )
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 收集一个类型包含string类型的成员信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CollectsSerilizableStringFieldType(Type type)
        {
            if (m_TypeStringInfos.ContainsKey(type))
                return true;
            else if (m_NoStringInfoTypes.Contains(type))
                return false;

            Type listType = typeof(System.Collections.IList);

            TypeStringInfo typeInfo = new TypeStringInfo();
            typeInfo.m_Type = type;

            // 如果直接是string相关类型，返回true。
            if(IsFieldTypeString(type))
            {
                m_TypeStringInfos.Add(type, typeInfo);
                return true;
            }

            // 如果正在检查中，说明是循环引用，先返回true，后面检测完后再处理。
            if (m_CheckingTypes.Contains(type))
            {
                typeInfo.m_LoopNode = true;
                m_TypeStringInfos.Add(type, typeInfo);
                return true;
            }

            m_CheckingTypes.Add(type);

            // 到这里，说明是其他类型，需要判断其filed
            bool bHasString = false;

            List<FieldInfo> serilizedFields = new List<FieldInfo>();

            Type baseType = type;
            while (baseType != null)
            {
                if(IsTypeIgnored(baseType))
                {
                    break;
                }

                FieldInfo[] publicFields = baseType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                FieldInfo[] nonPublicFields = baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                if (publicFields.Length > 0)
                {
                    for (int i = 0; i < publicFields.Length; ++i)
                    {
                        if (IsSerilizable(publicFields[i], true))
                            serilizedFields.Add(publicFields[i]);
                    }
                }

                for (int i = 0; i < nonPublicFields.Length; ++i)
                {
                    FieldInfo fieldInfo = nonPublicFields[i];

                    if (IsSerilizable(nonPublicFields[i], false))
                        serilizedFields.Add(nonPublicFields[i]);
                }

                // 不需要迭代父类，上面的GetFields能获取到父类的信息。
                baseType = null; 
                // baseType = baseType.BaseType;
            }

            // 判断其可序列化filed中是否有string相关类型
            for (int i = 0; i < serilizedFields.Count; ++i)
            {
                FieldInfo fieldInfo = serilizedFields[i];
                Type fieldType = fieldInfo.FieldType;

                // 如果是List<T>或者数组类型
                if ((fieldType.IsGenericType && listType.IsAssignableFrom(fieldType)))
                {
                    fieldType = fieldType.GetGenericArguments()[0];
                    if(fieldType == typeof(string))
                    {
                        fieldType = fieldInfo.FieldType;
                    }
                }

                if (fieldType.IsArray)
                {
                    fieldType = fieldType.GetElementType();
                    if (fieldType == typeof(string))
                    {
                        fieldType = fieldInfo.FieldType;
                    }
                }

                if (IsTypeIgnored(fieldType))
                {
                    continue;
                }

                if (CollectsSerilizableStringFieldType(fieldType))
                {
                    // 如果是list或数组，将数组类型也进行登记
                    if(fieldType != fieldInfo.FieldType && !m_TypeStringInfos.ContainsKey(fieldInfo.FieldType))
                    {
                        m_TypeStringInfos.Add(fieldInfo.FieldType, m_TypeStringInfos[fieldType]);
                    }

                    typeInfo.m_CandidateField.Add(fieldInfo, m_TypeStringInfos[fieldInfo.FieldType]);
                    bHasString = true;
                }
            }

            m_CheckingTypes.Remove(type);

            if (bHasString)
            {
                // 检查循环引用
                {
                    TypeStringInfo loopTypeInfo;
                    if (m_TypeStringInfos.TryGetValue(type, out loopTypeInfo))
                    {
                        if(!loopTypeInfo.m_LoopNode)
                        {
                            WarningWindow.PushError("loopTypeInfo.m_LoopNode shold bu true");
                        }

                        // 将之前占位的去掉
                        m_TypeStringInfos.Remove(type);

                        // 如果有它的数组类型也登记了上面的typeInfo，也修改为新的。
                        List<Type> sameType = new List<Type>();
                        foreach(var itr in m_TypeStringInfos)
                        {
                            if(itr.Value == loopTypeInfo)
                            {
                                sameType.Add(itr.Key);
                            }
                        }

                        foreach (var itr in sameType)
                        {
                            m_TypeStringInfos[itr] = typeInfo;
                        }
                    }
                }


                m_TypeStringInfos.Add(type, typeInfo);
                return true;
            }
            else
            {
                m_NoStringInfoTypes.Add(type);
                return false;
            }
        }

        /// <summary>
        /// 判断一个类型是否直接包含string数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsFieldTypeString(Type type)
        {
            foreach(var stringType in m_StringTypes)
            {
                if (stringType == type)
                    return true;
            }

            return false;
        }


    }
}
