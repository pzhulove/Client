
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
            internal class TypeTable
            {
                private readonly System.Reflection.Assembly[] m_Assemblies = null;
                private readonly Dictionary<string, System.Type> m_TypeTable = null;

                public TypeTable(System.Reflection.Assembly[] assemblies)
                {
                    m_Assemblies = assemblies ?? new System.Reflection.Assembly[0];
                    m_TypeTable = new Dictionary<string, System.Type>();
                }

                public System.Type GetType(string typeName)
                {
                    if (string.IsNullOrEmpty(typeName))
                    {
                        Debugger.LogWarning("Type name can not be null or empty string!");
                        return null;
                    }

                    System.Type curType = null;
                    if (m_TypeTable.TryGetValue(typeName, out curType))
                    {
                        return curType;
                    }

                    curType = System.Type.GetType(typeName);
                    if(null != curType)
                    {
                        m_TypeTable.Add(typeName, curType);
                        return curType;
                    }

                    for(int i = 0,icnt = m_Assemblies.Length;i<icnt;++i)
                    {
                        System.Reflection.Assembly assembly = m_Assemblies[i];

                        curType = System.Type.GetType(string.Format("{0}, {1}", typeName, assembly.FullName));
                        if (curType != null)
                        {
                            m_TypeTable.Add(typeName, curType);
                            return curType;
                        }
                    }

                    return null;
                }

                public System.Type[] GetAllTypes()
                {
                    List<System.Type> allTypes = new List<System.Type>();
                    for (int i = 0; i < m_Assemblies.Length; i++)
                    {
                        allTypes.AddRange(m_Assemblies[i].GetTypes());
                    }

                    return allTypes.ToArray();
                }

                public void ClearAll()
                {
                    m_TypeTable.Clear();
                }
            }
        }
    }
}
