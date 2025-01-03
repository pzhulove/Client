using System;
using System.Diagnostics;
using System.Reflection;

public static class UWAProfilerUtility
{
    private const string skUwaProfilerProfiler = "UWALocal.UwaProfiler";
    private static Assembly skUwaCoreAssembly = null;
    private static object skDumpHelper = null;
    private static object MonoDumpHelper 
    {
        get
        { 
            if (null == skDumpHelper)
            {
                var monoTraceType = GetUwaCoreType("UWALocal.MonoTrackManager");
                var dhField = monoTraceType.GetField("DumpHelper");
                skDumpHelper = dhField.GetValue(null);
            }

            return skDumpHelper;
        }
    }

    private static FieldInfo skDumpHelperIsAuto = null;
    private static object skTrue = true;
    private static object skFalse = false;

    public static bool MonoDumpHelperIsAuto
    {
        set 
        {
            if (null == skDumpHelperIsAuto)
            {
                var type = MonoDumpHelper.GetType();
                skDumpHelperIsAuto = type.GetField("AutoDump");
            }

            skDumpHelperIsAuto.SetValue(MonoDumpHelper, value ? skTrue : skFalse);
        }
    }

    private static FieldInfo skDumpHelperIsWait2Dump = null;
    private static bool MonoDumpHelperIsWait2Dump
    {
        set 
        {
            if (null == skDumpHelperIsWait2Dump)
            {
                var type = MonoDumpHelper.GetType();
                skDumpHelperIsWait2Dump = type.GetField("WaitToDump");
            }

            skDumpHelperIsWait2Dump.SetValue(MonoDumpHelper, value ? skTrue : skFalse);
        }
    }

    private static Assembly UwaCoreAssembly
    {
        get
        {
            if (null == skUwaCoreAssembly)
            {
                skUwaCoreAssembly = _FindAssembly("UWALocalCore", skUwaProfilerProfiler);
            }
            return skUwaCoreAssembly;
        }
    }

    private static Assembly _FindAssembly(string assetname, string typename)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Assembly[] array = assemblies;

        foreach (Assembly assembly in array)
        {
            if ((object)assembly.GetType(typename) != null && assembly.FullName.ToLower().Contains(assetname.ToLower()))
            {
                return assembly;
            }
        }

        return null;
    }
 
    private static Type GetUwaCoreType(string typename)
    {
        return _GetAssType(UwaCoreAssembly, typename);
    }

    private static Type _GetAssType(Assembly ass, string typename)
    {
        if (null == ass)
        {
            return null;
        }

        return ass.GetType(typename);
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// 请注意 DoDump调用的最小间隔为1秒
    /// </summary>
    [Conditional("ENABLE_PROFILER")]
    public static void DoDump()
    {
        //MonoDumpHelperIsWait2Dump = true;
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.AddMarker(tag)
    /// </summary>
    [Conditional("ENABLE_PROFILER")]
    public static void Mark(string tag)
    {
        // 在编包的时候会替换成 如下实现
        // UWAEngine.AddMarker(tag);
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.LogValue(tag, v);
    /// </summary>
    [Conditional("ENABLE_PROFILER")]
    public static void LogValue(string tag, int v)
    {
        // 在编包的时候会替换成 如下实现
        // UWAEngine.LogValue(tag, v);
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.LogValue(tag, v);
    /// </summary>
    [Conditional("ENABLE_PROFILER")]
    public static void LogValue(string tag, bool v)
    {
        // 在编包的时候会替换成 如下实现
        //UWAEngine.LogValue(tag, v);
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.LogValue(tag, v);
    /// </summary>
    [Conditional("ENABLE_PROFILER")]
    public static void LogValue(string tag, float v)
    {
        // 在编包的时候会替换成 如下实现
        //UWAEngine.LogValue(tag, v);
    }
}

public struct UWAProfilerNode : IDisposable
{
    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.PushSample(tag);
    /// </summary>
    public UWAProfilerNode(string tag)
    {
#if ENABLE_PROFILER && !LOGIC_SERVER
        UnityEngine.Profiling.Profiler.BeginSample(tag);
#endif

        // 在编包的时候会替换成 如下实现
        // UWAEngine.PushSample(tag);
    }

    /// <summary>
    /// 在编包的时候会替换成 如下实现
    /// UWAEngine.PopSample(tag);
    /// </summary>
    public void Dispose()
    {
#if ENABLE_PROFILER && !LOGIC_SERVER
        UnityEngine.Profiling.Profiler.EndSample();
#endif

        // 在编包的时候会替换成 如下实现
        // UWAEngine.PopSample();
    }
}
