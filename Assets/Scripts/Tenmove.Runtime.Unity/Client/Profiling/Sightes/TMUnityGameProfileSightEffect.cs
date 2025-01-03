
namespace Tenmove.Runtime.Unity
{
    public partial class UnityGameProfileSightEffect : ITMUnityGameProfileSight
    {
        private ITMUnityGameProfiler m_GameProfiler;

        public UnityGameProfileSightEffect()
        {
            m_GameProfiler = null;
        }

        public bool OnInit(ITMUnityGameProfiler profiler)
        {
            if(null != profiler)
            {
                m_GameProfiler = profiler;
                return true;
            }

            return false;
        }

        public void OnDeinit()
        {
        }

        public void OnGUI()
        {
        }

        public void OnUpdate()
        {
        }


    }
}