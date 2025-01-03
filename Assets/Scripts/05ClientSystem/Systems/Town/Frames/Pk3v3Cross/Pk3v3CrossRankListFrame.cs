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
    class AwardRankItem
    {
        public int iRankMin;
        public int iRankMax;
        public List<ItemData> arrItems = new List<ItemData>();
    }

    class Pk3v3CrossRankListFrame : ClientFrame
    {
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

        List<AwardRankItem> m_arrAwardItems = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossRankList";
        }       

        public static void OnOpenLinkFrame(string argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossRankListFrame>(FrameLayer.Middle);
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
                    RefreshScoreRank();
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

            RefreshScoreRank();
            BindUIEvent();

            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_SCORE_WAR);
            msg.start = (ushort)0;
            msg.num = (ushort)GetMaxRank();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

      

        protected override void _OnCloseFrame()
        {
            m_arrAwardItems = null;
            UnBindUIEvent();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdatePk3v3CrossRankScoreList, _OnUpdateScoreList);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdatePk3v3CrossRankScoreList, _OnUpdateScoreList);
        }

        void _OnUpdateScoreList(UIEvent uiEvent)
        {
            RefreshScoreRank();
        }

        [UIEventHandle("Close")]
        void OnCloseBtnClicked()
        {
            frameMgr.CloseFrame(this);
        }

        void RefreshScoreRank()
        {
            if(goScoreList == null)
            {
                return;
            }

            List<Pk3v3CrossDataManager.ScoreListItem> arrScoreList = Pk3v3CrossDataManager.GetInstance().GetScoreList();
            if(arrScoreList != null)
            {
                m_comScoreList.SetElementAmount(arrScoreList.Count);
            }

            Pk3v3CrossDataManager.ScoreListItem item = Pk3v3CrossDataManager.GetInstance().GetMyRankInfo();
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
            List<Pk3v3CrossDataManager.ScoreListItem> arrScoreList = Pk3v3CrossDataManager.GetInstance().GetScoreList();
            if (iIndex >= arrScoreList.Count)
            {
                return;
            }

            Pk3v3CrossDataManager.ScoreListItem a_data = arrScoreList[iIndex];
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

        void UpdateMyScoreListItem(int iIndex, GameObject a_objLine, Pk3v3CrossDataManager.ScoreListItem a_data)
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
            StaticUtility.SafeSetText(bind,"Player",a_data.strPlayerName);
            StaticUtility.SafeSetText(bind,"Score",a_data.nPlayerScore.ToString());
            StaticUtility.SafeSetText(bind,"Server",a_data.strServerName);

            // 未上榜的情况下去取玩家信息里面的score add by qxy 2018-10-08
            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (a_data.nRank == 0 || a_data.nRank >= GetMaxRank())
                {
                    StaticUtility.SafeSetText(bind, "Score",pkInfo.nScore.ToString());
                }
            }

            if (a_data.nRank == 0 || a_data.nRank > GetMaxRank())
            {
                StaticUtility.SafeSetText(bind, "Rank","未上榜");
            }

            if(string.IsNullOrEmpty(a_data.strPlayerName))
            {
                StaticUtility.SafeSetText(bind, "Player",PlayerBaseData.GetInstance().Name);
            }

            if(string.IsNullOrEmpty(a_data.strServerName))
            {
                StaticUtility.SafeSetText(bind, "Server",ClientApplication.adminServer.name);
            }
        }

        void UpdateScoreListItem(int iIndex, GameObject a_objLine, Pk3v3CrossDataManager.ScoreListItem a_data)
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

            StaticUtility.SafeSetText(bind,"Rank","");
            StaticUtility.SafeSetText(bind, "Player","");
            StaticUtility.SafeSetText(bind, "Score","");
            StaticUtility.SafeSetText(bind, "Server","");
            StaticUtility.SafeSetVisible<Image>(bind, "BG",false);

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
     
            
            if(a_data.nPlayerID == PlayerBaseData.GetInstance().RoleID)
            {
                StaticUtility.SafeSetText(bind,"Rank",a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player",a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score",a_data.nPlayerScore.ToString());
                StaticUtility.SafeSetText(bind, "Server",a_data.strServerName);
            }
            else
            {
                StaticUtility.SafeSetText(bind, "Rank(1)", a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player(1)", a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score(1)", a_data.nPlayerScore.ToString());
                StaticUtility.SafeSetText(bind, "Server(1)", a_data.strServerName);               
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
