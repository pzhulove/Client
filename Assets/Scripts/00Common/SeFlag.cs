using UnityEngine;
using System.Collections;

public class SeFlag {

    protected int m_iFlag;
    protected string flagData = null;

    public SeFlag(int iFlag = 0)
    {
        m_iFlag = iFlag;
    }
    public bool HasFlag(int iFlag)
    {
        int tmp = (m_iFlag & iFlag);
        return tmp != 0;
    }

    public void SetFlag(int iFlag, string data = null)
    {
        m_iFlag |= iFlag;
        flagData = data;
    }

    public void ClearFlag(int iFlag)
    {
        m_iFlag &= ~iFlag;
        flagData = null;
    }

    public void Clear()
    {
        m_iFlag = 0;
        flagData = null;
    }

    public int GetAllFlag()
    {
        return m_iFlag;
    }

    public string GetFlagData()
    {
        return flagData;
    }
}
