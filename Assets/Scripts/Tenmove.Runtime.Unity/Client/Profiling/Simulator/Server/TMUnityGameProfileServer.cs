

using System;
using System.Collections.Generic;
using Tenmove.Runtime;
using Tenmove.Runtime.Unity;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    internal partial class UnityGameProfileServer : BaseModule, ITMUnityGameProfileServer
    {
        private static readonly string PackageDataPath = "Scripts/Tenmove.Editor.Unity/Editor/Engine/Profiling/Simulator/res/data/package.dat";

        private readonly ITMNetManager m_NetManager;
        private ITMNetServer m_Server;
        private string m_ServerIP;
        private ITMGameProfileAssetPacker m_AssetPacker;
        private AssetPackageDesc m_AssetPackageDesc;

        public UnityGameProfileServer()
        {
            /// 注册网络消息
            m_NetManager = ModuleManager.GetModule<ITMNetManager>();

            m_NetManager.RegisterMessage<GameProfileTransmitFile>();
            m_NetManager.RegisterMessage<GameProfileTransmitFileEnd>();
            m_NetManager.RegisterMessage<GameProfileCreateAsset>();
            m_NetManager.RegisterMessage<GameProfileClearAllAssets>();

            m_AssetPacker = null;
            m_AssetPackageDesc = null;
        }

        internal override int Priority
        {
            get { return 0; }
        }

        public string ServerIP
        {
            get { return m_ServerIP; }
        }

        public bool HasConnection
        {
            get { return null != m_Server && m_Server.HasConnection; }
        }

        public bool IsServerOnline
        {
            get { return null != m_Server; }
        }

        public ITMGameProfileAssetPackageDesc CurrentPackageDesc
        {
            get { return m_AssetPackageDesc; }
        }
        
        internal override void Shutdown()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_Server)
                m_Server.Update();
        }

        public void SetAssetPacker(ITMGameProfileAssetPacker assetPacker)
        {
            if (null != assetPacker)
                m_AssetPacker = assetPacker;
        }

        public void StartServer()
        {
            if (null == m_Server)
            {
                m_Server = m_NetManager.CreateServer(9527, 32 * 1024);
                m_Server.Start();
                m_ServerIP = m_Server.IPEndPoint.Split(':')[0];
            }
        }

        public void ShutdownServer()
        {
            if (null != m_Server)
            {
                m_Server.UnregisterMessageProcessor<GameProfileTransmitFileEnd>();

                m_Server.Shutdown();
                m_Server = null;
            }
        }

        public NetMessage CreateMessage(Type netMessageType)
        {
            if (null != m_Server)
                return m_Server.CreateMessage(netMessageType);
            else
                Debugger.LogWarning("Must start server before you create a net message!");

            return null;
        }

        public T CreateMessage<T>() where T : NetMessage
        {
            return CreateMessage(typeof(T)) as T;
        }

        public void SendMessage<T>(T message, string ip = "") where T : NetMessage
        {
            if (null != m_Server)
                m_Server.SendMessage(message,ip);
            else
                Debugger.LogWarning("Must start server before you send a net message!");
        }

        public void RegisterMessageProcessor<T, U>(MessageProcessor<T, U> processor, U userData)
            where T : NetMessage where U : IEquatable<U>
        {
            if (null != m_Server)
                m_Server.RegisterMessageProcessor(processor, userData);
            else
                Debugger.LogWarning("Must start server before you register processors!");
        }
         
        public void GetConnections(List<ITMNetConnection> connectionsList)
        {
            if (null != m_Server)
                m_Server.GetConnections(connectionsList);
        }

        public void PackAssets(List<string> assetList)
        {
            if (null != m_AssetPacker)
            {
                if (null == m_AssetPackageDesc)
                    m_AssetPackageDesc = new AssetPackageDesc(string.Empty, string.Empty, 0, null);

                if (m_AssetPacker.PackAsset(
                Utility.Path.Combine(Application.streamingAssetsPath, "AssetBundles/simulator"), GetType().Name.Replace("Unity", null).ToLower(), assetList, m_AssetPackageDesc))
                    _SavePackageDesc(m_AssetPackageDesc);
            }
        }

        public void RefreshPackageDesc()
        {
            m_AssetPackageDesc = _LoadPackageDesc();
        }

        private void _SavePackageDesc(AssetPackageDesc desc)
        {
            using (System.IO.Stream data = Runtime.Utility.File.OpenWrite(
                Runtime.Utility.Path.Combine(Application.dataPath, PackageDataPath), true))
            {
                Serializer<Serializer, AssetPackageDesc>.Serialize(desc, data);
            }
        }

        private AssetPackageDesc _LoadPackageDesc()
        {
            AssetPackageDesc desc = null;
            string packageDataPath = Runtime.Utility.Path.Combine(Application.dataPath, PackageDataPath);
            if (Runtime.Utility.File.Exists(packageDataPath))
            {
                using (System.IO.Stream data = Runtime.Utility.File.OpenRead(
                    packageDataPath))
                {
                    Deserializer<Serializer, AssetPackageDesc>.Deserialize(data, ref desc);
                }
            }
            return desc;
        }
    }
}