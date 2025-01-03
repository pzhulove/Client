using Protocol;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class GuildMergeAskFrame : ClientFrame
    {

        private ComUIListScript mItemList = null;
        private Button mCloseBtn = null;
        private Button mClearAllBtn = null;

        private List<GuildEntry> mGuildList = new List<GuildEntry>();

        #region UIBase
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMergeAskFrame";
        }

        protected override void _bindExUI()
        {
            mItemList = mBind.GetCom<ComUIListScript>("ItemList");
            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.SafeAddOnClickListener(OnCloseBtnClick);
            mClearAllBtn = mBind.GetCom<Button>("ClearAllBtn");
            mClearAllBtn.SafeAddOnClickListener(OnClearAllAskInfoBtnClick);
        }



        protected override void _unbindExUI()
        {
            mItemList = null;
            mCloseBtn.SafeRemoveOnClickListener(OnCloseBtnClick);
            mCloseBtn = null;
            mClearAllBtn.SafeRemoveOnClickListener(OnClearAllAskInfoBtnClick);
            mClearAllBtn = null;
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            if (mItemList != null && !mItemList.IsInitialised())
            {
                mItemList.Initialize();
                mItemList.onItemVisiable += OnItemVisiable;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildReceiveMergedList, OnGuildReceiveMergedList);
            GuildDataManager.GetInstance().RequestGuildHaveMgergeRequest();
        }



        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mGuildList.Clear();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildReceiveMergedList, OnGuildReceiveMergedList);
        }
        #endregion

        private void OnGuildReceiveMergedList(UIEvent uiEvent)
        {

            GuildEntry[] lt = (GuildEntry[])uiEvent.Param1;
            if (lt != null)
            {
                mGuildList.Clear();
                for (int i = 0; i < lt.Length; i++)
                {
                    mGuildList.Add(lt[i]);
                }
            }
            mItemList.SetElementAmount(mGuildList.Count);

        }
        private void OnItemVisiable(ComUIListElementScript item)
        {
            if (item != null && item.m_index <mGuildList.Count&&item.m_index>=0)
            {
                GuildMergeAskItem askItem = item.GetComponent<GuildMergeAskItem>();
                if (askItem != null)
                {
                    askItem.SetData(mGuildList[item.m_index]);
                }
            }
        }

        private void OnCloseBtnClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<GuildMergeAskFrame>();
        }
        private void OnClearAllAskInfoBtnClick()
        {
            GuildDataManager.GetInstance().AcceptOrRefuseOrCancelMergeRequest(3,0);
        }

    }
}
