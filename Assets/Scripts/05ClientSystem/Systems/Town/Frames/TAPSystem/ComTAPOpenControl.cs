using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    enum TAPSystemTabType
    {
        TSTT_RELATION_INFO = 0,
        TSTT_COUNT,
    }

    class TAPTabData
    {
        public delegate bool OpenCheck();
        public delegate ClientFrame OnTabChanged(TAPSystemTabType eTAPSystemTabType, object data);

        public TAPSystemTabType eTAPSystemTabType = TAPSystemTabType.TSTT_COUNT;
        public string name;
        public OpenCheck isOpen;
        public OnTabChanged onTabChanged;
        public GameObject root;
    }

    class TAPSystemMainFrameData
    {
        public TAPSystemTabType eTAPSystemTabType = TAPSystemTabType.TSTT_RELATION_INFO;
    }
    //2020/08/12
    //以上三个从“TAPSystemMainFrame”挪过来的，被我删了，那个界面没用了。如果这个脚本也没用了，要删掉，现在不确定。

    class ComTAPOpenControl : MonoBehaviour
    {
        public UnityEvent onOk;
        public UnityEvent onFailed;

        void Awake()
        {
            _RegisterEvent();
            _Check();
        }

        void _RegisterEvent()
        {
            PlayerBaseData.GetInstance().onLevelChanged += _OnLevelChanged;
        }

        void _OnLevelChanged(int iPre,int iCur)
        {
            _Check();
        }

        void _UnRegisterEvent()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= _OnLevelChanged;
        }

        void _Check()
        {
            bool bOpened = IsOpen();
            var action = bOpened ? onOk : onFailed;
            if (null != action)
            {
                action.Invoke();
            }
        }

        public static bool IsOpen()
        {
            int iOpenLevel = 99;
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TAP_SYSTEM_OPEN_LEVEL);
            if (null != systemValue)
            {
                iOpenLevel = systemValue.Value;
            }
            return iOpenLevel <= PlayerBaseData.GetInstance().Level;
        }

        public static ClientFrame _OpenTAPSystem(TAPSystemTabType eTAPSystemTabType,object data)
        {
            TAPTabData tabData = data as TAPTabData;
            if(null != tabData)
            {
                switch (eTAPSystemTabType)
                {
                    case TAPSystemTabType.TSTT_RELATION_INFO:
                        {
                            //RelationDataManager.GetInstance().MakeDebugPupilDatas();
                            var clientFrame = ClientSystemManager.GetInstance().OpenFrame<TAPFrame>(tabData.root, tabData) as ClientFrame;
                            return clientFrame;
                        }
                        break;
                }
            }
            return null;
        }

        void OnDestroy()
        {
            _UnRegisterEvent();
        }
    }
}