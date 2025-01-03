using System.Collections;
using System.Collections.Generic;
using System;

public class TypeTable : Singleton<TypeTable>
{
    Dictionary<string, System.Type> m_TypeTable = new Dictionary<string, System.Type>();

    static public System.Type GetType(string typeName)
    {
        return instance._GetType(typeName);
    }

    public System.Type _GetType(string typeName)
    {
        if(string.IsNullOrEmpty(typeName))
        {
            Logger.LogWarning("typeName can not be null or empty string!");
            return null;
        }

        Type curType = null;
        if(m_TypeTable.TryGetValue(typeName,out curType))
        {
            return curType;
        }

        curType = Type.GetType(typeName);
        m_TypeTable.Add(typeName, curType);
        return curType;
    }

    public void ClearAll()
    {
        m_TypeTable.Clear();
    }
}
