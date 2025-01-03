using UnityEngine;
using System.Collections;

public struct THandle
{
    public int m_eType;
    public int m_iID;
    public uint m_iIndex;

    public THandle(int eType = -1, int iHandleID = 0, uint iIndex = 0)
    {
        m_eType = eType;
        m_iID = iHandleID;
        m_iIndex = iIndex;
    }

    public bool IsValid()
    {
        return true;
    }

    public int Type()
    {
        return m_eType;
    }

    public int Id()
    {
        return m_iID;
    }

    public uint Index()
    {
        return m_iIndex;
    }
}
