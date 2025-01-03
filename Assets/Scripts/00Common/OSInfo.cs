using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AndroidAPILevel
{
    None,
    Level_16 = 16,
    Level_17 = 17,
    Level_18 = 18,
    Level_19 = 19,
    Level_21 = 21,
    Level_22 = 22,
    Level_23 = 23,
    Level_24 = 24,
    Level_25 = 25,
    Level_26 = 26,
    Level_27 = 27,
    Level_28 = 28,
    Level_29 = 29,
    Level_30 = 30,
    Level_31 = 31,
    Level_32 = 32,
}

public class OSInfo
{
    static public AndroidAPILevel GetAndroidOSAPILevel()
    {
        string op = SystemInfo.operatingSystem;
        Debug.Log("### Operating System:" + op);

        if (op.Contains("Android OS",System.StringComparison.OrdinalIgnoreCase))
        {
            if(op.Contains("API-16", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_16;
            else if (op.Contains("API-17", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_17;
            else if (op.Contains("API-18", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_18;
            else if (op.Contains("API-19", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_19;
            else if (op.Contains("API-21", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_21;
            else if (op.Contains("API-22", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_22;
            else if (op.Contains("API-23", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_23;
            else if (op.Contains("API-24", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_24;
            else if (op.Contains("API-25", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_25;
            else if (op.Contains("API-26", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_26;
            else if (op.Contains("API-27", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_27;
            else if (op.Contains("API-28", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_28;
            else if (op.Contains("API-29", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_29;
            else if (op.Contains("API-30", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_30;
            else if (op.Contains("API-31", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_31;
            else if (op.Contains("API-32", System.StringComparison.OrdinalIgnoreCase))
                return AndroidAPILevel.Level_32;
            else
                return AndroidAPILevel.None;
        }

        return AndroidAPILevel.None;
    }

    static public int GetSysMemorySize()
    {
        int sms = SystemInfo.systemMemorySize;
        Debug.Log("### System Memory Size:" + sms);
        return sms;
    }
}
