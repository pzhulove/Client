using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class iOSConfigureLibGC 
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void bfutils_SetLibGCFreeSpaceDivisor(int divisor);
#endif

    public static void ConfigureLibGC(int divisor = 16)
    {
#if UNITY_IOS && !UNITY_EDITOR
        // The default divisor is 3. Anything higher saves memory, but causes more frequent collections.
        bfutils_SetLibGCFreeSpaceDivisor(divisor);
#endif
    }
}
