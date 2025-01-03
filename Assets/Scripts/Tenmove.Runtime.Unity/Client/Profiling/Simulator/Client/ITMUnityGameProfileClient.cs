

namespace Tenmove.Runtime.Unity
{
    using System;
    using UnityEngine;

    public interface ITMUnityGameProfileClient : ITMUnityGameProfiler
    {
        string IP { get; }

        void BeginConnect(NetIPAddress serverIP, int port);
        void EndConnect();
    }
}