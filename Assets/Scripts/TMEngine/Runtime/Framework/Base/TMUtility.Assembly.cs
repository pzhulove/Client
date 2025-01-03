
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        /// <summary>
        /// 程序集相关的实用函数。
        /// </summary>
        public static partial class Assembly
        {
            private readonly static System.Reflection.Assembly[] s_Assemblies;
            private readonly static TypeTable s_CachedTypes;

            static Assembly()
            {
                s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                s_CachedTypes = new TypeTable(s_Assemblies);
            }

            /// <summary>
            /// 获取已加载的程序集。
            /// </summary>
            /// <returns>已加载的程序集。</returns>
            public static System.Reflection.Assembly[] GetAssemblies()
            {
                return s_Assemblies;
            }

            /// <summary>
            /// 获取已加载的程序集中的所有类型。
            /// </summary>
            /// <returns>已加载的程序集中的所有类型。</returns>
            public static System.Type[] GetAllTypes()
            {
                return s_CachedTypes.GetAllTypes();
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static System.Type GetType(string typeName)
            {
                return s_CachedTypes.GetType(typeName);
            }

            public static T CreateInstance<T>()
            {
                return Activator.CreateInstance<T>();
            }

            public static object CreateInstance(System.Type type)
            {
                return Activator.CreateInstance(type);
            }

            public static object CreateInstance(string typeFullName)
            {
                System.Type targetType = Utility.Assembly.GetType(typeFullName);
                if (null != targetType)
                    return Activator.CreateInstance(targetType);

                return null;
            }

            public static T CreateInstance<T>( params object[] args)
            {
                return (T)Activator.CreateInstance(typeof(T),args);
            }

            public static object CreateInstance(System.Type type, params object[] args)
            {
                return Activator.CreateInstance(type, args);
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static string[] GetTypeNamesOf(System.Type baseType, bool sort = false)
            {
                List<string> subTypeNameList = FrameStackList<string>.Acquire();
                System.Type[] allTypes = GetAllTypes();
                for (int i = 0, icnt = allTypes.Length; i < icnt; ++i)
                {
                    System.Type type = allTypes[i];
                    if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                        subTypeNameList.Add(type.FullName);
                }

                if (sort)
                    subTypeNameList.Sort();

                string[] res = subTypeNameList.ToArray();
                FrameStackList<string>.Recycle(subTypeNameList);
                return res;
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static System.Type[] GetTypesOf(System.Type baseType, bool sort = false)
            {
                List<System.Type> subTypeList = FrameStackList<System.Type>.Acquire();
                System.Type[] allTypes = GetAllTypes();
                for (int i = 0, icnt = allTypes.Length; i < icnt; ++i)
                {
                    System.Type type = allTypes[i];
                    if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                        subTypeList.Add(type);
                }

                if (sort)
                    subTypeList.Sort();

                System.Type[] res = subTypeList.ToArray();
                FrameStackList<System.Type>.Recycle(subTypeList);
                return res;
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型。
            /// </summary>
            /// <param name="typeName">要获取的类型名。</param>
            /// <returns>已加载的程序集中的指定类型。</returns>
            public static void GetTypesOf(System.Type baseType, ref List<System.Type> types, bool sort = false)
            {
                System.Type[] allTypes = GetAllTypes();
                for (int i = 0, icnt = allTypes.Length; i < icnt; ++i)
                {
                    System.Type type = allTypes[i];
                    if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                        types.Add(type);
                }

                if (sort)
                    types.Sort();
            }

            public static System.Type GetSubTypeWithTypeName<T>(string subTypeName)
            {
                System.Type baseType = typeof(T);
                List<System.Type> subTypeList = FrameStackList<System.Type>.Acquire();
                GetTypesOf(baseType, ref subTypeList);
                System.Type targetSubType = null;
                for (int i = 0, icnt = subTypeList.Count; i < icnt; ++i)
                {
                    if (subTypeList[i].FullName.Contains(subTypeName.Trim()))
                    {
                        targetSubType = subTypeList[i];
                        break;
                    }
                }

                FrameStackList<System.Type>.Recycle(subTypeList);
                return targetSubType;
            }

            public static void GetSubTypeWithTypeNames<T>(string[] subTypeNames, ref List<System.Type> types)
            {
                if (null != subTypeNames && subTypeNames.Length > 0)
                {
                    System.Type baseType = typeof(T);
                    List<System.Type> subTypeList = FrameStackList<System.Type>.Acquire();
                    GetTypesOf(baseType, ref subTypeList);
                    if (subTypeList.Count > 0)
                    {
                        for (int i = 0, icnt = subTypeNames.Length; i < icnt; ++i)
                        {
                            for (int j = 0, jcnt = subTypeList.Count; j < jcnt; ++j)
                            {
                                if (subTypeList[j].FullName.Contains(subTypeNames[i].Trim()))
                                {
                                    types.Add(subTypeList[j]);
                                    break;
                                }
                            }
                        }
                    }

                    FrameStackList<System.Type>.Recycle(subTypeList);
                }
            }

            public static bool IsSubTypeOf(String typeName, String baseTypeName)
            {
                System.Type baseType = GetType(baseTypeName);
                if (null != baseType)
                    return IsSubTypeOf(typeName, baseType);
                else
                    Debugger.LogWarning("Can not find base type with name '{0}'.", baseTypeName);

                return false;
            }

            public static bool IsSubTypeOf(System.Type type, String baseTypeName)
            {
                System.Type baseType = GetType(baseTypeName);
                if (null != baseType)
                    return IsSubTypeOf(type, baseType);
                else
                    Debugger.LogWarning("Can not find base type with name '{0}'.", baseTypeName);

                return false;
            }

            public static bool IsSubTypeOf(String typeName, System.Type baseType)
            {
                System.Type type = GetType(typeName);
                if (null != type)
                    return IsSubTypeOf(type, baseType);
                else
                    Debugger.LogWarning("Can not find type with name '{0}'.", typeName);

                return false;
            }

            public static bool IsSubTypeOf(System.Type type, System.Type baseType)
            {
                return baseType.IsAssignableFrom(type);
            }
        }
    }
}
