using UnityEngine;
using System.Collections.Generic;

public class ActiveBindRecords : MonoBehaviour
{
    public List<ActiveVarBinder> m_VarBinders = new List<ActiveVarBinder>();
    void OnDestroy()
    {
        m_VarBinders = null;
    }
}
