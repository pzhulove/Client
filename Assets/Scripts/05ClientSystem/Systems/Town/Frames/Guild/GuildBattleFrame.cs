using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Network;
using Protocol;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{
    class GuildBattleFrame : ClientFrame
    {
        class RewardInfo
        {
            public enum EState
            {
                /// <summary>
                /// 已领取
                /// </summary>
                Got,

                /// <summary>
                /// 可以领取
                /// </summary>
                CanGet,

                /// <summary>
                /// 不可领取
                /// </summary>
                CanotGet,
            }

            public bool bDirty;
            public int nID;
            public EState state;
            public int nLimitScore;
            public List<ItemData> arrItems;
            public GameObject objCanGetTip;
            public GameObject objCanGetEffect;
            public GameObject objAlreadyGot;
            public ComButtonEx comBtnExFunc;
            public UIGray comGray;
            public DOTweenAnimation dotween;
            public Image imgBox;
            public Text labLimitScore;
        }

        [UIControl("Reward/Progress")]
        Slider m_sliProgress;

        [UIControl("Reward/Preview/Title")]
        Text m_labRewardPreviewTitle;

        [UIObject("Reward/Preview/Items")]
        GameObject m_objRewardPreviewRoot;

        [UIObject("Reward/Preview/Items/Template")]
        GameObject m_objRewardPreviewTemplate;

        [UIObject("Reward/Preview")]
        GameObject m_objRewardPreview;

        [UIControl("StartBattle/RemainFreeTime")]
        Text m_labRemainFreeTime;

        [UIObject("StartBattle/Cost")]
        GameObject m_objBattleCostRoot;

        [UIObject("StartBattle/Cost/Item")]
        GameObject m_objBattleCostTemplate;

        [UIControl("SimplifyBattleInfo/middle/SelfRecord/Text")]
        Text m_labSelfRecord;

        [UIControl("SimplifyBattleInfo/middle/SelfRank/Text")]
        Text m_labSelfRank;

        [UIControl("SimplifyBattleInfo/middle/GuildRank/Text")]
        Text m_labGuildRank;

        [UIControl("SimplifyBattleInfo/middle/SelfScore/Text")]
        Text m_labSelfScore;

        [UIControl("title")]
        Text m_labTitle;

        [UIControl("StartBattle/Start/Text")]
        Text m_labStart;

        [UIControl("Time/Text")]
        Text m_labBattleTime;

        [UIControl("Time")]
        Image m_imgBattleTimeBG;

        [UIControl("SimplifyBattleInfo")]
        DOTweenAnimation m_simplifyDotween;

        [UIObject("talk")]
        GameObject m_objTalkRoot;

        [UIControl("GuildRank/Content/First/Name")]
        Text m_labGuildRankFirstName;

        [UIControl("GuildRank/Content/Second/Name")]
        Text m_labGuildRankSecondName;

        [UIControl("GuildRank/Content/Third/Name")]
        Text m_labGuildRankThirdName;

        [UIControl("GuildRank/Content/First/Score")]
        Text m_labGuildRankFirstScore;

        [UIControl("GuildRank/Content/Second/Score")]
        Text m_labGuildRankSecondScore;

        [UIControl("GuildRank/Content/Third/Score")]
        Text m_labGuildRankThirdScore;

        [UIControl("GuildRank")]
        DOTweenAnimation m_guildRankDotween;

        List<RewardInfo> m_arrRewardInfos = new List<RewardInfo>();
        List<ComItem> m_arrIdleComItems = new List<ComItem>();
        List<ComItem> m_arrUseComItems = new List<ComItem>();
        List<GameObject> m_arrBattleCostObjs = new List<GameObject>();
        string[] m_arrOpenedBoxIcon =
        {
            "UI/Image/Icon/Icon_Item/TubiaoXiangzi_MuzhiGuan.png:TubiaoXiangzi_MuzhiGuan",
            "UI/Image/Icon/Icon_Item/TubiaoXiangzi_MuzhiGuan.png:TubiaoXiangzi_MuzhiGuan",
            "UI/Image/Icon/Icon_Item/TubiaoXiangzi_QingtongGuan.png:TubiaoXiangzi_QingtongGuan",
            "UI/Image/Icon/Icon_Item/TubiaoXiangzi_QingtongGuan.png:TubiaoXiangzi_QingtongGuan",
            "UI/Image/Icon/Icon_Item/TubiaoXiangzi_ZhizunGuan.png:TubiaoXiangzi_ZhizunGuan"
        };

        bool m_bMatching = false;
        DelayCallUnitHandle m_repeatCall;
        DelayCallUnitHandle m_repeatCallBattleTime;
        DelayCallUnitHandle m_repeatCallSyncGuildRank;
        bool m_bSimplifyDotweenPlayForward = false;
        bool m_bGuildRankDotweenPlayForward = false;
        ComTalk mComTalk = null;

        public override void Init()
        {
            //GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneJumpFinished, _OnSceneChangedFinish);
            //GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKMatched, _OnPKMatched);
        }

        public override void Clear()
        {
            //GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneJumpFinished, _OnSceneChangedFinish);
            //GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKMatched, _OnPKMatched);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattle";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();

            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam != null)
            {
                TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
            }
            
            ClientSystemTownFrame townFrame = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
            if (townFrame != null)
            {
                townFrame.SetForbidFadeIn(true);
            }
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OutPkWaitingScene);
            if (mComTalk != null)
            {
                ComTalk.Recycle();
                mComTalk = null;
            }
            
            _ClearUI();
            _UnRegisterUIEvent();

            ClientSystemTownFrame townFrame = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
            if (townFrame != null)
            {
                townFrame.SetForbidFadeIn(false);
            }
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleRewardGetSuccess, _OnRewardGetSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnGuildBattleStartMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnGuildBattleStopMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleRecordSync, _OnBattleRecordSync);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildSelfRankChanged, _OnSelfRankChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleRanklistChanged, _OnGuildBattleRanklistChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInspireSuccess, _OnInspireSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnPlayerDataGuildUpdated);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleRewardGetSuccess, _OnRewardGetSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnGuildBattleStartMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnGuildBattleStopMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleRecordSync, _OnBattleRecordSync);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildSelfRankChanged, _OnSelfRankChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleRanklistChanged, _OnGuildBattleRanklistChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildInspireSuccess, _OnInspireSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnPlayerDataGuildUpdated);
        }

        void _InitUI()
        {
            m_objRewardPreviewTemplate.SetActive(false);
            m_objRewardPreviewTemplate.transform.SetParent(frame.transform, false);
            m_objBattleCostTemplate.SetActive(false);
            m_objBattleCostTemplate.transform.SetParent(frame.transform, false);

            mGuild.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() != GuildBattleType.GBT_CROSS);
            mBtMenu.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() != GuildBattleType.GBT_CROSS);

            mComTalk = ComTalk.Create(m_objTalkRoot);
           
            _InitRewardInfo();
            _UpdateReward();
            _HideRewardPreview();
            _UpdateBattleStart(false);
            _UpdateFreeTime();
            _UpdateTitle();
            _UpdateGuildRankList(null);

            _UpdateLeftShowInfo();

            m_repeatCall = ClientSystemManager.GetInstance().delayCaller.RepeatCall(10000, () =>
            {
                GuildDataManager.GetInstance().RequestSelfRanklist();
            }, 9999999, true);

            m_repeatCallBattleTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
            {
                _UpdateBattleTime();
            }, 9999999, true);

            m_repeatCallSyncGuildRank = ClientSystemManager.GetInstance().delayCaller.RepeatCall(10000, () =>
            {
                GuildDataManager.GetInstance().RequestRanklist(SortListType.SORTLIST_GUILD_BATTLE_SCORE, 0, 3);
            }, 9999999, true);
        }

        void _ClearUI()
        {
            m_arrRewardInfos.Clear();
            m_arrIdleComItems.Clear();
            m_arrUseComItems.Clear();
            m_arrBattleCostObjs.Clear();

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCall);

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallBattleTime);

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallSyncGuildRank);

            _UpdateBattleStart(false);

            m_bSimplifyDotweenPlayForward = false;
            m_bGuildRankDotweenPlayForward = false;
        }

        void _InitRewardInfo()
        {
            m_arrRewardInfos.Clear();
            for (int i = 1; i <= 5; ++i)
            {
                RewardInfo info = new RewardInfo();
                info.nID = i;
                info.objCanGetTip = Utility.FindGameObject(frame, string.Format("Reward/Box{0}/ClickGet", i));
                info.objCanGetEffect = Utility.FindGameObject(frame, string.Format("Reward/Box{0}/CanGetEffect", i));
                info.objAlreadyGot = Utility.FindGameObject(frame, string.Format("Reward/Box{0}/AlreadyGot", i));
                info.comBtnExFunc = Utility.GetComponetInChild<ComButtonEx>(frame, string.Format("Reward/Box{0}/Icon", i));
                info.comGray = Utility.GetComponetInChild<UIGray>(frame, string.Format("Reward/Box{0}/Icon", i));
                info.dotween = Utility.GetComponetInChild<DOTweenAnimation>(frame, string.Format("Reward/Box{0}/Icon", i));
                info.labLimitScore = Utility.GetComponetInChild<Text>(frame, string.Format("Reward/Box{0}/Score", i));
                info.imgBox = Utility.GetComponetInChild<Image>(frame, string.Format("Reward/Box{0}/Icon", i));

                ProtoTable.GuildRewardTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.GuildRewardTable>(i);
                info.nLimitScore = tableData.TargetScore;
                info.arrItems = new List<ItemData>();
                for (int j = 0; j < tableData.ItemReward.Count; ++j)
                {
                    string[] values = tableData.ItemReward[j].Split('_');
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                    itemData.Count = int.Parse(values[1]);
                    info.arrItems.Add(itemData);
                }

                if (info.nLimitScore <= PlayerBaseData.GetInstance().guildBattleScore)
                {
                    if (PlayerBaseData.GetInstance().guildBattleRewardMask.CheckMask((uint)info.nID))
                    {
                        info.state = RewardInfo.EState.Got;
                    }
                    else
                    {
                        info.state = RewardInfo.EState.CanGet;
                    }

                }
                else
                {
                    info.state = RewardInfo.EState.CanotGet;
                }
                info.bDirty = true;
                m_arrRewardInfos.Add(info);
            }
        }

        void _UpdateLeftShowInfo()
        {
            EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

            if (state >= EGuildBattleState.Firing && state <= EGuildBattleState.LuckyDraw)
            {
                mBattleInfo.CustomActive(true);
                mInspireInfo.CustomActive(false);

                _UpdateSimplifyBattleInfo();
            }
            else
            {
                mBattleInfo.CustomActive(false);
                mInspireInfo.CustomActive(true);

                _UpdateInspireInfo();
            }
        }

        void _UpdateInspireInfo()
        {
            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireCostIcon, ref mInspireCost, ref mEnableInspire, GuildBattleOpenType.GBOT_GUILD_SCENE_MAIN);
        }

        void _UpdateReward()
        {
            for (int i = 0; i < m_arrRewardInfos.Count; ++i)
            {
                RewardInfo info = m_arrRewardInfos[i];
                if (info.bDirty)
                {
                    info.bDirty = false;

                    info.comBtnExFunc.onClick.RemoveAllListeners();
                    info.comBtnExFunc.onMouseDown.RemoveAllListeners();
                    info.comBtnExFunc.onMouseUp.RemoveAllListeners();
                    info.objCanGetTip.SetActive(false);
                    info.objCanGetEffect.SetActive(false);
                    info.objAlreadyGot.SetActive(false);
                    info.comGray.Restore();
                    info.dotween.DORewind();
                    info.labLimitScore.text = info.nLimitScore.ToString();

                    if (info.state == RewardInfo.EState.CanGet)
                    {
                        info.objCanGetTip.SetActive(true);
                        info.objCanGetEffect.SetActive(true);
                        info.dotween.DORestart();
                        info.comBtnExFunc.onClick.AddListener(() =>
                        {
                            GuildDataManager.GetInstance().RequestGetBattleReward(info.nID);
                        });
                    }
                    else if (info.state == RewardInfo.EState.Got)
                    {
                        //info.imgBox.sprite = AssetLoader.GetInstance().LoadRes(m_arrOpenedBoxIcon[info.nID-1], typeof(Sprite)).obj as Sprite;
                        info.comGray.SetGray();
                        info.objAlreadyGot.SetActive(true);
                    }
                    else
                    {
                        info.comBtnExFunc.onMouseDown.AddListener((var) => { _ShowRewardPreview(info); });
                        info.comBtnExFunc.onMouseUp.AddListener((var) => { _HideRewardPreview(); });
                        
                    }
                }
            }

            float fProgress = 0.0f;
            for (int i = 0; i < m_arrRewardInfos.Count; ++i)
            {
                RewardInfo info = m_arrRewardInfos[i];
                if (info.state == RewardInfo.EState.CanotGet)
                {
                    int nPreScore = 0;
                    if (i > 0)
                    {
                        nPreScore = m_arrRewardInfos[i - 1].nLimitScore;
                    }
                    fProgress += (PlayerBaseData.GetInstance().guildBattleScore - nPreScore) / (float)(info.nLimitScore - nPreScore);
                    break;
                }
                else if (info.state == RewardInfo.EState.Got || info.state == RewardInfo.EState.CanGet)
                {
                    fProgress += 1.0f;
                }
            }
            m_sliProgress.value = fProgress / m_arrRewardInfos.Count;
        }

        void _RefreshGray()
        {
            for (int i = 0; i < m_arrRewardInfos.Count; ++i)
            {
                RewardInfo info = m_arrRewardInfos[i];
                UIGray.Refresh(info.comGray);
            }
        }

        void _ShowRewardPreview(RewardInfo a_info)
        {
            m_objRewardPreview.SetActive(true);
            m_labRewardPreviewTitle.text = TR.Value("guild_battle_reward_preview_title", a_info.nLimitScore);
            for (int i = 0; i < a_info.arrItems.Count; ++i)
            {
                ComItem comItem;
                if (m_arrIdleComItems.Count > 0)
                {
                    comItem = m_arrIdleComItems[0];
                    comItem.transform.parent.gameObject.SetActive(true);
                    m_arrIdleComItems.RemoveAt(0);
                }
                else
                {
                    GameObject obj = GameObject.Instantiate(m_objRewardPreviewTemplate);
                    obj.transform.SetParent(m_objRewardPreviewRoot.transform, false);
                    obj.SetActive(true);
                    comItem = CreateComItem(obj);
                }

                comItem.Setup(a_info.arrItems[i], (GameObject a_obj, ItemData a_item) =>
                {
                    ItemTipManager.GetInstance().ShowTip(a_item);
                });
                m_arrUseComItems.Add(comItem);
            }
        }

        void _HideRewardPreview()
        {
            m_objRewardPreview.SetActive(false);

            for (int i = 0; i < m_arrUseComItems.Count; ++i)
            {
                m_arrUseComItems[i].transform.parent.gameObject.SetActive(false);
            }
            m_arrIdleComItems.AddRange(m_arrUseComItems);
            m_arrUseComItems.Clear();
        }

        void _UpdateBattleStart(bool a_bMatching)
        {
            bool bEnable = GuildDataManager.GetInstance().HasTargetManor() &&
                GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Firing;

            if (bEnable && a_bMatching)
            {
                m_bMatching = true;
                m_labStart.text = "取消决斗";
                ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle);
            }
            else
            {
                m_bMatching = false;
                m_labStart.text = "开始决斗";
                ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();
            }
        }

        void _UpdateFreeTime()
        {
            GuildTerritoryTable tableData = null;

            if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(GuildDataManager.GetInstance().myGuild.nTargetCrossManorID);
            }
            else
            {
                tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(GuildDataManager.GetInstance().myGuild.nTargetManorID);
            }
            
            if (tableData == null)
            {
                m_labRemainFreeTime.gameObject.SetActive(false);
                m_objBattleCostRoot.SetActive(false);
                return;
            }

            int nCurrentTimes = PlayerBaseData.GetInstance().guildBattleTimes;
            if (nCurrentTimes < 0)
            {
                Logger.LogErrorFormat("公会战次数非法，当前次数{0}", nCurrentTimes);
            }
            else
            {
                int nIndex = nCurrentTimes;
                if (nIndex >= tableData.MatchIConsume.Count)
                {
                    nIndex = tableData.MatchIConsume.Count - 1;
                }

                string strTemp = tableData.MatchIConsume[nIndex];
                if (string.IsNullOrEmpty(strTemp) == false)
                {
                    m_labRemainFreeTime.gameObject.SetActive(false);
                    m_objBattleCostRoot.SetActive(true);
                    for (int i = 0; i < m_arrBattleCostObjs.Count; ++i)
                    {
                        m_arrBattleCostObjs[i].SetActive(false);
                    }

                    string[] data = strTemp.Split(',');
                    for (int j = 0; j < data.Length; ++j)
                    {
                        string[] values = data[j].Split('_');
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(int.Parse(values[0]));

                        GameObject objCost;
                        if (j >= m_arrBattleCostObjs.Count)
                        {
                            objCost = GameObject.Instantiate(m_objBattleCostTemplate);
                            objCost.transform.SetParent(m_objBattleCostRoot.transform, false);
                        }
                        else
                        {
                            objCost = m_arrBattleCostObjs[j];
                        }
                        objCost.SetActive(true);

                        int nNeedCount = int.Parse(values[1]);
                        if (ItemDataManager.GetInstance().GetOwnedItemCount((int)itemData.TableID) < nNeedCount)
                        {
                            Utility.GetComponetInChild<Text>(objCost, "Count").text = TR.Value("color_red", values[1]);
                        }
                        else
                        {
                            Utility.GetComponetInChild<Text>(objCost, "Count").text = values[1];
                        }
                        //Utility.GetComponetInChild<Image>(objCost, "Icon").sprite =
                        //    AssetLoader.GetInstance().LoadRes(itemData.Icon, typeof(Sprite)).obj as Sprite;
                        Image iconImage = Utility.GetComponetInChild<Image>(objCost, "Icon");
                        ETCImageLoader.LoadSprite(ref iconImage, itemData.Icon);
                    }
                }
                else
                {
                    int nRemainFreeTimes = 0;
                    for (int i = nIndex; i < tableData.MatchIConsume.Count; ++i)
                    {
                        if (string.IsNullOrEmpty(tableData.MatchIConsume[i]))
                        {
                            nRemainFreeTimes++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    m_labRemainFreeTime.gameObject.SetActive(true);
                    m_labRemainFreeTime.text = TR.Value("guild_battle_remain_free_time", nRemainFreeTimes, nRemainFreeTimes + nIndex);
                    m_objBattleCostRoot.SetActive(false);
                }

            }

            _RefreshGray();
        }

        void _UpdateSimplifyBattleInfo()
        {
            //GuildDataManager.GetInstance().RequestBattleRecord
            m_labSelfRecord.text = TR.Value("guild_battle_simply_record",
                GuildDataManager.GetInstance().GetGuildBattleWinTimes(), 
                GuildDataManager.GetInstance().GetGuildBattleFailTimes());
            m_labSelfScore.text = PlayerBaseData.GetInstance().guildBattleScore.ToString();
        }

        void _UpdateBattleTime()
        {
            if (GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Firing)
            {
                uint nTimeLeft = GuildDataManager.GetInstance().GetGuildBattleStateEndTime() - TimeManager.GetInstance().GetServerTime();

                if (nTimeLeft > 0)
                {
                    uint second = 0;
                    uint minute = 0;

                    second = nTimeLeft % 60;
                    minute = nTimeLeft / 60;
                    m_labBattleTime.text = TR.Value("guild_battle_remain_time", minute, second);

                    m_imgBattleTimeBG.gameObject.SetActive(true);

                    if (nTimeLeft > 10 * 60)
                    {
                        //m_imgBattleTimeBG.sprite = AssetLoader.GetInstance().LoadRes(
                        //    "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Lvse_Tiao_Di", typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref m_imgBattleTimeBG, "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Lvse_Tiao_Di");
                    }
                    else
                    {
                        //m_imgBattleTimeBG.sprite = AssetLoader.GetInstance().LoadRes(
                        //    "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Hongse_Tiao_Di", typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref m_imgBattleTimeBG, "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Hongse_Tiao_Di");
                    }
                }
                else
                {
                    m_imgBattleTimeBG.gameObject.SetActive(false);
                }
            }
            else
            {
                uint nTimeLeft = GuildDataManager.GetInstance().GetGuildBattleStateEndTime() - TimeManager.GetInstance().GetServerTime();

                if (nTimeLeft > 0 && nTimeLeft < 3600 && GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Preparing)
                {
                    uint second = 0;
                    uint minute = 0;

                    second = nTimeLeft % 60;
                    minute = nTimeLeft / 60;
                    m_labBattleTime.text = TR.Value("guild_battle_start_time", minute, second);

                    m_imgBattleTimeBG.gameObject.SetActive(true);
                    //m_imgBattleTimeBG = AssetLoader.GetInstance().LoadRes(
                    //        "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Huangse_Tiao_Di", typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref m_imgBattleTimeBG, "UIFlatten/Image/Packed/pck_UI_Guild.png:UI_Gonghui_Huangse_Tiao_Di");
                }
                else
                {
                    m_imgBattleTimeBG.gameObject.SetActive(false);
                }
            }

            _RefreshGray();
        }

        void _UpdateTitle()
        {
            int nManorID = GuildDataManager.GetInstance().myGuild.nTargetManorID;

            if(GuildDataManager.GetInstance().GetGuildBattleType()  == GuildBattleType.GBT_CROSS)
            {
                nManorID = GuildDataManager.GetInstance().myGuild.nTargetCrossManorID;
            }

            GuildTerritoryTable table = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(nManorID);
            if (table != null)
            {
                if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
                {
                    m_labTitle.text = TR.Value("guild_battle_title", table.Name);
                }
                else if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                {
                    m_labTitle.text = TR.Value("guild_battle_title", table.Name);
                }
                else
                {
                    m_labTitle.text = TR.Value("guild_AttackCity_title", table.Name);
                }         
            }
            else
            {
                m_labTitle.text = TR.Value("guild_battle_title_default");
            } 
        }

        void _UpdateGuildRankList(BaseSortList a_sortlist)
        {
            if (a_sortlist == null)
            {
                m_labGuildRankFirstName.text = TR.Value("guild_battle_guild_rank_sync");
                m_labGuildRankSecondName.text = TR.Value("guild_battle_guild_rank_sync");
                m_labGuildRankThirdName.text = TR.Value("guild_battle_guild_rank_sync");
                m_labGuildRankFirstScore.text = string.Empty;
                m_labGuildRankSecondScore.text = string.Empty;
                m_labGuildRankThirdScore.text = string.Empty;
            }
            else
            {
                if ((SortListType)a_sortlist.type.mainType == SortListType.SORTLIST_GUILD_BATTLE_SCORE)
                {
                    m_labGuildRankFirstName.text = TR.Value("guild_battle_guild_rank_none");
                    m_labGuildRankSecondName.text = TR.Value("guild_battle_guild_rank_none");
                    m_labGuildRankThirdName.text = TR.Value("guild_battle_guild_rank_none");

                    m_labGuildRankFirstScore.text = string.Empty;
                    m_labGuildRankSecondScore.text = string.Empty;
                    m_labGuildRankThirdScore.text = string.Empty;

                    if (a_sortlist.entries != null)
                    {
                        if (a_sortlist.entries.Count > 0)
                        {
                            GuildBattleScore data = a_sortlist.entries[0] as GuildBattleScore;
                            m_labGuildRankFirstName.text = data.name;
                            m_labGuildRankFirstScore.text = TR.Value("guild_battle_guild_rank_format", data.score);
                        }

                        if (a_sortlist.entries.Count > 1)
                        {
                            GuildBattleScore data = a_sortlist.entries[1] as GuildBattleScore;
                            m_labGuildRankSecondName.text = data.name;
                            m_labGuildRankSecondScore.text = TR.Value("guild_battle_guild_rank_format", data.score);
                        }

                        if (a_sortlist.entries.Count > 2)
                        {
                            GuildBattleScore data = a_sortlist.entries[2] as GuildBattleScore;
                            m_labGuildRankThirdName.text = data.name;
                            m_labGuildRankThirdScore.text = TR.Value("guild_battle_guild_rank_format", data.score);
                        }
                    }
                }
            }
        }

        void _OnRewardGetSuccess(UIEvent a_event)
        {
            int nID = (byte)a_event.Param1;
            for (int i = 0; i < m_arrRewardInfos.Count; ++i)
            {
                if (m_arrRewardInfos[i].nID == nID)
                {
                    m_arrRewardInfos[i].state = RewardInfo.EState.Got;
                    m_arrRewardInfos[i].bDirty = true;
                    _UpdateReward();
                    break;
                }
            }
        }

        void _OnGuildBattleStateUpdated(UIEvent a_event)
        {
            _UpdateBattleStart(m_bMatching);
            _UpdateLeftShowInfo();

            EGuildBattleState oldState = (EGuildBattleState)(a_event.Param1);
            EGuildBattleState newState = (EGuildBattleState)(a_event.Param2);
            if (oldState == EGuildBattleState.Firing && newState == EGuildBattleState.Invalid)
            {
                if (m_bMatching)
                {
                    GuildDataManager.GetInstance().CancelStartBattle();
                }
                ClientSystemManager.GetInstance().OpenFrame<GuildBattleResultFrame>(FrameLayer.Middle, a_event.Param3);
            }
        }

        void _OnGuildBattleStartMatch(UIEvent a_event)
        {
            _UpdateBattleStart(true);
        }

        void _OnGuildBattleStopMatch(UIEvent a_event)
        {
            _UpdateBattleStart(false);
        }

        void _OnBattleRecordSync(UIEvent a_event)
        {
            _UpdateLeftShowInfo();
        }

        void _OnSelfRankChanged(UIEvent a_event)
        {
            m_labSelfRank.text = a_event.Param1.ToString();
            m_labGuildRank.text = a_event.Param2.ToString();
        }

        void _OnGuildBattleRanklistChanged(UIEvent a_event)
        {
            _UpdateGuildRankList(a_event.Param1 as BaseSortList);
        }

        void _OnInspireSuccess(UIEvent a_event)
        {
            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireCostIcon, ref mInspireCost, ref mEnableInspire, GuildBattleOpenType.GBOT_GUILD_SCENE_MAIN);
        }

        void _OnPlayerDataGuildUpdated(UIEvent a_event)
        {
            SceneObjectAttr prop = (SceneObjectAttr)a_event.Param1;

            if (prop == SceneObjectAttr.SOA_GUILD_BATTLE_NUMBER)
            {
                _UpdateFreeTime();
            }
        }

        void _OnSceneChangedFinish(UIEvent a_event)
        {
//             int nSceneID = (int)(a_event.Param1);
//             ProtoTable.CitySceneTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.CitySceneTable>(nSceneID);
//             if (tableData != null && 
//                 tableData.SceneType == ProtoTable.CitySceneTable.eSceneType.PK_PREPARE && 
//                 tableData.SceneSubType == ProtoTable.CitySceneTable.eSceneSubType.GuildBattle)
//             {
//                 // TODO 整理界面打开机制，现在的界面打开太臃肿了
//                 Open(ClientSystemManager.GetInstance().GetLayer(FrameLayer.Middle));
//             }
        }

        void _OnPKMatched(UIEvent a_event)
        {
//             if ((byte)a_event.Param1 == (byte)RaceType.GuildBattle)
//             {
//                 Close();
//             }
        }

        [UIEventHandle("btClose")]
        void _OnCloseClicked()
        {
            if (m_bMatching)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_battle_cannot_operate"));
            }
            else
            {
                ClientSystemManager.GetInstance().CloseFrame(this);
                ClientSystemTown town = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                if (town != null)
                {
                    town.ChangeScene(0, 0, 0, 0, () =>
                    {
                        if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                        {
                            ClientSystemManager.GetInstance().OpenFrame<GuildMainFrame>(FrameLayer.Middle, EOpenGuildMainFramData.OpenGuildCrossManor);
                        }
                        else
                        {
                            ClientSystemManager.GetInstance().OpenFrame<GuildMainFrame>(FrameLayer.Middle, EOpenGuildMainFramData.OpenManor);
                        }    
                    });
                }
            }
        }

        [UIEventHandle("SimplifyBattleInfo/middle/RankDetail")]
        void _OnRankDetailClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildBattleRanklistFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("SimplifyBattleInfo/middle/BattleRecord")]
        void _OnBattleRecordClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildBattleRecordFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("StartBattle/Start")]
        void _OnStartBattleClicked()
        {
            bool bEnable = GuildDataManager.GetInstance().HasTargetManor() && GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Firing;
            if (!bEnable)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_battle_cannot_start"));
                return;
            }

            if (m_bMatching == false)
            {
                int nCurrentTimes = PlayerBaseData.GetInstance().guildBattleTimes;
                if (nCurrentTimes < 0)
                {
                    Logger.LogErrorFormat("公会战次数非法，当前次数{0}", nCurrentTimes);
                }
                else
                {
                    GuildTerritoryTable tableData = null;

                    if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                    {
                        tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(GuildDataManager.GetInstance().myGuild.nTargetCrossManorID);
                    }
                    else
                    {
                        tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(GuildDataManager.GetInstance().myGuild.nTargetManorID);
                    }

                    if(tableData == null)
                    {
                        Logger.LogErrorFormat("GuildTerritoryTable tableData is null in [_OnStartBattleClicked], GuildBattleType == {0}", GuildDataManager.GetInstance().GetGuildBattleType());
                        return;
                    }

                    int nIndex = nCurrentTimes;
                    if (nIndex >= tableData.MatchIConsume.Count)
                    {
                        nIndex = tableData.MatchIConsume.Count - 1;
                    }

                    string strTemp = tableData.MatchIConsume[nIndex];
                    if (string.IsNullOrEmpty(strTemp) == false)
                    {
                        string[] data = strTemp.Split(',');
                        List<CostItemManager.CostInfo> costInfos = new List<CostItemManager.CostInfo>();
                        for (int j = 0; j < data.Length; ++j)
                        {
                            string[] values = data[j].Split('_');
                            CostItemManager.CostInfo info = new CostItemManager.CostInfo();
                            info.nMoneyID = int.Parse(values[0]);
                            info.nCount = int.Parse(values[1]);
                            costInfos.Add(info);
                        }
                        bool isJumpTip = false;
                        if(GuildDataManager.GetInstance().GetGuildBattleType()== GuildBattleType.GBT_NORMAL)
                        {
                            isJumpTip = GuildDataManager.GetInstance().IsJumpTipWhenStartGuildBattle;
                        }else if(GuildDataManager.GetInstance().GetGuildBattleType()==GuildBattleType.GBT_CROSS)
                        {
                            isJumpTip = GuildDataManager.GetInstance().IsJumpTipWhenStartGuildBattleCorssServer;
                        }
                        if (isJumpTip)
                        {
                            CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, () =>
                            {
                                GuildDataManager.GetInstance().StartBattle();
                            });
                        }
                        else
                        {
                            CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                            comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("guild_battle_consume_ask", CostItemManager.GetInstance().GetCostMoneiesDesc(costInfos.ToArray()));
                            comconMsgBoxOkCancelParamData.IsShowNotify = true;
                            comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = onToggleClick;
                            comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("GuildBattle_OK_Cancle_Lable");
                            comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("GuildBattle_OK_Buy_Lable");
                            comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = () =>
                            {
                                CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, () =>
                                {
                                    GuildDataManager.GetInstance().StartBattle();
                                });
                            };

                            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(comconMsgBoxOkCancelParamData);
                        }


                        //SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("guild_battle_consume_ask", CostItemManager.GetInstance().GetCostMoneiesDesc(costInfos.ToArray())), () =>
                        //{
                        //    CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, () =>
                        //    {
                        //        GuildDataManager.GetInstance().StartBattle();
                        //    });
                        //});
                    }
                    else
                    {
                        GuildDataManager.GetInstance().StartBattle();
                    }
                }
            }
            else
            {
                GuildDataManager.GetInstance().CancelStartBattle();
            }
        }

        private void onToggleClick(bool value)
        {
            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
            {
                GuildDataManager.GetInstance().IsJumpTipWhenStartGuildBattle=value;
            }
            else if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                GuildDataManager.GetInstance().IsJumpTipWhenStartGuildBattleCorssServer=value;
            }
        }

        [UIEventHandle("SimplifyBattleInfo/btOpen")]
        void _OnOpenCloseSimplifyClicked()
        {
            if (m_bSimplifyDotweenPlayForward == false)
            {
                m_simplifyDotween.DOPlayForward();
                m_bSimplifyDotweenPlayForward = true;
            }
            else
            {
                m_simplifyDotween.DOPlayBackwards();
                m_bSimplifyDotweenPlayForward = false;
            }
        }

        [UIEventHandle("btMenu")]
        void _OnMenuClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("GuildRank/btOpen")]
        void _OnOpenCloseGuildRankClicked()
        {
            if (m_bGuildRankDotweenPlayForward == false)
            {
                m_guildRankDotween.DOPlayForward();
                m_bGuildRankDotweenPlayForward = true;
            }
            else
            {
                m_guildRankDotween.DOPlayBackwards();
                m_bGuildRankDotweenPlayForward = false;
            }
        }

        [UIEventHandle("guild")]
        void _OnReturnToGuildClicked()
        {
            frameMgr.OpenFrame<GuildMainFrame>(FrameLayer.Middle);
        }

        #region ExtraUIBind
        private GameObject mBattleInfo = null;
        private GameObject mInspireInfo = null;
        private Text mInspireLevel = null;
        private Button mInspireDetail = null;
        private Text mCurAttr = null;
        private Button mInspire = null;
        private Image mInspireCostIcon = null;
        private Text mInspireCost = null;
        private GameObject mInspireMax = null;
        private ComButtonEnbale mEnableInspire = null;
        private GameObject mGuild = null;
        private GameObject mBtMenu = null;

        protected override void _bindExUI()
        {
            mBattleInfo = mBind.GetGameObject("BattleInfo");
            mInspireInfo = mBind.GetGameObject("InspireInfo");
            mInspireLevel = mBind.GetCom<Text>("InspireLevel");
            mInspireDetail = mBind.GetCom<Button>("InspireDetail");
            mInspireDetail.onClick.AddListener(_onInspireDetailButtonClick);
            mCurAttr = mBind.GetCom<Text>("CurAttr");
            mInspire = mBind.GetCom<Button>("Inspire");
            mInspire.onClick.AddListener(_onInspireButtonClick);
            mInspireCostIcon = mBind.GetCom<Image>("InspireCostIcon");
            mInspireCost = mBind.GetCom<Text>("InspireCost");
            mInspireMax = mBind.GetGameObject("InspireMax");
            mEnableInspire = mBind.GetCom<ComButtonEnbale>("EnableInspire");
            mGuild = mBind.GetGameObject("guild");
            mBtMenu = mBind.GetGameObject("btMenu");
        }

        protected override void _unbindExUI()
        {
            mBattleInfo = null;
            mInspireInfo = null;
            mInspireLevel = null;
            mInspireDetail.onClick.RemoveListener(_onInspireDetailButtonClick);
            mInspireDetail = null;
            mCurAttr = null;
            mInspire.onClick.RemoveListener(_onInspireButtonClick);
            mInspire = null;
            mInspireCostIcon = null;
            mInspireCost = null;
            mInspireMax = null;
            mEnableInspire = null;
            mGuild = null;
            mBtMenu = null;
        }
        #endregion

        #region Callback
        private void _onInspireDetailButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildInspireDetailFrame>(FrameLayer.Middle);
        }

        private void _onInspireButtonClick()
        {
            GuildDataManager.GetInstance().SendInspire();
        }
        #endregion
    }
}
