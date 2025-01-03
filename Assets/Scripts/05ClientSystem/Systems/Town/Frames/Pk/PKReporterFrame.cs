using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Scripts.UI;
namespace GameClient
{
    public class PKReporterFrame : ClientFrame
    {
        private int mSelectType = 0;
        [UIControl("middle/labFileSize", typeof(Text))]
        Text mFileDescript;
        StateController comState;
        [UIControl("middle/InputField", typeof(InputField))]
        InputField m_inputField;
        string m_SessionId = string.Empty;
        bool isInBattle = false;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PKReporter";
        }
        protected override void _OnOpenFrame()
        {
            mSelectType = 0;
            var info = userData as VideoInfo;
            if (info != null)
            {
                isInBattle = info.isInBattle;
                m_SessionId = info.sessionId;
            }
            int fileSize = 0;
            if (mFileDescript != null)
            {
#if !LOGIC_SERVER
                if (RecordServer.GetInstance() != null)
                {
                    if (string.IsNullOrEmpty(m_SessionId))
                    {
                        fileSize = RecordServer.GetInstance().GetCurrentRecordSize();
                    }
                    else
                    {
                        fileSize = RecordServer.GetInstance().GetPkRecordSize(m_SessionId);
                    }
                }
#endif
                if (isInBattle)
                {
                    mFileDescript.text = "";
                }
                else
                {
                    mFileDescript.text = string.Format("(录像文件:{0:N1}K)", fileSize / 1024.0f);
                }
            }
        }

        [UIEventHandle("middle/btUpload")]
        void _OnConfirmClicked()
        {
            if(mSelectType == -1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请选择反馈类型");
                return;
            }
            if (m_inputField != null)
            {
                if (m_inputField.text.Length <= 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请描述录像问题原因");
                    return;
                }

            }
#if !LOGIC_SERVER
            if (RecordServer.GetInstance() != null)
            {
                if (isInBattle)
                {
                    string errorReason = "";
                    if (!RecordServer.GetInstance().EndRecordInBattle(ref errorReason, mSelectType, m_inputField != null ? m_inputField.text : ""))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format("上传文件出错 原因:{0}", errorReason));
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUploadFileSucc, userData);
                        SystemNotifyManager.SysNotifyFloatingEffect("上传成功");
                    }
                    frameMgr.CloseFrame(this);
                    return;
                }

                if (string.IsNullOrEmpty(m_SessionId))
                {
                    string errorReason = "";
                    if (!RecordServer.GetInstance().UpLoadCurrentRecordFile(ref errorReason, mSelectType, m_inputField != null ? m_inputField.text : ""))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format("上传文件出错 原因:{0}", errorReason));
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUploadFileSucc);
                        SystemNotifyManager.SysNotifyFloatingEffect("上传成功");
                    }
                }
                else
                {
                    string errorReason = "";
                    if (!RecordServer.GetInstance().UpLoadRecordFile(m_SessionId, ref errorReason, mSelectType, m_inputField != null ? m_inputField.text : ""))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format("上传文件出错 原因:{0}", errorReason));
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUploadFileSucc, userData);
                        SystemNotifyManager.SysNotifyFloatingEffect("上传成功");
                    }
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("系统出错无法上传");
            }
#endif
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("middle/btClose")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUploadFileClose);
        }

        [UIEventHandle("middle/Panel/cbType{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, 3)]
        void OnChangeType(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                mSelectType = -1;
                return;
            }
            mSelectType = iIndex;
        }
    }
}
