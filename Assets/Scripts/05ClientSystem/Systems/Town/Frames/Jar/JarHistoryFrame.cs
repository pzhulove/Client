using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using EJarType = ProtoTable.JarBonus.eType;
using Protocol;
using ProtoTable;

namespace GameClient
{

    class JarHistoryFrame : ClientFrame
    {
        private ComUIListScript mComUIList = null;
        private CanvasGroup mCanvasNoHistory = null;
        private WorldOpenJarRecordRes m_recordData = null;

        #region ClientFrame
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/JarHistoryFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            BindUIEvent();

            if (userData != null && userData is int)
            {
                _GetHistory((int)userData);
            }
        }

        private void _GetHistory(int id)
        {
            JarDataManager.GetInstance().RequestJarBuyRecord(id);
        }

        void _OnUpdateOpenRecord(UIEvent a_event)
        {
            _ClearRecord();
            m_recordData = a_event.Param1 as WorldOpenJarRecordRes;
            _UpdateRecord();
        }

        void _ClearRecord()
        {
            mComUIList.SetElementAmount(0);
            m_recordData = null;
        }

        void _UpdateRecord()
        {
            if (m_recordData != null)
            {
                mComUIList.SetElementAmount(m_recordData.records.Length);
                mComUIList.EnsureElementVisable(0);
                mCanvasNoHistory.CustomActive(m_recordData.records == null || m_recordData.records.Length <= 0);
            }
            else
            {
                mCanvasNoHistory.CustomActive(true);
                mComUIList.SetElementAmount(0);
            }
        }

        private void _OnItemUpdate(ComUIListElementScript var)
        {
            if (var == null)
            {
                return;
            }

            if (m_recordData != null)
            {
                if (var.m_index >= 0 && var.m_index < m_recordData.records.Length)
                {
                    int nIdx = m_recordData.records.Length - var.m_index - 1;
                    OpenJarRecord record = m_recordData.records[nIdx];
                    ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)record.itemId);
                    if (item != null)
                    {
                        string strItem = string.Format(" {{I 0 {0} 0}}", record.itemId);
                        var.GetComponent<LinkParse>().SetText(TR.Value("jar_record", record.name, strItem, record.num));
                    }
                }
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            _UnRegisterUIEvent();

        }
        
        protected override void _bindExUI()
        {
            mComUIList = mBind.GetCom<ComUIListScript>("ComUIList");
            if (mComUIList != null)
            {
                mComUIList.Initialize();
                mComUIList.onItemVisiable = _OnItemUpdate;
                mComUIList.OnItemUpdate = _OnItemUpdate;
            }

            mCanvasNoHistory = mBind.GetCom<CanvasGroup>("NoHistory");
        }

        protected override void _unbindExUI()
        {
            mComUIList = null;
            mCanvasNoHistory = null;

            m_recordData = null;
        }
        #endregion


        

        #region UIEvent
        //UI系统之间事件
        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
        }

        //UI控件之间事件的绑定，应该分离到View
        void BindUIEvent()
        {

        }

        void UnBindUIEvent()
        {
        }

        public sealed override bool IsNeedUpdate()
        {
            return false;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
        }

        #endregion

        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
