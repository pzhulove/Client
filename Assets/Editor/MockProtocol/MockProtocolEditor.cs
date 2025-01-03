using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Network;
using AdvancedInspector;

namespace Mock
{
    static class FileUtility
    {
        public static string FindFileWithName(string findName, string findPath)
        {
            try
            {
                string[] guids = AssetDatabase.FindAssets(string.Format("t:script {0}", findName), new string[] { findPath });

                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (System.IO.Path.GetFileNameWithoutExtension(path) == findName)
                    {
                        return path;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("[MockUnit] mockFilePath {0}", e.ToString());
            }

            return string.Empty;
        }

        public static ScriptableObject SaveAssetAndPingObject(ScriptableObject scriptObject)
        {
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(scriptObject);
            Selection.activeObject = scriptObject;

            return scriptObject;
        }

        public static string CreateFolder(string parentDir, string name)
        {
            string rootPathGUID = AssetDatabase.CreateFolder(parentDir, name);

            return AssetDatabase.GUIDToAssetPath(rootPathGUID);
        }

        public static ScriptableObject CreateOneScriptableObject(string parentDir, string name, Type type)
        {
            ScriptableObject scriptObject = ScriptableObject.CreateInstance(type);

            if (!System.IO.Directory.Exists(parentDir))
            {
                parentDir = CreateFolder(System.IO.Path.GetDirectoryName(parentDir), System.IO.Path.GetFileName(parentDir));
            }

            string path = System.IO.Path.Combine(parentDir, name + ".asset");

            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(scriptObject, path);

            return scriptObject;
        }

        public static ScriptableObject CreateScriptableObjectsWithFolder(string parentDir, string name, Type type)
        {
            ScriptableObject scriptObject = ScriptableObject.CreateInstance(type);

            string root = CreateFolder(parentDir, name);

            string path = System.IO.Path.Combine(root, name + ".asset");

            //DungeonID id = new DungeonID(iid);
            //id.diffID = 0;
            //id.dungeonIDWithOutDiff
            //DungeonDataManager();

            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(scriptObject, path);
            return scriptObject;
        }

        public static ScriptableObject CreateScriptObjectRecursion(string fullClsNamePrefix, string parentDir, string name, Type type)
        {
            ScriptableObject scriptObject = FileUtility.CreateScriptableObjectsWithFolder(parentDir, name, type);

            _travelScriptsObjectProperty(fullClsNamePrefix, scriptObject);

            return scriptObject;
        }

        private static void _travelScriptsObjectProperty(string fullClsNamePrefix, ScriptableObject scriptObject)
        {
            if (null == scriptObject)
            {
                return;
            }

            string path = AssetDatabase.GetAssetPath(scriptObject.GetInstanceID());
            string root = System.IO.Path.GetDirectoryName(path);

            SerializedObject serializedObject = new SerializedObject(scriptObject);
            SerializedProperty serializedProperty = serializedObject.GetIterator();

            serializedProperty.NextVisible(true);
            do
            {
                if (serializedProperty.isArray)
                {
                    serializedProperty.InsertArrayElementAtIndex(0);

                    SerializedProperty arrayElem = serializedProperty.GetArrayElementAtIndex(0);

                    if (!_createSerializedProperty(fullClsNamePrefix, root, arrayElem))
                    {
                        serializedProperty.DeleteArrayElementAtIndex(0);
                    }
                }
                else
                {
                    _createSerializedProperty(fullClsNamePrefix, root, serializedProperty);
                }
            }
            while (serializedProperty.NextVisible(false));

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static bool _createSerializedProperty(string fullClsNamePrefix, string parentDir, SerializedProperty serializedProperty)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                Type propertyType = _getSerializedPropertyTypeNameType(fullClsNamePrefix, serializedProperty);

                if (null != propertyType)
                {
                    string typeName = _getSerializedPropertyTypeName(serializedProperty);
                    serializedProperty.objectReferenceValue = CreateScriptObjectRecursion(fullClsNamePrefix, parentDir, typeName, propertyType);
                    return true;
                }
            }

            return false;
        }

        private static Type _getSerializedPropertyTypeNameType(string fullClsNamePrefix, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return null;
            }

            string typeName = _getSerializedPropertyTypeName(property);

            return Type.GetType(fullClsNamePrefix + typeName);
        }

        private static string _getSerializedPropertyTypeName(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return string.Empty;
            }

            string typeName = property.type;

            if (!typeName.Contains("PPtr<$"))
            {
                return string.Empty;
            }

            typeName = typeName.Replace("PPtr<$", "");
            typeName = typeName.Replace(">", "");

            return typeName;
        }
    }

    class MockUnit
    {
        public const string kMockProtocolPath = "Assets/Editor/MockProtocol/Protocol";

        public MockUnit(Type mockType)
        {
            mockMsgType = mockType;
            mockMsgClsName = mockType.FullName;

            msgClsName = mockMsgClsName.Replace("Mock.", "");

            msgType = Type.GetType(msgClsName);

            isMsg = typeof(global::Protocol.IGetMsgID).IsAssignableFrom(mockType);

            if (isMsg)
            {
                var f = mockMsgType.GetField("MsgID", BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static);
                msgID = (uint)f.GetValue(null);
            }

            mockFilePath = string.Empty;

            object[] attris = mockType.GetCustomAttributes(typeof(AdvancedInspector.DescriptorAttribute), false);

            Debug.LogFormat("[MockUnit] create type name {0}", msgClsName);

            if (attris.Length > 0)
            {
                AdvancedInspector.DescriptorAttribute des = attris[0] as AdvancedInspector.DescriptorAttribute;
                if (null != des)
                {
                    mockDescription = des.Name;
                }
            }
        }

        public Type mockMsgType { get; private set; }
        public Type msgType { get; private set; }
        public string mockFilePath { get; private set; }
        public string mockMsgClsName { get; private set; }
        public string msgClsName { get; private set; }

        public string mockDescription { get; private set; }

        public bool isMsg { get; private set; }
        public uint msgID { get; private set; }


        public string optionString
        {
            get
            {
                if (string.IsNullOrEmpty(mockFilePath))
                {
                    mockFilePath = FileUtility.FindFileWithName(mockMsgType.Name, kMockProtocolPath);
                }

                if (string.IsNullOrEmpty(mockFilePath))
                {
                    return string.Format("{1}({2})", mockMsgType.Name, mockDescription);
                }
                else
                {
                    var dirInfo = System.IO.Directory.GetParent(mockFilePath);
                    return string.Format("{0}/[{1}] {2}({3})", dirInfo.Name, msgID, mockMsgType.Name, mockDescription);
                }
            }
        }
    }

    class MockClientFrameUnit
    {
        public MockClientFrameUnit(Type clientFrame)
        {
            this.clientFrameType = clientFrame;
        }

        public Type clientFrameType { private set; get; }
        public string codeString
        {
            get
            {
                return string.Empty;
            }
        }
    }

    public class MockProtocolEditor : EditorWindow
    {
        private static MockProtocolEditor smMockProtocolEditor = null;
        [MenuItem("[TM工具集]/网络调试器MockProtocol")]
        public static void OpenWindows()
        {
            if (smMockProtocolEditor == null)
            {
                smMockProtocolEditor = EditorWindow.GetWindow<MockProtocolEditor>(false, "网络调试器MockProtocol" , true);
                smMockProtocolEditor._UnInit();
                smMockProtocolEditor._Init();
            }

            smMockProtocolEditor.Show();
            EditorWindow.FocusWindowIfItsOpen<SceneView>();
        }

        private MocksManager mMocksManager = null;// new MocksManager();
        private MocksController mMocksController = null;
        private bool mIsInited = false;

        public MockProtocolEditor()
        {
        }

        private void _Init()
        {
            if (mIsInited)
            {
                return;
            }

            mIsInited = true;
            mMocksManager = new MocksManager();
            mMocksController = new MocksController(mMocksManager);

            _BindCallback();
            _InitExternalEditor();

            EditorApplication.playmodeStateChanged += _OnPlayerModeStateChange;
        }

        private string mNetMessageProcess = string.Empty;

        private void _UnInit()
        {
            //NetManager.instance.sendCommandCallback -= _OnSendMessage;

            mMocksController = null;
            if (null != mMocksManager)
            {
                mMocksManager.Clear();
            }
            mMocksManager = null;
            mIsInited = false;

            _UnInitExternalEditor();
            _UnbindCallback();
            _ClearScriptableObject2Record();

            EditorApplication.playmodeStateChanged -= _OnPlayerModeStateChange;
        }

        private void _OnPlayerModeStateChange()
        {
            _UnInit();
        }

        enum eState
        {
            None = 0,

            /// <summary>
            /// 录制测试流程中
            /// </summary>
            Recording,
        }

        #region Net Callback Bind
        private void _BindCallback()
        {
            _UnbindCallback();

            if (!EditorApplication.isPlaying)
            {
                return;
            }

            NetManager.onSendCommand   += _OnSendMessage;
            NetProcess.onReciveMsgData += _OnReciveMsgData;
        }

        private void _UnbindCallback()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            NetManager.onSendCommand   -= _OnSendMessage;
            NetProcess.onReciveMsgData -= _OnReciveMsgData;
        }

        private MockRecordPrococol mRecordPrococol = null;
        private int mSeqIndex = 0;

        private void _AddScriptableObject2Record(ScriptableObject obj, eMockRecordPrococolUnitType type)
        {
            if (null == mRecordPrococol)
            {
                mRecordPrococol = ScriptableObject.CreateInstance<MockRecordPrococol>();
            }

            mRecordPrococol.allRecordedPrococols.Insert(0, new MockRecordPrococolUnit()
            {
                Seq          = mSeqIndex++,
                Timestamp    = Time.frameCount,
                Protocol     = obj,
                Type         = type,
                MockManagers = mMocksManager,
            });
        }

        private void _ClearScriptableObject2Record()
        {
            if (null == mRecordPrococol)
            {
                return;
            }

            for (int i = 0; i < mRecordPrococol.allRecordedPrococols.Count; ++i)
            {
                if(null != mRecordPrococol.allRecordedPrococols[i].Protocol)
                {
                    DestroyImmediate(mRecordPrococol.allRecordedPrococols[i].Protocol);
                }
            }

            mRecordPrococol.allRecordedPrococols.Clear();

            DestroyImmediate(mRecordPrococol);
            mRecordPrococol = null;
        }

        private void _OnReciveMsgData(MsgDATA data)
        {
            if (0 == data.id
                || global::Protocol.GateSyncServerTime.MsgID == data.id
                )
            {
                Debug.LogFormat("[MockProtocol] 收到 过滤掉了 {0}", data.id);
                return;
            }

            Debug.LogFormat("[MockProtocol] 收到 {0}", data.id);

            var mockUnit = mMocksManager.GetMockUnitByMsgId(data.id);
            if (null != mockUnit)
            {
                ScriptableObject obj = ScriptableObject.CreateInstance(mockUnit.mockMsgType.FullName);

                if (null != obj)
                {
                    var msgStream = obj as global::Protocol.IProtocolStream;

                    for (int i = 0; i < data.bytes.Length; i++)
                    {
                        skRecivedBuff[i] = data.bytes[i];
                    }

                    if (null != msgStream)
                    {
                        int pos = 0;
                        msgStream.decode(skRecivedBuff, ref pos);
                    }

                    _AddScriptableObject2Record(obj, eMockRecordPrococolUnitType.Response);
                    skExternalEditor.Instances = new object[] { mRecordPrococol };
                }
            }
        }


        private byte[] skBuff = new byte[1024 * 1024 * 16];
        private byte[] skRecivedBuff = new byte[1024 * 1024 * 16];


        private void _OnSendMessage(ServerType type, global::Protocol.IGetMsgID msg, global::Protocol.IProtocolStream msgStream)
        {
            if (null == msg || null == msgStream)
            {
                Debug.LogFormat("[MockProtocol] error {0}", type);
                return;
            }

            Debug.LogFormat("[MockProtocol] {0} 发送 {1}", type, msg.GetMsgID());


            uint msgID = msg.GetMsgID();
            if (msgID == 0)
            {
                return;
            }

            var mockUnit = mMocksManager.GetMockUnitByMsgId(msgID);
            if (null != mockUnit)
            {
                ScriptableObject obj = ScriptableObject.CreateInstance(mockUnit.mockMsgType.FullName);

                global::Protocol.IProtocolStream mockStream = obj as global::Protocol.IProtocolStream;
                if (null != mockStream)
                {
                    int pos = 0;
                    msgStream.encode(skBuff, ref pos);

                    int mockPos = 0;
                    mockStream.decode(skBuff, ref mockPos);

                    _AddScriptableObject2Record(obj, eMockRecordPrococolUnitType.Request);
                    skExternalEditor.Instances = new object[] { mRecordPrococol };
                }
            }
        }
        #endregion

        private int mSelectPopIndex          = -1;
        private string mSelectedProtocolName = string.Empty;
        private string mFilter               = string.Empty;

        public void OnGUI()
        {
            if (null == mMocksManager)
            {
                if (GUILayout.Button("点一下就加载"))
                {
                    _UnInit();
                    _Init();
                }
                return;
            }

            string filter       = EditorGUILayout.DelayedTextField(mFilter);
            string[] allOption  = mMocksManager.GetOptionsByFilter(filter);

            if (filter != mFilter)
            {
                mFilter         = filter;
                mSelectPopIndex = mMocksManager.GetOptionIndex(mSelectedProtocolName);
            }

            mSelectPopIndex = EditorGUILayout.Popup(mSelectPopIndex, allOption);

            if (mSelectPopIndex >= 0 && allOption.Length > 0)
            {
                mSelectedProtocolName = allOption[mSelectPopIndex];
            }

            if (GUILayout.Button("创建一个新协议"))
            {
                MockUnit unit = mMocksManager.GetMockUnitByMockMsgTypeName(mSelectedProtocolName);
                if (null != unit)
                {
                    Ready2SendMessage = mMocksController.CreateMockProtocol(unit);
                }
                else
                {
                    UnityEngine.Debug.LogErrorFormat("[MockProtocol] {0} not found!", mSelectedProtocolName);
                }
            }

            //if (GUILayout.Button("Bind"))
            //{
            //    _BindCallback();
            //    //Selection.activeInstanceID = AssetDatabase.AssetPathToGUID(MockUnit.kMockProtocolPath);
            //    //EditorGUIUtility.
            //}

            EditorGUILayout.Space();

            //if (GUILayout.Button("创建一个新的测试流程"))
            //{
            //    ScriptableObject process = mMocksController.CreateMockProcess();
            //}

            //if (GUILayout.Button("开始录制流程"))
            //{

            //}

            var readySendMessage = EditorGUILayout.ObjectField(Ready2SendMessage, typeof(UnityEngine.ScriptableObject)) as UnityEngine.ScriptableObject;

            if (_CheckIsMockObject(readySendMessage))
            {
                Ready2SendMessage = readySendMessage;
            }


            EditorGUILayout.Space();

            if (null != Ready2SendMessage)
            {
                uint msgID = _GetMsgObjectMsgId(Ready2SendMessage);

                if (0 != msgID)
                {
                    MockUnit unit = mMocksManager.GetMockUnitByMsgId(msgID);

                    if (null != unit)
                    {

                        EditorGUILayout.LabelField(string.Format("[{0}] {1}", unit.msgID, unit.mockDescription));

                        if (GUILayout.Button("发送消息"))
                        {
                            mMocksController.SendMessage(Ready2SendMessage as Protocol.IMockProtocol);
                        }

                        if (GUILayout.Button("假装收到消息"))
                        {
                            mMocksController.RecivedMessage(Ready2SendMessage as Protocol.IMockProtocol);
                        }
                    }
                }
            }

            int posh1 = 150;

            if (null != skWait2SendExternalEditor)
            {
                Rect pre = new Rect();
                pre.size = new Vector2(position.width, posh1);
                pre.position = new Vector2(0, posh1);
                skWait2SendExternalEditor.Draw(pre);
                Repaint();
            }

            if (null != skExternalEditor)
            {
                Rect pre = new Rect();
                int posh = 150 + posh1;
                pre.size = new Vector2(position.width, position.height - posh);
                pre.position = new Vector2(0, posh);
                skExternalEditor.Draw(pre);
                Repaint();
            }
        }

        private static ExternalEditor skExternalEditor = null;
        private static ExternalEditor skWait2SendExternalEditor = null;

        private void _InitExternalEditor()
        {
            skExternalEditor = ExternalEditor.CreateInstance<ExternalEditor>();
            skWait2SendExternalEditor = ExternalEditor.CreateInstance<ExternalEditor>();
        }

        private void _UnInitExternalEditor()
        {
            if (null != skExternalEditor)
            {
                ExternalEditor.DestroyImmediate(skExternalEditor);
                skExternalEditor = null;
            }

            if (null != skWait2SendExternalEditor)
            {
                ExternalEditor.DestroyImmediate(skWait2SendExternalEditor);
                skWait2SendExternalEditor = null;
            }
        }

        private UnityEngine.ScriptableObject mReady2SendMessage = null;

        private ScriptableObject Ready2SendMessage
        {
            get
            {
                return mReady2SendMessage;
            }

            set
            {
                mReady2SendMessage = value;

                if (null != skWait2SendExternalEditor)
                {
                    skWait2SendExternalEditor.Instances = new object[] { value };
                }
            }
        }

        private bool _CheckIsMockObject(UnityEngine.ScriptableObject msg)
        {
            return msg is Mock.Protocol.IMockProtocol;
        }

        private uint _GetMsgObjectMsgId(UnityEngine.ScriptableObject msg)
        {
            var msgID = msg as global::Protocol.IGetMsgID;

            if (null == msgID)
            {
                return 0;
            }

            return msgID.GetMsgID();
        }


        private void _updateCreatePart()
        {

        }
    }
}
