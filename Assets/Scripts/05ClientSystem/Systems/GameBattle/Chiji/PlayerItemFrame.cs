using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class PlayerItemFrame : ClientFrame
    {
        List<OtherPlayerDetailItem> detailItems = new List<OtherPlayerDetailItem>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/PlayerItemFrame";
        }

        protected override void _OnOpenFrame()
        {
            ChijiOtherPlayerItems otherPlayerItems = ChijiDataManager.GetInstance().OtherPlayerItems;
            if (otherPlayerItems != null && otherPlayerItems.detailItems != null)
            {
                detailItems = otherPlayerItems.detailItems;
            }

            _InitPlayerItemScrollListBind();
            RefreshPlayerItemListCount();
            _SetTitleName();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PickUpLoserItem, _OnPickUpLoserItem);
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
        }

        private void _ClearData()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PickUpLoserItem, _OnPickUpLoserItem);

            if(detailItems != null)
            {
                detailItems.Clear();
            }
        }

        void _OnPickUpLoserItem(UIEvent uiEvent)
        {
            UInt64 guid = (UInt64)uiEvent.Param1;

            for(int i = 0; i < detailItems.Count; i++)
            {
                if(guid == detailItems[i].guid)
                {
                    detailItems.RemoveAt(i);
                    break;
                }
            }

            RefreshPlayerItemListCount();
        }

        void _InitPlayerItemScrollListBind()
        {
            mPlayerItemsScrollView.Initialize();

            mPlayerItemsScrollView.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateItemScrollListBind(item);
                }
            };

            mPlayerItemsScrollView.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button iconBtn = combind.GetCom<Button>("item");
                iconBtn.onClick.RemoveAllListeners();
            };
        }

        void _UpdateItemScrollListBind(ComUIListElementScript item)
        {
            ChijiOtherPlayerItems otherPlayerItems = ChijiDataManager.GetInstance().OtherPlayerItems;
            if(otherPlayerItems == null)
            {
                return;
            }

            if (detailItems == null || item.m_index < 0 || item.m_index >= detailItems.Count)
            {
                return;
            }

            OtherPlayerDetailItem detail = detailItems[item.m_index];
            if(detail == null)
            {
                return;
            }

            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            Text name = combind.GetCom<Text>("name");
            Image icon = combind.GetCom<Image>("icon");
            Button btItem = combind.GetCom<Button>("item");
            Image itemBg = combind.GetCom<Image>("itemBg");
            Text num = combind.GetCom<Text>("num");

            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)detail.itemTypeId);

            if (name != null)
            {
                if(itemData != null)
                {
                    name.text = itemData.GetColorName();
                }  
            }

            if (icon != null)
            {
                if (itemData != null)
                {
                    ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
                }    
            }

            if (itemBg != null)
            {
                if(itemData != null)
                {
                    ETCImageLoader.LoadSprite(ref itemBg, itemData.GetQualityInfo().Background);
                }
            }

            if (num != null)
            {
                if (detail.num > 0)
                {
                    num.text = detail.num.ToString();
                    num.gameObject.CustomActive(true);
                }
                else
                {
                    num.gameObject.CustomActive(false);
                }
            }

            if (btItem != null)
            {
                btItem.onClick.RemoveAllListeners();

                UInt64 playerid = otherPlayerItems.playerID;
                UInt64 guid = detail.guid;

                btItem.onClick.AddListener(() => { _OnClickPlayerItem(guid, playerid); });
            }
        } 

        public void RefreshPlayerItemListCount()
        {
            if(mPlayerItemsScrollView == null)
            {
                return;
            }

            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            if(detailItems != null)
            {
                mPlayerItemsScrollView.SetElementAmount(detailItems.Count);
            }
        }
        
        private void _SetTitleName()
        {
            ChijiOtherPlayerItems otherPlayerItems = ChijiDataManager.GetInstance().OtherPlayerItems;
            JoinPlayerInfo playerInfo = null;
            if (ChijiDataManager.GetInstance().JoinPlayerInfoList != null)
            {
                for (int i = 0; i < ChijiDataManager.GetInstance().JoinPlayerInfoList.Count; i++)
                {
                    var info = ChijiDataManager.GetInstance().JoinPlayerInfoList[i];
                    if (info.playerId != otherPlayerItems.playerID)
                    {
                        continue;
                    }

                    playerInfo = info;
                    break;
                }
            }

            if (mTitleName != null && playerInfo != null)
            {
                mTitleName.text = string.Format("{0}的物品", playerInfo.playerName);
            }
        }

        void _OnClickPlayerItem(UInt64 guid, UInt64 playerid)
        {
            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not GameBattle!!!");
                return;
            }

            ChijiDataManager.GetInstance().SendPickUpOtherPlayerItems(guid, playerid);
        }
        
		#region ExtraUIBind
		private ComUIListScript mPlayerItemsScrollView = null;
		private Button mAllPickUpItem = null;
        private Button mClose = null;
        private Text mTitleName = null;

        protected override void _bindExUI()
		{
            mPlayerItemsScrollView = mBind.GetCom<ComUIListScript>("PlayerItemsScrollView");
            mAllPickUpItem = mBind.GetCom<Button>("AllPickUpItem");
			if (null != mAllPickUpItem)
			{
                mAllPickUpItem.onClick.AddListener(_onAllPickUpItemButtonClick);
			}
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
            mTitleName = mBind.GetCom<Text>("title");
        }
		
		protected override void _unbindExUI()
		{
            mPlayerItemsScrollView = null;
			if (null != mAllPickUpItem)
			{
                mAllPickUpItem.onClick.RemoveListener(_onAllPickUpItemButtonClick);
			}
            mAllPickUpItem = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mTitleName = null;

        }
        #endregion

        #region Callback
        private void _onAllPickUpItemButtonClick()
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current == null)
            {
                return;
            }

            ChijiOtherPlayerItems otherPlayerItems = ChijiDataManager.GetInstance().OtherPlayerItems;
            if (otherPlayerItems != null)
            {
                if (detailItems != null)
                {
                    for (int i = 0; i < detailItems.Count; i++)
                    {
                        ChijiDataManager.GetInstance().SendPickUpOtherPlayerItems(detailItems[i].guid, otherPlayerItems.playerID);
                    }
                }
            }

            _onCloseButtonClick();
        }

        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
