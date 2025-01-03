using System.Runtime.InteropServices;
using System;

namespace iOSUtility
{
    public class MemoryReport
    {
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern UInt64 ios_report_memory(); 
#else 
        private static UInt64 ios_report_memory() 
        {
            UnityEngine.Debug.LogError("当前平台不支持获取内存");
            return 0;
        }
#endif

        private static int mLastMemoryInMB = 0;
        private static int mMaxMemoryInMB  = 0;

        public static int GetMaxMemoryUsedInMB()
        {
            return mMaxMemoryInMB;
        }

        public static int GetMemoryUsedInMB()
        {
            UInt64 memoryUseInByte = ios_report_memory();

            mLastMemoryInMB = (int)memoryUseInByte / 1024 / 1024;

            if (mLastMemoryInMB > mMaxMemoryInMB)
            {
                mMaxMemoryInMB = mLastMemoryInMB;
            }

            return mLastMemoryInMB;
        }
    }
}
