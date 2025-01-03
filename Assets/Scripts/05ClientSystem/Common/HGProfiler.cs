using UnityEngine;
using System.Collections.Generic;

public class HGProfiler
{
    static float time;
    static string current;

    struct profilerData
    {
        public float time;
        public string name;
    }

    static profilerData[]  sdata = new profilerData[100];
    static int             stop = -1;


    static public void BeginProfiler(string name)
    {
		//Debug.LogErrorFormat("BeginProfiler [{0}]",name);
		ExceptionManager.instance.RecordLog(string.Format("BeginProfiler [{0}]",name));
        stop += 1;
        if(stop < 0)
        {
            stop = 0;
        }

        if(stop >= 100)
        {
            stop = 99;
        }

        var data = sdata[stop];
        data.time = Time.realtimeSinceStartup;
        data.name = name;
        sdata[stop] = data;
    }

    static public void EndProfiler()
    {
		if (stop <= -1)
			return;
		
        var data = sdata[stop];
        data.time = Time.realtimeSinceStartup - data.time;
		//Debug.LogErrorFormat("[{0}] use time: [{1}]s",data.name,data.time);
		ExceptionManager.instance.RecordLog(string.Format("[{0}] use time: [{1}]s",data.name,data.time));
        stop--;
    }
}