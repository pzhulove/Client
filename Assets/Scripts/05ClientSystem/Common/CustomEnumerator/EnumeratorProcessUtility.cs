using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class EnumeratorProcessManagerUtility
{
    private static Type type = typeof(WaitForSeconds);
    private static FieldInfo info = type.GetField("m_Seconds", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

    public static int GetWaitForMS(WaitForSeconds t)
    {
        float v = 0.0f;

        try
        {
            v = (float)info.GetValue(t);
        }
        catch
        {
            v = 0.0f;
        }

        return (int)(v * 1000);
    }
}

