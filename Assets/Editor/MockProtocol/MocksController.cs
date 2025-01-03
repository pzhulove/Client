using System.Collections.Generic;
using UnityEngine;
using System;
using Network;

namespace Mock
{
    class MocksController
    {
        public const string kPrePath = "Assets/Editor/MockProtocol/MockData";
        public const string kMockProcessPath = "Assets/Editor/MockProtocol/MockProcessData";

        private MocksManager mMananger = null;
        private byte[] mBuff = new byte[0];
        private byte[] mRecivedBuff = new byte[0];
        private List<ScriptableObject> mAllCreatedScriptObjects = new List<ScriptableObject>();

        public ServerType mServerType = ServerType.GATE_SERVER;

        public MocksController(MocksManager manager)
        {
            mMananger = manager;
            mBuff = new byte[1024 * 1024 * 16];
            mRecivedBuff = new byte[1024 * 1024 * 16];
        }

        public void RecivedMessage(Protocol.IMockProtocol mockProtocol)
        {
            if (null == mockProtocol)
            {
                UnityEngine.Debug.LogError("[MockProtocol] mockObj is nil");
                return;
            }

            global::Protocol.IProtocolStream stream = mockProtocol as global::Protocol.IProtocolStream;
            if (null == stream)
            {
                return;
            }

            global::Protocol.IGetMsgID msgId = mockProtocol as global::Protocol.IGetMsgID;
            if (null == msgId)
            {
                return;
            }

            int pos = 0;
            stream.encode(mRecivedBuff, ref pos);

            MsgDATA data = new MsgDATA(pos);
            data.id = msgId.GetMsgID();
            data.serverType = mServerType;

            Array.Copy(mRecivedBuff, data.bytes, data.bytes.Length);

            if (Application.isPlaying)
            {
                NetProcess.Instance().PushQueue(data);
            }
        }

        public void SendMessage(Protocol.IMockProtocol mockProtocol)
        {
            if (null == mockProtocol)
            {
                UnityEngine.Debug.LogError("[MockProtocol] mockObj is nil");
                return;
            }

            global::Protocol.IProtocolStream stream = mockProtocol as global::Protocol.IProtocolStream;
            if (null == stream)
            {
                return;
            }

            global::Protocol.IGetMsgID msgId = mockProtocol as global::Protocol.IGetMsgID;
            if (null == msgId)
            {
                return;
            }

            Type mockType = mockProtocol.GetType();
            MockUnit mockUnit = mMananger.GetMockUnit(mockType);
            if (null == mockUnit)
            {
                return;
            }

            if (Application.isPlaying)
            {
                NetManager.instance.SendCommandObject(mServerType, mockProtocol);
                UnityEngine.Debug.LogFormat("[MockProtocol] mock send {0}", mockUnit.mockMsgClsName);
            }
        }

        public ScriptableObject CreateMockProtocol(MockUnit unit)
        {
            if (null == unit)
            {
                return null;
            }
            ScriptableObject mockScriptObject = FileUtility.CreateScriptObjectRecursion("Mock.Protocol.", kPrePath, unit.mockMsgType.Name, unit.mockMsgType);

            FileUtility.SaveAssetAndPingObject(mockScriptObject);

            return mockScriptObject;
        }

        public ScriptableObject CreateMockProcess()
        {
            ScriptableObject processObject = FileUtility.CreateScriptObjectRecursion("Mock.", kMockProcessPath, "process", typeof(MockProcessData));
            FileUtility.SaveAssetAndPingObject(processObject);
            return processObject;
        }
   }
}
