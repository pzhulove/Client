using System;

using System.Diagnostics;

public class FrameRandom2
{
    private const uint addValue = 0x3039dfd;
    public static uint callNum = 0;
    private const uint maxShort = 0x1000230;

#if LOGIC_SERVER
	private static uint multiper = 	0x7fffffff;//((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
    private static uint nSeed = 	0x7fffffff;//((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
	private static uint maxv1 = 	0x7fffffff;//	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
	private static uint maxv2 = 	0x7fffffff;//	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
#else
    private static uint multiper = 	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
    private static uint nSeed = 	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
	private static uint maxv1 = 	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
	private static uint maxv2 = 	((uint)UnityEngine.Random.Range(0x7fff+1, 0x7fffffff-1));
#endif


    public static float fRandom()
    {

        return (((float)Random(0x10000)) / 65536f);
    }



    public static float InRange(float low,float high)
    {
        return fRandom() * (high - low) + low;
    }

    public static ushort Random(uint nMax)
    {
        callNum++;
	

        nSeed = (nSeed * 0x472e396d) + 0x3039;
        return (ushort)((nSeed >> 0x10) % nMax);
    }

	public static ushort Range100()
	{
		return (ushort)(Random(100) + 1);
	}

	public static ushort Range1000()
	{
		return (ushort)(Random(1000) + 1);
	}

    public static int InRange(int low, int high)
    {
		return (int)(Random((uint)(high - low)) + low);
    }

    public static int GetSeed()
    {
        return (int)nSeed;
    }

	public static void ChangeSeed()
	{
		nSeed = (nSeed * 0x472e396d) + 0x3039;
	}

    public static void ResetSeed(uint seed)
    {
        nSeed = seed;
        callNum = 0;

        Logger.LogForAI("FrameRandom resetseed:{0}", seed);
    }
}

