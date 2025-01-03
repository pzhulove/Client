


namespace Tenmove.Runtime.Unity
{
    using System;

    public interface ITMUnityGameProfileSight
    {
        bool OnInit(ITMUnityGameProfiler profiler);
        void OnDeinit();

        void OnGUI();
        void OnUpdate();
    }
}