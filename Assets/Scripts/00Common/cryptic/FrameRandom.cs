using System;

using System.Diagnostics;

public class FrameRandomImp
{
    private const uint addValue = 0x3039;
    public uint callNum = 0;
    public uint  callFrame = 0;
    private const uint maxShort = 0x10000;
    private const uint multiper = 0x472e396d;

    public RecordServer mRecordServer;


#if !LOGIC_SERVER
    private uint nSeed = ((uint)UnityEngine.Random.Range(0x7fff, 0x7fffffff));
#else
	private  uint nSeed = 0x7fffffff;//((uint)UnityEngine.Random.Range(0x7fff, 0x7fffffff));
#endif

    static void RecordStackTrace()
    {
        /*if (RecordServer.GetInstance().IsRecord())
		{
			StackTrace st = new StackTrace(true);
			if (st != null)
			{

				var frames = st.GetFrames();

				if (frames != null && frames.Length > 0)
				{
					int length = frames.Length;

					string log = "";

					for (int i = 0; i < length-1; i++)
					{
						StackFrame sf = st.GetFrame(i);
						if (sf != null)
						{
							var mtmd = sf.GetMethod();

							if (null != mtmd)
							{
								log += string.Format("[R]{0}", mtmd.Name);
							}
							else
							{
								log += string.Format("[R][!!NULL METHMOD!!]");
							}
						}
					}
					RecordServer.GetInstance().RecordProcess(log);
				}



			}
		}*/
    }

//     public float fRandom()
//     {
//         //Logger.LogForAI("frame:{0} FrameRandom callNum:{1}", FrameSync.instance.curFrame, callNum);
// 
//         return (((float)Random(0x10000)) / 65536f);
//     }
// 
// 
// 
//     public float InRange(float low, float high)
//     {
//         RecordStackTrace();
//         return fRandom() * (high - low) + low;
//     }
// 
 
    static bool GetPathLastName(string fullPath,char split,ref string outstring)
	{
        /* 
		var tmp = fullPath.Split('/');
		string folderName = tmp[tmp.Length - 1];
		return folderName;
        */
        int index  = fullPath.LastIndexOf(split);
        
        if(index >= 0)
        {
            index += 1;
            int len = fullPath.Length - index;

            if(len > 0)
            {
                outstring =  fullPath.Substring(index,len);
                return true;
            }
        }

        return false;
	}

    static string GetFileNameWithNoPath(string fullpath)
    {
        if(string.IsNullOrEmpty(fullpath))
        {
            return string.Empty;
        }

        string path = fullpath;

        if(GetPathLastName(fullpath,'/',ref path))
        {
            return path;
        }

         if(GetPathLastName(fullpath,'\\',ref path))
        {
            return path;
        }

        return path;
    }

    public static void PrintStackTrace(RecordServer r,int stCount)
    {
            StackTrace st = new StackTrace(true);
            string log = "PrintStackTrace!! No Trace Cap!!";
            int frameCount = Math.Min(stCount,st.FrameCount);

            for (int i = 0; i < frameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                string filename = GetFileNameWithNoPath(sf.GetFileName());

                var m = sf.GetMethod();
                log = string.Format("{0}:{1} {2} {3}", m.DeclaringType.Name,m.Name, filename,sf.GetFileLineNumber());
                r.RecordProcess(log);
            }
            //Logger.LogErrorFormat(log);
            //Logger.LogErrorFormat(log);
            //mRecordServer.RecordProcess(log);
    }

	public void RandomCallNum(uint count)
	{
		callNum += count;
	}

    public ushort Random(uint nMax)
    {
        if (nMax == 0)
            return 0;

        callNum++;
        //Logger.LogForAI("frame:{0} FrameRandom callNum:{1}", FrameSync.instance.curFrame, callNum);
        nSeed = (nSeed * 0x472e396d) + 0x3039;
        var ret = (ushort)((nSeed >> 0x10) % nMax);
#if UNITY_EDITOR || LOGIC_SERVER
        if (RecordServerConfigManager.instance.isPrintStack())
        {
            if (mRecordServer != null && mRecordServer.IsProcessRecord())
            {
                StackTrace st = new StackTrace(true);
                string log = "[R]Random Happen!! No Trace Cap!!";

                for (int i = 0; i < st.FrameCount; i++)
                {
                    StackFrame sf = st.GetFrame(i);
                    string filename = GetFileNameWithNoPath(sf.GetFileName());

                    if(filename.Equals("FrameRandom.cs",StringComparison.OrdinalIgnoreCase) == false
                            && filename.Equals("BTAgent.cs",StringComparison.OrdinalIgnoreCase) == false)
                    {
                        var m = sf.GetMethod();
                        log = string.Format("[!!R:{4}] {0}:{1} {2} {3}", m.DeclaringType.Name,m.Name, filename, sf.GetFileLineNumber(),callNum);
                        break;
                    }
                }
                //Logger.LogErrorFormat(log);
                //Logger.LogErrorFormat(log);
                mRecordServer.RecordProcess(log);
            }
        }
#endif

        return ret;
    }

    public ushort Range100()
    {
        RecordStackTrace();
        return (ushort)(Random((uint)GlobalLogic.VALUE_100) + 1);
    }

    public ushort Range1000()
    {
        RecordStackTrace();
        return (ushort)(Random((uint)GlobalLogic.VALUE_1000) + 1);
    }

    public int InRange(int low, int high)
    {
        RecordStackTrace();
        return (int)(Random((uint)(high - low)) + low);
    }

    public int InRange(long low, long high)
    {
        RecordStackTrace();
        return (int)(Random((uint)(high - low)) + low);
    }

    public uint GetSeed()
    {
        return (uint)nSeed;
    }

    public void ResetSeed(uint seed)
    {
        nSeed = seed;
        callNum = 0;
        Logger.LogForAI("FrameRandom resetseed:{0}", seed);
    }
}

