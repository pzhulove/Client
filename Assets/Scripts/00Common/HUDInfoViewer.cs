using UnityEngine;
using System.Collections;

public class HUDInfoViewer : MonoSingleton<HUDInfoViewer>
{
    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        UpdateUsed();
        UpdateFPS();
    }

    //Memory
    private string sUserMemory;
    private string s;
    public bool OnMemoryGUI = true;
    private float MonoUsedM;
    private float AllMemory;
    [Range(0, 100)]
    public int MaxMonoUsedM = 50;
    [Range(0, 400)]
    public int MaxAllMemory = 200;

    int cnt = 0;
    void UpdateUsed()
    {
        ++cnt;
        if (cnt < 5)
            return;

        cnt = 0;
        sUserMemory = "";
		MonoUsedM = UnityEngine.Profiling.Profiler.GetMonoUsedSize() / (1024.0f * 1024.0f);
		AllMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory() / (1024.0f * 1024.0f);

        sUserMemory += "Mono Used:" + (MonoUsedM.ToString("f2") )+ "MB" + "\n";
		sUserMemory += "Used Heap:" + (UnityEngine.Profiling.Profiler.usedHeapSize / (1024.0f * 1024.0f)) .ToString("f2")+ "MB" + "\n";
		sUserMemory += "Mono Heap:" + (UnityEngine.Profiling.Profiler.GetMonoHeapSize() / (1024.0f * 1024.0f) ).ToString("f2") + "MB" + "\n";
		sUserMemory += "UnUsedReserved:" + (UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemory() / (1024.0f * 1024.0f)).ToString("f2")  + "MB" + "\n";
        sUserMemory += "AllMemory:" + AllMemory.ToString("f2") + "MB" + "\n";
        sUserMemory += "Pooled Game Objects:" + CGameObjectPool.instance.GetPooledGameObjectNum() + "\n";

        //s = "";
        //s += " MonoHeap:" + Profiler.GetMonoHeapSize() / 1000 + "k";
        //s += " MonoUsed:" + Profiler.GetMonoUsedSize() / 1000 + "k";
        //s += " Allocated:" + Profiler.GetTotalAllocatedMemory() / 1000 + "k";
        //s += " Reserved:" + Profiler.GetTotalReservedMemory() / 1000 + "k";
        //s += " UnusedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000 + "k";
        //s += " UsedHeap:" + Profiler.usedHeapSize / 1000 + "k";
    }


    //FPS
    float updateInterval = 0.5f;
    private float accum = 0.0f;
    private float frames = 0;
    private float timeleft;
    private float fps;
    private string FPSAAA;
    [Range(0, 150)]
    public int MaxFPS;
    void UpdateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = accum / frames;
            FPSAAA = "FPS: " + fps.ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        if (OnMemoryGUI)
        {
            GUI.color = new Color(0,1,  0);
            GUI.Label(new Rect(10, Screen.height - 140, 200, 100), sUserMemory);
            GUI.Label(new Rect(10, Screen.height - 40, 100, 30), FPSAAA);
            //if (MonoUsedM > MaxMonoUsedM)
            //{
            //    GUI.backgroundColor = new Color(1, 0, 0);
            //    GUI.Button(new Rect(0, 0, 100, 100), "MonoUsedM Warning!!内存不足");
            //}
            //if (AllMemory > MaxAllMemory)
            //{
            //    GUI.backgroundColor = new Color(1, 0, 1);
            //    GUI.Button(new Rect(0, 100, 100, 100), "AllMemory Warning!!内存堪忧");
            //}
            //if (fps > MaxFPS)
            //{
            //    GUI.backgroundColor = new Color(1, 0.4f, 0.5f);
            //    GUI.Button(new Rect(0, 0, 1024, 1024), "FPS Warning!!");
            //}
        }
    }
}
