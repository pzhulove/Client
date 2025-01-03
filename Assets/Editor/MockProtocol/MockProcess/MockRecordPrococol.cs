using UnityEngine;
using System.Collections.Generic;
using AdvancedInspector;

namespace Mock
{
    public enum eMockRecordPrococolUnitType
    {
        [AdvancedInspector.Descriptor("请求", "")]
        Request,

        [AdvancedInspector.Descriptor("应答", "")]
        Response,
    }

    [System.Serializable]
    
    struct MockRecordPrococolUnit
    {
        [AdvancedInspector.Descriptor("类型", "")]
        [AdvancedInspector.ReadOnly]
        public eMockRecordPrococolUnitType Type;

        [AdvancedInspector.Descriptor("序号", "")]
        [AdvancedInspector.ReadOnly]
        public int Seq;

        [AdvancedInspector.Descriptor("时间戳", "")]
        [AdvancedInspector.ReadOnly]
        public int Timestamp;

        [AdvancedInspector.Descriptor("消息内容", "")]
        [AdvancedInspector.Expandable(AlwaysExpanded = true, Expandable = true, Expanded = true)]
        //[AdvancedInspector.ReadOnly]
        public ScriptableObject Protocol;

        [System.NonSerialized]
        public MocksManager MockManagers;

        private string _GetTypeString()
        {
            if (Type == eMockRecordPrococolUnitType.Request)
            {
                return "*" + Type;
            }

            return Type.ToString();
        }

        private string _GetProcotolDesc()
        {
            var msgIDGetter = Protocol as global::Protocol.IGetMsgID;
            if (null == msgIDGetter)
            {
                return "[-1] 空空";
            }

            var msgId = msgIDGetter.GetMsgID();
            if (msgId == 0)
            {
                return "[0] 这或许是一个错误";
            }

            if (null != MockManagers)
            {
                MockUnit unit = MockManagers.GetMockUnitByMsgId(msgId);
                if (null != unit)
                {
                    return string.Format("[{0}] {1} {2}#", msgId, unit.msgClsName, unit.mockDescription);
                }
            }

            return string.Format("[{0}] {1}#", msgId, Protocol.name);
        }

        public override string ToString()
        {
            return string.Format("No.{0} {1} {2}", Seq, _GetTypeString(), _GetProcotolDesc());
        }
    }

    class MockRecordPrococol : ScriptableObject
    {
        [AdvancedInspector.Descriptor("所有记录的消息", "")]
        [AdvancedInspector.Collection(AlwaysExpanded = true, Sortable = false)]
        public List<MockRecordPrococolUnit> allRecordedPrococols = new List<MockRecordPrococolUnit>();
    }
}
