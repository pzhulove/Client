using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using System;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    public class GuildDungeonBufAddUpDetailFrame : ClientFrame
    {

        List<GuildDataManager.BuffAddUpInfo> buffAddUpInfos = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonBufAddUpDetail";
        }
        protected override void _OnOpenFrame()
        {
            buffAddUpInfos = new List<GuildDataManager.BuffAddUpInfo>();
            BindEvent();
            UpdateBufAddUpItems(0);
        }
        protected override void _OnCloseFrame()
        {
            buffAddUpInfos = null;
            UnBindEvent();
            ClearData();        
        }

        private void BindEvent()
        {

        }

        private void UnBindEvent()
        {

        }


        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        List<GuildDataManager.BuffAddUpInfo> GetBuffAddUpInfoList()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data == null)
            {
                return null;
            }

            return data.buffAddUpInfos;
        }     

        private void UpdateBufAddUpItems(int selectPayActId)
        {
            if (buffAddUpInfos == null)
            {
                Logger.LogError("ItemdataList data is null");
                return;
            }

            if (mScrollUIList == null)
            {
                Logger.LogError("mScrollUIList obj is null");
                return;
            }


            if (mScrollUIList.IsInitialised() == false)
            {
                mScrollUIList.Initialize();
                mScrollUIList.onBindItem = (GameObject go) =>
                {
                    GuildDungeonBuffAddUpInfoItem item = null;
                    if (go)
                    {
                        item = go.GetComponent<GuildDungeonBuffAddUpInfoItem>();
                    }
                  
                    return item;
                };
            }
            mScrollUIList.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }
                int iIndex = var1.m_index;
                if (iIndex >= 0 && iIndex < buffAddUpInfos.Count)
                {                    
                    GuildDungeonBuffAddUpInfoItem item = var1.gameObjectBindScript as GuildDungeonBuffAddUpInfoItem;
                    if (item != null)
                    {
                        item.SetUp(buffAddUpInfos[iIndex]);
                    }
                }
            };

            buffAddUpInfos = GetBuffAddUpInfoList();
     
            mScrollUIList.SetElementAmount(buffAddUpInfos.Count);                
        }
     
        private void ClearData(bool isClearWithTabs = true)
        {
            if (mScrollUIList != null)
            {
                mScrollUIList.SetElementAmount(0);
            }         
        }     
     

        #region EventCallback
        

        #endregion

		#region ExtraUIBind
		private ComUIListScript mScrollUIList = null;
        private Button btnClose = null;


        protected override void _bindExUI()
		{			
            btnClose = mBind.GetCom<Button>("btnClose");
            if (null != btnClose)
            {
                btnClose.onClick.AddListener(_onBtnCloseButtonClick);
            }

			mScrollUIList = mBind.GetCom<ComUIListScript>("ScrollUIList");

		}
		
		protected override void _unbindExUI()
		{
		
            if (null != btnClose)
            {
                btnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            }
            btnClose = null;         
			mScrollUIList = null;			
		}
		#endregion

        #region Callback  
        private void _onBtnCloseButtonClick()
        {
            /* put your code in here */
            OnClickClose();
        }
        #endregion
    }

}

