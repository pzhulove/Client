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
using Network;

namespace GameClient
{
    class GuildDungeonDamageRankListFrame : ClientFrame
    {
        class AwardRankItem
        {
            public int iRankMin;
            public int iRankMax;
            public List<ItemData> arrItems = new List<ItemData>();
        }

        [UIControl("Tab/Func1", typeof(Toggle))]
        Toggle toggleScoreRank;

        [UIControl("Tab/Func2", typeof(Toggle))]
        Toggle toggleRankAward;

        [UIObject("ScoreList")]
        GameObject goScoreList;

        [UIObject("AwardList")]
        GameObject goAwardList;

        [UIControl("ScoreList/Content")]
        ComUIListScript m_comScoreList;

        [UIControl("AwardList/Content")]
        ComUIListScript m_comAwardList;

        [UIObject("ScoreList/Content/MyRank")]
        GameObject goMyScoreRank;

        private ComUIListScript mScrollUIList = null;
        private ComUIListScript mScrollUIList2 = null;
        private ComUIListScript mScrollUIList3 = null;

        private GuildDungeonAwardsShowItem showItems = null;
        List<AwardRankItem> m_arrAwardItems = null;

        List<AwardItemData> awardItemDataList = new List<AwardItemData>(); 
        List<AwardItemData> awardItemDataList2 = new List<AwardItemData>();

        private Text txtTotalDamage = null;

        Dictionary<GuildDungeonAwardsShowItem.AwardType, List<AwardItemData>> awardType2ItemDatas = null;
        protected override void _bindExUI()
        {
            txtTotalDamage = mBind.GetCom<Text>("txtTotalDamage");

            mScrollUIList = mBind.GetCom<ComUIListScript>("ScrollUIList");
            mScrollUIList2 = mBind.GetCom<ComUIListScript>("ScrollUIList2");
            mScrollUIList3 = mBind.GetCom<ComUIListScript>("ScrollUIList3");
            showItems = mBind.GetCom<GuildDungeonAwardsShowItem>("showItems");
        }

        protected override void _unbindExUI()
        {
            txtTotalDamage = null;

            mScrollUIList = null;
            mScrollUIList2 = null;
            mScrollUIList3 = null;
            showItems = null;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonDamageRankList";
        }       

        public static void OnOpenLinkFrame(string argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildDungeonDamageRankListFrame>(FrameLayer.Middle);
        }

        protected override void _OnOpenFrame()
        {
            m_arrAwardItems = new List<AwardRankItem>();
            if(m_arrAwardItems == null)
            {
                Logger.LogErrorFormat("new List<AwardRankItem>() error!!!");
            }
            else
            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWarRewardTable>();
                if(dicts == null)
                {
                    Logger.LogErrorFormat("TableManager.instance.GetTable<ScoreWarRewardTable>() error!!!");
                }
                else
                {

                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ScoreWarRewardTable adt = iter.Current.Value as ScoreWarRewardTable;
                        if(adt != null && adt.RankingBegin > 0 && adt.RankingEnd > 0)
                        {
                            AwardRankItem awardRankItem = new AwardRankItem();
                            awardRankItem.iRankMin = adt.RankingBegin;
                            awardRankItem.iRankMax = adt.RankingEnd;

                            for(int i = 0;i < adt.ItemReward.Count;i++)
                            {
                                string strReward = adt.ItemRewardArray(i);
                                string[] reward = strReward.Split('_');
                                if(reward.Length >= 2)
                                {
                                    int id = int.Parse(reward[0]);
                                    int iCount = int.Parse(reward[1]);
                                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
                                    if(itemData != null)
                                    {
                                        itemData.Count = iCount;
                                        awardRankItem.arrItems.Add(itemData);
                                    }                                    
                                }
                            }
                            
                            m_arrAwardItems.Add(awardRankItem);
                        }
                    }
                }
            }
            awardType2ItemDatas = new Dictionary<GuildDungeonAwardsShowItem.AwardType, List<AwardItemData>>();
            if(awardType2ItemDatas != null)
            {
                awardType2ItemDatas.Add(GuildDungeonAwardsShowItem.AwardType.Kill3ShiTu,GetKill3ShiTuAwardList());
                awardType2ItemDatas.Add(GuildDungeonAwardsShowItem.AwardType.NotKillBigBoss,GetNotKillBossAwardList());                                
            }           

            if(toggleScoreRank != null)
            {
                toggleScoreRank.onValueChanged.AddListener((bool bValue) =>
                {
                    if (goScoreList != null)
                    {
                        goScoreList.CustomActive(true);
                    }

                    if (goAwardList != null)
                    {
                        goAwardList.CustomActive(false);
                    }
                    RefreshRankList();
                });
            }

            if (toggleRankAward != null)
            {
                toggleRankAward.onValueChanged.AddListener((bool bValue) =>
                {
                    if (goScoreList != null)
                    {
                        goScoreList.CustomActive(false);
                    }

                    if (goAwardList != null)
                    {
                        goAwardList.CustomActive(true);
                    }
                    RrefeshRankAward();
                });
            }

            if(goScoreList != null)
            {
                goScoreList.CustomActive(true);
            }

            if(goAwardList != null)
            {
                goAwardList.CustomActive(false);
            }

            if(m_comScoreList != null)
            {
                m_comScoreList.Initialize();
                                
                m_comScoreList.onItemVisiable = item =>
                {
                    if (item.m_index >= 0)
                    {
                        OnUpdateScoreListItem(item.m_index, item.gameObject);
                    }
                };
            }

            if (m_comAwardList != null)
            {
                m_comAwardList.Initialize();
                m_comAwardList.onItemVisiable = item =>
                {
                    if (item.m_index >= 0 && item.m_index < m_arrAwardItems.Count && m_arrAwardItems != null)
                    {
                        //UpdateAwardListItem(item.m_index, item.gameObject, m_arrAwardItems[item.m_index]);
                    }
                };
            }

            for(int i = 0;i < m_arrAwardItems.Count;i++)
            {
                UpdateAwardListItem(i, null, m_arrAwardItems[i]);
            }

            GuildDataManager.GetInstance().SendWorldGuildDungeonDamageRankReq();

            RefreshRankList();
            BindUIEvent();
            
            UpdatePayRewardItems(0);
        }      

        protected override void _OnCloseFrame()
        {
            m_arrAwardItems = null;
            awardType2ItemDatas = null;
            UnBindUIEvent();

            ClearData();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonDamageRank, _OnUpdateRankList);        
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonDamageRank, _OnUpdateRankList);
        }

        void _OnUpdateRankList(UIEvent uiEvent)
        {

            RefreshRankList();
        }

        [UIEventHandle("Close")]
        void OnCloseBtnClicked()
        {
            frameMgr.CloseFrame(this);
        }

        void RefreshRankList()
        {
            if(goScoreList == null)
            {
                return;
            }
   
            List<GuildDataManager.DungeonDamageRankInfo> arrScoreList = GuildDataManager.GetInstance().GetDungeonRankInfoList();
            if (arrScoreList != null)
            {
                m_comScoreList.SetElementAmount(arrScoreList.Count);
            }

            GuildDataManager.DungeonDamageRankInfo item = GuildDataManager.GetInstance().GetMyDungeonDamageRankInfo();
            UpdateMyScoreListItem(item.nRank, goMyScoreRank, item);
        }

        void RrefeshRankAward()
        {
            if(goAwardList == null)
            {
                return;
            }

            m_comAwardList.SetElementAmount(m_arrAwardItems.Count);
        }

        void OnUpdateScoreListItem(int iIndex, GameObject a_objLine)
        {
            List<GuildDataManager.DungeonDamageRankInfo> arrScoreList = GuildDataManager.GetInstance().GetDungeonRankInfoList();
            if (iIndex >= arrScoreList.Count)
            {
                return;
            }

            GuildDataManager.DungeonDamageRankInfo a_data = arrScoreList[iIndex];
            UpdateScoreListItem(iIndex, a_objLine, a_data);

            return;
        }

        int GetMaxRank()
        {
            int iMAX = 0;

            Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWarRewardTable>();
            if (dicts == null)
            {
                Logger.LogErrorFormat("TableManager.instance.GetTable<ScoreWarRewardTable>() error!!!");
            }
            else
            {

                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    ScoreWarRewardTable adt = iter.Current.Value as ScoreWarRewardTable;
                    if (adt != null && adt.RankingBegin > 0 && adt.RankingEnd > 0)
                    {  
                        if(adt.RankingEnd >= iMAX)
                        {
                            iMAX = adt.RankingEnd;
                        }
                    }
                }
            }

            return iMAX;
        }

        List<AwardItemData> GetKillBossAwardList()
        {
            List<AwardItemData> awardList = new List<AwardItemData>();

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

                    if (adt.rewardType != 11)
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
                            awardList.Add(data);
                        }
                    }
                }
            }

            return awardList;
        }

        Vector2 GetAwardItemSize(GuildDungeonAwardsShowItem.AwardType awardType)
        {
            Vector2 vec = new Vector2();
            if(awardType2ItemDatas == null)
            {
                return vec;
            }
            if(awardType2ItemDatas.ContainsKey(awardType))
            {
                if(showItems != null)
                {
                    return showItems.GetContentSize(awardType2ItemDatas[awardType]);
                }
            }
            return vec;
        }

        // 击杀失败奖励数据
        List<AwardItemData> GetNotKillBossAwardList()
        {
            List<AwardItemData> awardList = new List<AwardItemData>();

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

                    if (adt.rewardType != 12)
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
                            awardList.Add(data);
                        }
                    }
                }
            }

            return awardList;
        }

        // 击杀三个使徒成功奖励数据
        List<AwardItemData> GetKill3ShiTuAwardList()
        {
            List<AwardItemData> awardList = new List<AwardItemData>();
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
                    if (adt.rewardType != 13)
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
                            awardList.Add(data);
                        }
                    }
                }
            }
            return awardList;
        }

        private void UpdatePayRewardItems(int selectPayActId)
        {
            //ClearPayRewardItems();
         
            if (awardItemDataList == null)
            {
                Logger.LogError("ItemdataList data is null");
            }
            if (mScrollUIList == null)
            {
                Logger.LogError("mScrollUIList obj is null");
                return;
            }

            //float mScorllWidth = 0f;
            //float mScrollPadding = 0f;
            //float mScorllItemWidth = 0f;
            //float mScrollItemSpace = 0f;

            if (mScrollUIList.IsInitialised() == false)
            {
                mScrollUIList.Initialize();
                mScrollUIList.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    //if(go.GetComponent<RectTransform>())
                    //{
                    //    mScorllItemWidth = go.GetComponent<RectTransform>().sizeDelta.x;
                    //}
                    return payItem;
                };
            }
            mScrollUIList.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }
                int iIndex = var1.m_index;
                if (iIndex >= 0 && iIndex < awardItemDataList.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataList[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", awardItemDataList[iIndex].ID);
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

            awardItemDataList = GetKillBossAwardList();

            //mScrollUIList.m_autoCenteredElements = true;
            mScrollUIList.SetElementAmount(awardItemDataList.Count);

            if (mScrollUIList2.IsInitialised() == false)
            {
                mScrollUIList2.Initialize();
                mScrollUIList2.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    //if(go.GetComponent<RectTransform>())
                    //{
                    //    mScorllItemWidth = go.GetComponent<RectTransform>().sizeDelta.x;
                    //}
                    return payItem;
                };
            }
            mScrollUIList2.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }
                int iIndex = var1.m_index;
                if (iIndex >= 0 && iIndex < awardItemDataList2.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataList2[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", awardItemDataList2[iIndex].ID);
                        return;
                    }
                    itemDetailData.Count = awardItemDataList2[iIndex].Num;
                    PayRewardItem payItem = var1.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(this, itemDetailData, true, false);
                        payItem.RefreshView();                      
                    }
                }
            };

            awardItemDataList2 = GetNotKillBossAwardList();

            //mScrollUIList.m_autoCenteredElements = true;
            mScrollUIList2.SetElementAmount(awardItemDataList2.Count);
            {
                if (mScrollUIList3.IsInitialised() == false)
                {
                    mScrollUIList3.Initialize();
                    mScrollUIList3.onBindItem = (GameObject go) =>
                    {
                        GuildDungeonAwardsShowItem payItem = null;
                        if (go)
                        {
                            payItem = go.GetComponent<GuildDungeonAwardsShowItem>();
                        }
                        return payItem;
                    };
                }
                mScrollUIList3.onItemVisiable = (var1) =>
                {
                    if (var1 == null)
                    {
                        return;
                    }
                    int iIndex = var1.m_index;
                    if (awardType2ItemDatas != null && iIndex >= 0 && iIndex < awardType2ItemDatas.Count)
                    {                        
                        GuildDungeonAwardsShowItem payItem = var1.gameObjectBindScript as GuildDungeonAwardsShowItem;
                        if (payItem != null)
                        {
                            GuildDungeonAwardsShowItem.AwardType awardType = (GuildDungeonAwardsShowItem.AwardType)iIndex;
                            if(awardType2ItemDatas.ContainsKey(awardType))
                            {
                                payItem.SetUp(this,awardType,awardType2ItemDatas[awardType]);
                            }
                        }
                    }
                };        
                if(awardType2ItemDatas != null)
                {
                    List<Vector2> vecs = new List<Vector2>();
                    vecs.Add(GetAwardItemSize(GuildDungeonAwardsShowItem.AwardType.Kill3ShiTu));  
                    vecs.Add(GetAwardItemSize(GuildDungeonAwardsShowItem.AwardType.NotKillBigBoss));                                 
                    mScrollUIList3.SetElementAmount(awardType2ItemDatas.Count,vecs);
                }
            }
        }

        private void ClearData(bool isClearWithTabs = true)
        {
            if (awardItemDataList != null)
            {
                awardItemDataList.Clear();
            }
            if (awardItemDataList2 != null)
            {
                awardItemDataList2.Clear();
            }
         


            if (mScrollUIList != null)
            {
                mScrollUIList.SetElementAmount(0);
            }
            if (mScrollUIList2 != null)
            {
                mScrollUIList2.SetElementAmount(0);
            }         
            if (mScrollUIList3 != null)
            {
                mScrollUIList3.SetElementAmount(0);
            }
        }

        void UpdateMyScoreListItem(uint iIndex, GameObject a_objLine, GuildDataManager.DungeonDamageRankInfo a_data)
        {
            if (a_objLine == null || a_data == null)
            {
                return;
            }

            GameObject objValid = Utility.FindGameObject(a_objLine, "Valid");

            if (objValid == null)
            {
                return;
            }

            ComCommonBind bind = objValid.GetComponent<ComCommonBind>();
            if(bind == null)
            {
                return;
            }
            Button btnSelect = bind.GetCom<Button>("BtnSelect");

            if (btnSelect == null)
            {
                return;
            }

            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(() =>
            {
                
            });

            StaticUtility.SafeSetText(bind,"Rank",a_data.nRank.ToString());
            StaticUtility.SafeSetText(bind, "Player",a_data.strPlayerName);
            StaticUtility.SafeSetText(bind, "Score",a_data.nDamage.ToString());       

            if(string.IsNullOrEmpty(a_data.strPlayerName))
            {
                StaticUtility.SafeSetText(bind, "Player",PlayerBaseData.GetInstance().Name);
            }           
        }

        void UpdateScoreListItem(int iIndex, GameObject a_objLine, GuildDataManager.DungeonDamageRankInfo a_data)
        {
            if (a_objLine == null || a_data == null)
            {
                return;
            }

            GameObject objValid = Utility.FindGameObject(a_objLine, "Valid"); 

            if (objValid == null)
            {
                return;
            }

            ComCommonBind bind = objValid.GetComponent<ComCommonBind>();
            if(bind == null)
            {
                return;
            }
            Button btnSelect = bind.GetCom<Button>("BtnSelect");

            if (btnSelect == null)
            {
                return;
            }

            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(() =>
            {

            });

            StaticUtility.SafeSetText(bind, "Rank", "");
            StaticUtility.SafeSetText(bind, "Player", "");
            StaticUtility.SafeSetText(bind, "Score", "");
            StaticUtility.SafeSetText(bind, "Server", "");
            StaticUtility.SafeSetVisible<Image>(bind, "BG", false);

            StaticUtility.SafeSetText(bind, "Rank(1)", "");
            StaticUtility.SafeSetText(bind, "Player(1)", "");
            StaticUtility.SafeSetText(bind, "Score(1)", "");
            StaticUtility.SafeSetText(bind, "Server(1)", "");
            StaticUtility.SafeSetVisible<Image>(bind, "BG(1)", false);
            

            if (iIndex % 2 == 0)
            {
                StaticUtility.SafeSetVisible<Image>(bind,"BG",true);
            }
            else
            {         
                StaticUtility.SafeSetVisible<Image>(bind,"BG(1)",true);
            }


            if (a_data.nPlayerID == PlayerBaseData.GetInstance().RoleID)
            {
                StaticUtility.SafeSetText(bind,"Rank", a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player",a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score",a_data.nDamage.ToString());       
            }
            else
            {
                StaticUtility.SafeSetText(bind, "Rank(1)", a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player(1)", a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score(1)", a_data.nDamage.ToString());        
            }
        }

        void UpdateAwardListItem(int iIndex, GameObject a_objLine, AwardRankItem a_data)
        {
            Text txtRank = mBind.GetCom<Text>(string.Format("txtRank{0}", iIndex));
            if(txtRank != null)
            {
                txtRank.text = string.Format("排名{0}-{1}奖励", a_data.iRankMin, a_data.iRankMax);
            }

            Button btnChest = mBind.GetCom<Button>(string.Format("btnChest{0}", iIndex));
            if(btnChest != null)
            {
                btnChest.onClick.RemoveAllListeners();
                btnChest.onClick.AddListener(() => 
                {
                    //AwardRankItem data = new AwardRankItem();
                    //data = a_data;

                    if(a_data.arrItems.Count > 0)
                    {
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(a_data.arrItems[0]);
                    }

                    //frameMgr.OpenFrame<AwardShowFrame>(FrameLayer.Middle,data);
                });
            }


        }
    }
}
