using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace CleanAssetsTool
{
    public static class CleanAssetAssemblyUtility
    {
        private readonly static Assembly[] s_Assemblies;
        private readonly static Dictionary<string, Type> m_TypeTable = new Dictionary<string, Type>();

        static CleanAssetAssemblyUtility()
        {
            s_Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogWarningFormat("Type name can not be null or empty string!");
                return null;
            }

            Type curType = null;
            if (m_TypeTable.TryGetValue(typeName, out curType))
            {
                return curType;
            }

            curType = Type.GetType(typeName);
            if (null != curType)
            {
                m_TypeTable.Add(typeName, curType);
                return curType;
            }

            for (int i = 0, icnt = s_Assemblies.Length; i < icnt; ++i)
            {
                Assembly assembly = s_Assemblies[i];

                curType = Type.GetType(string.Format("{0}, {1}", typeName, assembly.FullName));
                if (curType != null)
                {
                    m_TypeTable.Add(typeName, curType);
                    return curType;
                }
            }

            return null;
        }
    }
}
