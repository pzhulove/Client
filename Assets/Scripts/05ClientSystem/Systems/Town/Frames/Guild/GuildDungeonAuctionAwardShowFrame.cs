using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    // 公会地下城排名奖励展示界面
    public class GuildDungeonAuctionAwardShowFrame : ClientFrame
    {
        #region val
        List<AwardItemData> awardItemDataList = null;

        #endregion

        #region ui bind
        ComUIListScript awardItems = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonAuctionAwardShow";
        }

        protected override void _OnOpenFrame()
        {
            awardItemDataList = null;

            InitAwardItems();      
            UpdateAwardItems();

            BindUIEvent();       
        }

        protected override void _OnCloseFrame()
        {
            awardItemDataList = null;

            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            awardItems = mBind.GetCom<ComUIListScript>("awardItems");
        }

        protected override void _unbindExUI()
        {
            awardItems = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void InitAwardItems()
        {
            if(awardItems == null)
            {
                return;
            }

            awardItems.Initialize();

            awardItems.onBindItem = (GameObject go) =>
            {
                PayRewardItem payItem = null;
                if (go)
                {
                    payItem = go.GetComponent<PayRewardItem>();
                }
               
                return payItem;
            };

            awardItems.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }

                int iIndex = var1.m_index;
                if (iIndex >= 0 && awardItemDataList != null && iIndex < awardItemDataList.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataList[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("GuildDungeonAuctionAwardShowFrame Can not find item id in item table!!! Please Check item data id {0} !!!", awardItemDataList[iIndex].ID);
                        return;
                    }

                    itemDetailData.Count = awardItemDataList[iIndex].Num;
                    PayRewardItem payItem = var1.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(this, itemDetailData, true, false);
                        payItem.RefreshView();
                    }
                }
            };
        }

        void CalAwardItemList()
        {
            awardItemDataList = new List<AwardItemData>();
            if(awardItemDataList == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildDungeonRewardTable adt = iter.Current.Value as GuildDungeonRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.rewardType != 14)
                    {
                        continue;
                    }

                    for (int i = 0; i < adt.rewardShowLength; i++)
                    {
                        string strReward = adt.rewardShowArray(i);
                        string[] reward = strReward.Split('_');
                        if (reward.Length >= 2)
                        {
                            AwardItemData data = new AwardItemData();
                            int.TryParse(reward[0], out data.ID);
                            int.TryParse(reward[1], out data.Num);
                            awardItemDataList.Add(data);
                        }
                    }
                }
            }

            return;
        }        

        void UpdateAwardItems()
        {
            if(awardItems == null)
            {
                return;
            }

            CalAwardItemList();

            if(awardItemDataList != null)
            {
                awardItems.SetElementAmount(awardItemDataList.Count);
            }
        }

        #endregion

        #region ui event

        #endregion
    }
}
