

using System;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    internal partial class UnityGameProfileClient : BaseModule ,ITMUnityGameProfileClient
    {
        private readonly ITMNetManager m_NetManager;
        private readonly AssetLoader m_AssetLoader;

        private ITMNetClient m_NetClient;
        private string m_ClientIP;

        public UnityGameProfileClient()
        {
            m_NetManager = ModuleManager.GetModule<ITMNetManager>();
            m_AssetLoader = new AssetLoader();

            m_NetManager.RegisterMessage<GameProfileTransmitFile>();
            m_NetManager.RegisterMessage<GameProfileTransmitFileEnd>();
            m_NetManager.RegisterMessage<GameProfileCreateAsset>();
            m_NetManager.RegisterMessage<GameProfileClearAllAssets>();
        }

        public string IP
        {
            get { return m_ClientIP; }
        }

        public uint InvalidAssetHandle
        {
            get { return AssetLoader.InvalidAssetReuqestHandle; }
        }

        public void BeginConnect(NetIPAddress serverIP, int port)
        {
            if(null != m_NetClient)
            {
                Debugger.LogWarning("Last connect is still alive, Please invoke 'EndConnect()' before you create a new connection!");
                return;
            }

            m_NetClient = m_NetManager.CreateClient(serverIP, 9527, 32 * 1024);
            m_NetClient.Start();
            m_ClientIP = m_NetClient.IPEndPoint.Split(':')[0];
            m_NetClient.RegisterMessageProcessor<GameProfileTransmitFile>(_OnNetMessageTransmitFile);
        }

        public void EndConnect()
        {
            if(null != m_NetClient)
            {
                m_NetClient.UnregisterMessageProcessor<GameProfileTransmitFile>();
                m_NetClient.Shutdown();
                m_NetClient = null;
            }
        }

        public void RegisterMessageProcessor<T, U>(MessageProcessor<T, U> processor, U userData)
            where T : NetMessage
            where U : IEquatable<U>
        {
            if (null != m_NetClient)
                m_NetClient.RegisterMessageProcessor(processor, userData);
            else
                Debugger.LogWarning("Please connect before you register processors!");
        }

        public uint LoadAsset<TAsset,TUserData>(string assetPath, TUserData userData, AssetLoadCallback<TAsset, TUserData> callback) where TAsset: class
        {
            return m_AssetLoader.LoadAsset(assetPath, userData, callback);
        }

        internal override int Priority
        {
            get { return 0; }
        }

        internal override void Shutdown()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_NetClient)
                m_NetClient.Update();

            m_AssetLoader.Update();
        }

        private void _OnNetMessageTransmitFile(GameProfileTransmitFile netMessage)
        {
            if (null != netMessage)
            {
                string assetPackagePath = netMessage.GetNativeFilePath(netMessage.FileName);
                if (Utility.File.Exists(assetPackagePath))
                    m_AssetLoader.LoadAssetPackage(assetPackagePath, this, _OnAssetPackageLoaded);
                else
                    Debugger.LogWarning("Asset package with path '{0}' is not exist!", assetPackagePath);
            }
        }

        private void _OnAssetPackageLoaded(string assetPackagePath,UnityGameProfileClient simulator)
        {
            /// 通知服务端收到文件
            if (null != m_NetClient)
            {
                GameProfileTransmitFileEnd message = m_NetClient.CreateMessage<GameProfileTransmitFileEnd>();
                message.Fill(string.Format("设备:{0}\n同步资源包[{1}]成功！", IP, Utility.Path.GetFileName(assetPackagePath)));
                m_NetClient.SendMessage(message);
            }
        }
    }
}