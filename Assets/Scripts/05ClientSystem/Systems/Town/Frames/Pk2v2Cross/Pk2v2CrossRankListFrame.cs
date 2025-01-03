using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // Pk2v2CrossRankList
    public class Pk2v2CrossRankListFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        List<AwardItemData> awardItemDatas = new List<AwardItemData>();
        const int maxRank = 100;
        #endregion

        #region ui bind
        ComUIListScript rankList = null;
        Text testTxt = null;
        Button testBtn = null;
        Image testImg = null;
        Slider testSlider = null;
        Toggle testToggle = null;
        GameObject goMyScoreRank = null;
        ComUIListScript rankAwardList = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk2v2Cross/Pk2v2CrossRankList";
        }

        protected override void OnOpenFrame()
        {
            InitRankList();
            UpdateRankList();

            InitRankAwardList();
            UpdateRankAwardList();

            BindUIEvent();

            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_2V2_SCORE_WAR);
            msg.start = (ushort)0;
            msg.num = (ushort)GetMaxRank();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        protected override void OnCloseFrame()
        { 
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            rankList = mBind.GetCom<ComUIListScript>("rankList");
            rankAwardList = mBind.GetCom<ComUIListScript>("rankAwardList");

            testTxt = mBind.GetCom<Text>("testTxt");

            testBtn = mBind.GetCom<Button>("testBtn");
            testBtn.SafeSetOnClickListener(() => 
            {

            });

            testImg = mBind.GetCom<Image>("testImg");

            testSlider = mBind.GetCom<Slider>("testSlider");
            testSlider.SafeSetValueChangeListener((value) => 
            {

            });

            testToggle = mBind.GetCom<Toggle>("testToggle");
            testToggle.SafeSetOnValueChangedListener((value) => 
            {

            });

            goMyScoreRank = mBind.GetGameObject("goMyScoreRank");            
        }

        protected override void _unbindExUI()
        {
            rankList = null;
            rankAwardList = null;

            testTxt = null;
            testBtn = null;
            testImg = null;
            testSlider = null;
            testToggle = null;
            goMyScoreRank = null;
        }

        public override bool IsNeedUpdate()
        {
            return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {

        }

        #endregion

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdatePk2v2CrossRankScoreList, _OnUpdateScoreList);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdatePk2v2CrossRankScoreList, _OnUpdateScoreList);
        }

        int GetMaxRank()
        {
            return maxRank;
        }

        void RefreshScoreRank()
        {
            List<Pk2v2CrossDataManager.ScoreListItem> arrScoreList = Pk2v2CrossDataManager.GetInstance().GetScoreList();
            if (arrScoreList != null)
            {
                rankList.SetElementAmount(arrScoreList.Count);
            }

            Pk2v2CrossDataManager.ScoreListItem item = Pk2v2CrossDataManager.GetInstance().GetMyRankInfo();
            UpdateMyScoreListItem(item.nRank, goMyScoreRank, item);
        }

        void OnUpdateScoreListItem(int iIndex, GameObject a_objLine)
        {
            List<Pk2v2CrossDataManager.ScoreListItem> arrScoreList = Pk2v2CrossDataManager.GetInstance().GetScoreList();
            if (iIndex >= arrScoreList.Count)
            {
                return;
            }

            Pk2v2CrossDataManager.ScoreListItem a_data = arrScoreList[iIndex];
            UpdateScoreListItem(iIndex, a_objLine, a_data);

            return;
        }

        void UpdateScoreListItem(int iIndex, GameObject a_objLine, Pk2v2CrossDataManager.ScoreListItem a_data)
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
            if (bind == null)
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
                StaticUtility.SafeSetVisible<Image>(bind, "BG", true);
            }
            else
            {
                StaticUtility.SafeSetVisible<Image>(bind, "BG(1)", true);
            }


            if (a_data.nPlayerID == PlayerBaseData.GetInstance().RoleID)
            {
                StaticUtility.SafeSetText(bind, "Rank", a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player", a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score", a_data.nPlayerScore.ToString());
                StaticUtility.SafeSetText(bind, "Server", a_data.strServerName);
            }
            else
            {
                StaticUtility.SafeSetText(bind, "Rank(1)", a_data.nRank.ToString());
                StaticUtility.SafeSetText(bind, "Player(1)", a_data.strPlayerName);
                StaticUtility.SafeSetText(bind, "Score(1)", a_data.nPlayerScore.ToString());
                StaticUtility.SafeSetText(bind, "Server(1)", a_data.strServerName);
            }
        }

        void UpdateMyScoreListItem(int iIndex, GameObject a_objLine, Pk2v2CrossDataManager.ScoreListItem a_data)
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
            if (bind == null)
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

            StaticUtility.SafeSetText(bind, "Rank", a_data.nRank.ToString());
            StaticUtility.SafeSetText(bind, "Player", a_data.strPlayerName);
            StaticUtility.SafeSetText(bind, "Score", a_data.nPlayerScore.ToString());
            StaticUtility.SafeSetText(bind, "Server", a_data.strServerName);

            // 未上榜的情况下去取玩家信息里面的score add by qxy 
            Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (a_data.nRank == 0 || a_data.nRank >= GetMaxRank())
                {
                    StaticUtility.SafeSetText(bind, "Score", pkInfo.nScore.ToString());
                }
            }

            if (a_data.nRank == 0 || a_data.nRank > GetMaxRank())
            {
                StaticUtility.SafeSetText(bind, "Rank", TR.Value("2v2melee_score_war_no_rank"));
            }

            if (string.IsNullOrEmpty(a_data.strPlayerName))
            {
                StaticUtility.SafeSetText(bind, "Player", PlayerBaseData.GetInstance().Name);
            }

            if (string.IsNullOrEmpty(a_data.strServerName))
            {
                StaticUtility.SafeSetText(bind, "Server", ClientApplication.adminServer.name);
            }
        }

        void InitRankList()
        {
            if(rankList == null)
            {
                return;
            }

            rankList.Initialize();
            rankList.onBindItem = (go) => 
            {
                return go;
            };    

            rankList.onItemVisiable = item =>
            {
                if (item.m_index >= 0)
                {
                    OnUpdateScoreListItem(item.m_index, item.gameObject);
                }
            };
        }       

        void UpdateRankList()
        {
            RefreshScoreRank();       
        }

        void InitRankAwardList()
        {
            if (rankAwardList == null)
            {
                return;
            }

            rankAwardList.Initialize();
            rankAwardList.onBindItem = (go) =>
            {
                return go;
            };

            rankAwardList.onItemVisiable = item =>
            {
                if (awardItemDatas != null && item.m_index < awardItemDatas.Count)
                {
                    UpdateRankAwardListItem(item,awardItemDatas[item.m_index]);
                }
            };
        }

        void CalcRankAwardItemDatas()
        {
            awardItemDatas = new List<AwardItemData>();
            if(awardItemDatas == null)
            {
                return;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWar2v2RewardTable>();
            if (dicts == null)
            {
                Logger.LogErrorFormat("TableManager.instance.GetTable<ScoreWarRewardTable>() error!!!");
            }
            else
            {

                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    ScoreWar2v2RewardTable adt = iter.Current.Value as ScoreWar2v2RewardTable;
                    if (adt != null && adt.RankingBegin == 1 && adt.RankingEnd == 10)
                    {
                        for (int i = 0; i < adt.ItemReward.Count; i++)
                        {
                            string strReward = adt.ItemRewardArray(i);
                            string[] reward = strReward.Split('_');
                            if (reward.Length >= 2)
                            {
                                int id = int.Parse(reward[0]);
                                int iCount = int.Parse(reward[1]);

                                awardItemDatas.Add(new AwardItemData()
                                {
                                    ID = id,
                                    Num = iCount,
                                });
                            }
                        }

                        break;
                    }
                }
            }    
        }

        void UpdateRankAwardListItem(ComUIListElementScript item, AwardItemData awardItemData)
        {
            if(item == null)
            {
                return;
            }

            if(awardItemData == null)
            {
                return;
            }

            ComCommonBind bind = item.GetComponent<ComCommonBind>();
            if(bind == null)
            {
                return;
            }

            ComItem comItem = bind.GetCom<ComItem>("Item");
            if(comItem == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(awardItemData.ID);
            if(itemData == null)
            {
                return;
            }
            itemData.Count = awardItemData.Num;

            comItem.Setup(itemData, (go,data) => 
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(data);
            });

            return;
        }

        void UpdateRankAwardList()
        {
            if(rankAwardList == null)
            {
                return;
            }

            CalcRankAwardItemDatas();
            if(awardItemDatas == null)
            {
                return;
            }

            rankAwardList.SetElementAmount(awardItemDatas.Count);
        }

        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {           
            return;
        }

        void _OnUpdateScoreList(UIEvent uiEvent)
        {
            RefreshScoreRank();
        }

        #endregion
    }
}
