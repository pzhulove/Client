using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class MailNewFrame : ClientFrame
    {
        #region ExtraUIBind
        private MailNewView mMailFrameNew = null;

        protected sealed override void _bindExUI()
        {
            MailDataManager.GetInstance().UpdateOpenMailTabType();

            mMailFrameNew = mBind.GetCom<MailNewView>("MailFrameNew");
        }

        protected sealed override void _unbindExUI()
        {
            mMailFrameNew = null;
        }
        #endregion
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mail/MailFrameNew";
        }

        protected sealed override void _OnOpenFrame()
        {
            RegisterUIEventHandler();

            mMailFrameNew.InitView(OnMailMainTabClick);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnRegisterUIEventHandler();

            MailDataManager.GetInstance().SortMailList();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
        }

        private void RegisterUIEventHandler()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewMailNotify, OnUpdateNewMailNotify);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMailDeleteSuccess, OnMailDeleteSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReadMailResSuccess, OnReadMailResSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateMailStatus, OnUpdateMailTitleInfoList);
        }

        private void UnRegisterUIEventHandler()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewMailNotify, OnUpdateNewMailNotify);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMailDeleteSuccess, OnMailDeleteSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReadMailResSuccess, OnReadMailResSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateMailStatus, OnUpdateMailTitleInfoList);
        }

        private void OnUpdateNewMailNotify(UIEvent uiEvent)
        {
            List<MailTitleInfo> mCurrentMailTitleInfoList = MailDataManager.GetInstance().GetCurrentShowMailTitleInfoList();

            mMailFrameNew.UpdateNewMailNotify(mCurrentMailTitleInfoList);
            mMailFrameNew.UpdateRedPoint();
        }

        private void OnUpdateMailTitleInfoList(UIEvent uiEvent)
        {
            List<MailTitleInfo> mCurrentMailTitleInfoList = MailDataManager.GetInstance().GetCurrentShowMailTitleInfoList();

            mMailFrameNew.UpdateMailTitleInfo(mCurrentMailTitleInfoList);
            mMailFrameNew.UpdateRedPoint();
        }

        private void OnMailDeleteSuccess(UIEvent uiEvent)
        {
            List<MailTitleInfo> mCurrentMailTitleInfoList = MailDataManager.GetInstance().GetCurrentShowMailTitleInfoList();
            mMailFrameNew.UpdateSelfMailTitleInfoList(mCurrentMailTitleInfoList);
            mMailFrameNew.UpdateRedPoint();
        }

        private void OnReadMailResSuccess(UIEvent uiEvent)
        {
            var mailDataModel = uiEvent.Param1 as MailDataModel;
            mMailFrameNew.UpdateMailInfoMationView(mailDataModel);
        }
        
        private void OnMailMainTabClick(CommonTabData tabData)
        {
            if (tabData == null)
            {
                return;
            }

            MailDataManager.CurrentSelectMailTabType = (MailTabType)tabData.id;

            List<MailTitleInfo> mCurrentMailTitleInfoList = MailDataManager.GetInstance().GetCurrentShowMailTitleInfoList();

            mMailFrameNew.UpdateBtnStatue(tabData);

            mMailFrameNew.UpdateSelfMailTitleInfoList(mCurrentMailTitleInfoList);
        }
    }
}

