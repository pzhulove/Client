using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public enum PkRoomType
    {
        TraditionPk,
        BudoPk,
        Pk3v3,
        Pk3v3Cross,
        Pk2v2Cross,
    }

    public class PkWaitingRoomData
    {
        public CitySceneTable.eSceneSubType SceneSubType = CitySceneTable.eSceneSubType.NULL;
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;

        public void Clear()
        {
            SceneSubType = CitySceneTable.eSceneSubType.NULL;
            CurrentSceneID = 0;
            TargetTownSceneID = 0;
        }
    }

    public class PkWaitingRoom : ClientFrame
    {
        [UIObject("PKRankRoot/Promotion")]
        GameObject m_objPromotionRoot;

        [UIObject("PKRankRoot/Promotion/Records/Viewport/Content")]
        GameObject m_objRecordRoot;

        [UIObject("PKRankRoot/Promotion/Records/Viewport/Content/Template")]
        GameObject m_objRecordTemplate;

        [UIControl("PKRankRoot/Promotion/Desc")]
        Text m_labPromotionDesc;

        [UIControl("PKRankRoot/CoinDescRoot/CoinDesc")]
        Text m_labCoinDesc;

        [UIObject("btPkVideo")]
        GameObject objPkVideo;

        [UIControl("btSeasonAttr/Time")]
        Text m_labSeasonAttrTime;

        [UIControl("", typeof(StateController))]
        StateController comState;

        string fingerPath = "UIFlatten/Prefabs/Pk/DualFinger";
        string EffectPath = "btBegin/GuideEffect";

        PkWaitingRoomData RoomData = new PkWaitingRoomData();

        ComTalk m_miniTalk = null;

        public float fTimeIntrval = 0.0f;
        bool bIsAddGuide = false;

        float selfPkCoin = 0;
        int currLevel = 0;
        int currJob = 0;
        int duelTuiJianEquipMaxLevel = 0;
        public float fTime = 0.0f;
        ShopItemTable shopItemTable;

        public static bool bBeginSeekPlayer = false;

        DelayCallUnitHandle m_repeatCallAttrTime;

        List<ShopItemTable> getDuleWeaponList = new List<ShopItemTable>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkWaitingRoom";
        }

        void _UpdateStatus()
        {
            if (null != comState)
            {
                if (MoneyRewardsDataManager.GetInstance().needEnterance)
                {
                    comState.Key = "moneyrewards";
                }
                else if (GuildDataManager.GetInstance().IsInGuildBattle())
                {
                    comState.Key = "guildfighter";
                }
                else if (BudoManager.GetInstance().IsOpen)
                {
                    comState.Key = "budofighter";
                }
                else
                {
                    comState.Key = "normal";
                }
            }
        }

        protected override void _OnOpenFrame()
        {
            RoomData = userData as PkWaitingRoomData;
            TableManager.instance.LoadData();
            currLevel = PlayerBaseData.GetInstance().Level;
            //currJob = PlayerBaseData.GetInstance().JobTableID / 10 * 10;
            currJob = Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID);


            ShopDataManager.GetInstance().InitBaseWeaponData(currJob);
            InitInterface();
            BindUIEvent();

            _UpdateStatus();
            TryOpenSkilltreeFrame();
            var DespoitData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DUELREDSHOW);
            duelTuiJianEquipMaxLevel = DespoitData.Value;

            _QueryShopData(5, () =>
            {
                _RefreshWeaponShow();
            }
           );


            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam != null)
            {
                TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);
            }

            if (ReplayServer.GetInstance().IsLastPlaying())
            {
                ReplayServer.GetInstance().SetLastPlaying(false);
                if (ReplayServer.GetInstance().replaySource == ReplayPlayFrom.VIDEO_FRAME)
                    ClientSystemManager.GetInstance().OpenFrame<PkVideoFrame>();
            }

            if (/*!GameClient.SwitchFunctionUtility.IsOpen(15)*/false)
            {
                if (objPkVideo != null)
                    objPkVideo.CustomActive(false);
            }
            //ShowInstituteIcon(null);
            //公平竞技场的活动是否开启
            mFairDuel.CustomActive(FairDuelDataManager.GetInstance().IsShowFairDuelEnterBtn());
            mCancelGo.CustomActive(false);
            {
                UIEvent uiEvent = new UIEvent();
                uiEvent.Param1 = (byte)Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
                Update2V2PointRaceBtn(uiEvent);
            }
            {
                UIEvent uiEvent = new UIEvent();
                uiEvent.Param1 = (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                Update3V3PointRaceBtn(uiEvent);
            }
        }

        //private void ShowInstituteIcon(UIEvent uiEvent)
        //{
        //    JobTable data = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
        //    institute.CustomActive(data.JobType == 1 && TableManager.instance.comboDataList.Count > 0);
        //}

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();

            ClientSystemManager.GetInstance().CloseFrame<Pk3v3RoomListFrame>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.DailyProve);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OutPkWaitingScene);
        }

        void ClearData()
        {
            RoomData.Clear();
            if (m_miniTalk != null)
            {
                ComTalk.Recycle();
                m_miniTalk = null;
            }
            

            if (mCommonPKRank != null)
            {
                mCommonPKRank.Clear();
                mCommonPKRank = null;
            }

            if (getDuleWeaponList != null)
            {
                getDuleWeaponList.Clear();
            }

            bIsAddGuide = false;
            fTimeIntrval = 0.0f;
            fTime = 0.0f;
            duelTuiJianEquipMaxLevel = 0;
            bBeginSeekPlayer = false;

            //RemoveGuideEffect();

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallAttrTime);
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchStartSuccess, OnPkMatchStartSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelSuccess, OnPkMatchCancelSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.pkGuideStart, OnPkGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.pkGuideEnd, OnPkGuideEnd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKRankChanged, _OnPkRankChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnSeasonDataUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MissionUpdated, _OnMissionUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkCoinChanged, _OnPkCoinUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _OnSelfLevelUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _OnShopBuyGoodsSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _GuildBattleStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, ShowInstituteIcon);
            BudoManager.GetInstance().onBudoInfoChanged += _OnBudoInfoChanged;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateFairDuelEntryState, _OnUpdateFairDuelEntryState);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK2V2CrossButton, Update2V2PointRaceBtn);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3CrossButton, Update3V3PointRaceBtn);

            //counter值改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChangeChanged);

        }

        protected void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, ShowInstituteIcon);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchStartSuccess, OnPkMatchStartSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelSuccess, OnPkMatchCancelSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.pkGuideStart, OnPkGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.pkGuideEnd, OnPkGuideEnd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKRankChanged, _OnPkRankChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnSeasonDataUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MissionUpdated, _OnMissionUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkCoinChanged, _OnPkCoinUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _OnSelfLevelUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _OnShopBuyGoodsSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, _GuildBattleStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            BudoManager.GetInstance().onBudoInfoChanged -= _OnBudoInfoChanged;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateFairDuelEntryState, _OnUpdateFairDuelEntryState);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK2V2CrossButton, Update2V2PointRaceBtn);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3CrossButton, Update3V3PointRaceBtn);


            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChangeChanged);

        }

        [UIEventHandle("btClose")]
        void OnClose()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town!!!");
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = RoomData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = RoomData.TargetTownSceneID,
                targetDoorID = 0,
            }, true));

            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("btMenu")]
        void OnMenu()
        {
            //             if (bBeginSeekPlayer)
            //             {
            //                 SystemNotifyManager.SystemNotify(4004);
            //                 return;
            //             }

            ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("btPkfriends")]
        void OnPkFriends()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PkFriendsFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PkFriendsFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<PkFriendsFrame>(FrameLayer.Middle, RequestType.Request_Challenge_PK);
        }



        [UIEventHandle("btFreeTrain")]
        void OnFreeTrain()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<InstituteFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("ranklist")]
        void OnRankList()
        {
            //             if (bBeginSeekPlayer)
            //             {
            //                 SystemNotifyManager.SystemNotify(4004);
            //                 return;
            //             }

            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Ranklist))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足,功能尚未解锁");
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<RanklistFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RanklistFrame>();
            }

            frameMgr.OpenFrame<RanklistFrame>(FrameLayer.Middle, SortListType.SORTLIST_1V1_SEASON);
        }

        [UIEventHandle("duelshop")]
        void OnDuelShop()
        {
            //             if (bBeginSeekPlayer)
            //             {
            //                 SystemNotifyManager.SystemNotify(4004);
            //                 return;
            //             }

            //frameMgr.OpenFrame<ShopMainFrame>(FrameLayer.Middle, new ShopMainFrameData { iShopFrameID = 1, iShopID = 5 });
            //单独打开决斗商店
            ShopNewDataManager.GetInstance().OpenShopNewFrame(5);
        }

        [UIEventHandle("topRight/btGotoButo")]
        void OnGotoButo()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if (!Utility.EnterBudo())
            {
                return;
            }

            //BudoManager.GetInstance().TryBeginActive();

            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("topRight/btGotoMoneyRewards")]
        void OnGotoMoneyRewards()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            MoneyRewardsEnterFrame.CommandOpen(null);
        }

        [UIEventHandle("btBegin")]
        void OnBegin()
        {
            if (!bBeginSeekPlayer)
            {
                SendMatchStartReq();
            }
            else
            {
                SendMatchCancelReq();
            }

            btBegin.interactable = false;

            GameStatisticManager.GetInstance().DoStatPk(StatPKType.MATCHING);
        }

        [UIEventHandle("btPkVideo")]
        void _OnPKVideoClicked()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<PkVideoFrame>();
        }

        void InitInterface()
        {
            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Ranklist))
            {
                ranklist.gameObject.SetActive(true);
            }
#if APPLE_STORE
            //add by mjx for ios appstore 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
            {
                ranklist.gameObject.CustomActive(false);
            }
#endif
            _InitTalk();
            InitRedPoint();
            _InitSeasonLevel();

            GameStatisticManager.GetInstance().DoStatPk(StatPKType.ENTER);

            UpdateHonorExpValueLabel();
        }

        void _InitSeasonLevel()
        {
            if (mCommonPKRank != null)
                mCommonPKRank.Initialize(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonExp);

            PromotionInfo promotionInfo = SeasonDataManager.GetInstance().GetPromotionInfo(SeasonDataManager.GetInstance().seasonLevel);
            if (promotionInfo.eState == EPromotionState.Promoting)
            {
                m_objPromotionRoot.SetActive(true);

                _SetChildrenEnable(m_objRecordRoot, false);
                for (int i = 0; i < promotionInfo.nTotalCount; ++i)
                {
                    GameObject objRecord = null;
                    if (i < m_objRecordRoot.transform.childCount)
                    {
                        objRecord = m_objRecordRoot.transform.GetChild(i).gameObject;
                    }
                    else
                    {
                        objRecord = GameObject.Instantiate(m_objRecordTemplate);
                        objRecord.transform.SetParent(m_objRecordRoot.transform, false);
                    }
                    objRecord.SetActive(true);
                    _SetChildrenEnable(objRecord, false);
                }

                List<byte> records = SeasonDataManager.GetInstance().seasonUplevelRecords;
                if (records != null && records.Count <= m_objRecordRoot.transform.childCount)
                {
                    for (int i = 0; i < records.Count; ++i)
                    {
                        GameObject objRecord = m_objRecordRoot.transform.GetChild(i).gameObject;
                        if (records[i] == (byte)PKResult.WIN)
                        {
                            Utility.FindGameObject(objRecord, "Win").SetActive(true);
                        }
                        else
                        {
                            Utility.FindGameObject(objRecord, "Lose").SetActive(true);
                        }
                    }
                }

                m_labPromotionDesc.text = TR.Value("pk_rank_detail_promotion_rule",
                    promotionInfo.nTotalCount, promotionInfo.nTargetWinCount, SeasonDataManager.GetInstance().GetRankName(promotionInfo.nNextSeasonLevel));
            }
            else
            {
                m_objPromotionRoot.SetActive(false);
            }

            m_labCoinDesc.text = TR.Value(
                "pk_rank_detail_coin_info",
                CountDataManager.GetInstance().GetCount("day_pkcoin"),
                _GetDailyMaxPKCoin(),
                PlayerBaseData.GetInstance().VipLevel
                );

            m_repeatCallAttrTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
            {
                if (m_labSeasonAttrTime != null)
                {
                    m_labSeasonAttrTime.text = _GetTimeLeft(SeasonDataManager.GetInstance().seasonAttrEndTime);
                }
            }, 9999999, true);

        }

        void _UpdateSeasonLevel()
        {
            if (mCommonPKRank != null)
            {
                mCommonPKRank.Initialize(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonExp);

                if(mCommonPKRank.commonPKRankBase != null)
                {
                    mCommonPKRank.commonPKRankBase.Initialize(SeasonDataManager.GetInstance().seasonLevel);
                }
            }


            PromotionInfo promotionInfo = SeasonDataManager.GetInstance().GetPromotionInfo(SeasonDataManager.GetInstance().seasonLevel);
            if (promotionInfo.eState == EPromotionState.Promoting)
            {
                m_objPromotionRoot.SetActive(true);

                _SetChildrenEnable(m_objRecordRoot, false);
                for (int i = 0; i < promotionInfo.nTotalCount; ++i)
                {
                    GameObject objRecord = null;
                    if (i < m_objRecordRoot.transform.childCount)
                    {
                        objRecord = m_objRecordRoot.transform.GetChild(i).gameObject;
                    }
                    else
                    {
                        objRecord = GameObject.Instantiate(m_objRecordTemplate);
                        objRecord.transform.SetParent(m_objRecordRoot.transform, false);
                    }
                    objRecord.SetActive(true);
                    _SetChildrenEnable(objRecord, false);
                }

                List<byte> records = SeasonDataManager.GetInstance().seasonUplevelRecords;
                if (records != null && records.Count <= m_objRecordRoot.transform.childCount)
                {
                    for (int i = 0; i < records.Count; ++i)
                    {
                        GameObject objRecord = m_objRecordRoot.transform.GetChild(i).gameObject;
                        if (records[i] == (byte)PKResult.WIN)
                        {
                            Utility.FindGameObject(objRecord, "Win").SetActive(true);
                        }
                        else
                        {
                            Utility.FindGameObject(objRecord, "Lose").SetActive(true);
                        }
                    }
                }

                m_labPromotionDesc.text = TR.Value("pk_rank_detail_promotion_rule",
                    promotionInfo.nTotalCount, promotionInfo.nTargetWinCount, SeasonDataManager.GetInstance().GetRankName(promotionInfo.nNextSeasonLevel));
            }
            else
            {
                m_objPromotionRoot.SetActive(false);
            }
        }

        string _GetTimeLeft(int a_nEndTime)
        {
            int nTimeLeft = a_nEndTime - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft > 0)
            {
                int second = 0;
                int minute = 0;
                int hour = 0;
                int day = 0;
                second = nTimeLeft % 60;
                int temp = nTimeLeft / 60;
                if (temp > 0)
                {
                    minute = temp % 60;
                    temp = temp / 60;
                    if (temp > 0)
                    {
                        hour = temp % 24;
                        day = temp / 24;
                    }
                }

                string value = "";
                if (day > 0)
                {
                    value += string.Format("{0}天", day);
                    value += string.Format("{0:D2}小时", hour);
                    return value;
                }

                if (hour > 0)
                {
                    value += string.Format("{0:D2}小时", hour);
                    value += string.Format("{0:D2}分", minute);
                    return value;
                }


                if (minute > 0)
                {
                    value += string.Format("{0:D2}分", minute);
                    value += string.Format("{0:D2}秒", second);
                    return value;
                }


                return string.Format("{0:D2}秒", second);
            }
            else
            {
                return string.Empty;
            }
        }

        void _SetChildrenEnable(GameObject a_obj, bool a_bEnable)
        {
            for (int i = 0; i < a_obj.transform.childCount; ++i)
            {
                a_obj.transform.GetChild(i).gameObject.SetActive(a_bEnable);
            }
        }

        int _GetDailyMaxPKCoin()
        {
            int nValue = 0;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PKCOIN_BASENUM);
            if (SystemValueTableData != null)
            {
                nValue = SystemValueTableData.Value;
            }

            float fMaxCoinData = Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.PK_MONEY_LIMIT);
            nValue += (int)fMaxCoinData;

            return nValue;
        }

        void _InitTalk()
        {
            m_miniTalk = ComTalk.Create(TalkRoot);
        }

        void InitRedPoint()
        {           
            DailyProveRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
        }

        void OnRedPointChanged(UIEvent iEvent)
        {
            ERedPoint redpointType = (ERedPoint)iEvent.Param1;
         
            if (redpointType == ERedPoint.DailyProve)
            {
                DailyProveRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
            }        
        }

        void OnPkMatchStartSuccess(UIEvent iEvent)
        {
            PkSeekWaitingData data = new PkSeekWaitingData();
            data.roomtype = PkRoomType.TraditionPk;

            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, data);

            bBeginSeekPlayer = true;
            mCancelGo.CustomActive(true);
            btBegin.interactable = true;
        }

        void OnPkMatchCancelSuccess(UIEvent iEvent)
        {
            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();

            bBeginSeekPlayer = false;
            mCancelGo.CustomActive(false);
            btBegin.interactable = true;
        }

        void OnPkGuideStart(UIEvent iEvent)
        {
            ResetGuide();
        }

        void OnPkGuideEnd(UIEvent iEvent)
        {
            CloseGuide();
        }

        void _OnPkRankChanged(UIEvent a_event)
        {
            _UpdateSeasonLevel();
        }

        void _OnSeasonDataUpdate(UIEvent a_event)
        {
            _UpdateSeasonLevel();
        }

        void _OnMissionUpdated(UIEvent a_event)
        {

            DailyProveRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
        }

        void _OnPkCoinUpdated(UIEvent a_event)
        {
            if (mItemRoot == null)
            {
                return;
            }

            selfPkCoin = PlayerBaseData.GetInstance().uiPkCoin;

            if (mItemRoot.activeInHierarchy)
            {
                mCostSlider.value = selfPkCoin / shopItemTable.CostNum;
                mCostText.text = selfPkCoin + "/" + shopItemTable.CostNum;

                var goGoumai = Utility.FindChild(mItemRoot, "EffUI_kegoumai");

                if (goGoumai == null)
                {
                    return;
                }

                if (mCostSlider.value == 1)
                {
                    goGoumai.CustomActive(true);
                }
                else
                {
                    goGoumai.CustomActive(false);
                }
            }
        }

        void _RefreshWeaponShow()
        {
            currLevel = PlayerBaseData.GetInstance().Level;

            int iBuyTopLevel = 0;

            var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(5);
            if (null != shopData)
            {
                var allDuelWeapons = ShopDataManager.GetInstance().GetDuelWeaponsDict().GetEnumerator();

                while (allDuelWeapons.MoveNext())
                {
                    var shopItemTableList = allDuelWeapons.Current.Value as List<ShopItemTable>;

                    if (shopItemTableList == null)
                    {
                        continue;
                    }
                    int count = 0;
                    for (int i = 0; i < shopItemTableList.Count; i++)
                    {
                        // 判断有没有买过
                        for (int j = 0; j < shopData.Goods.Count; j++)
                        {
                            if (shopItemTableList[i].ID == shopData.Goods[j].ID && shopData.Goods[j].LimitBuyTimes != 0)
                            {
                                count++;
                                break;
                            }
                        }
                    }
                    if (count < shopItemTableList.Count)
                    {
                        if (allDuelWeapons.Current.Key > iBuyTopLevel)
                        {
                            iBuyTopLevel = allDuelWeapons.Current.Key;
                        }

                    }

                }
            }

            iBuyTopLevel += 6;
            if (iBuyTopLevel > duelTuiJianEquipMaxLevel)
            {
                mGoDuelWeapon.CustomActive(false);
                return;
            }

            if (iBuyTopLevel > currLevel)
            {
                getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(iBuyTopLevel);
                CreatDuleWeaponItem(getDuleWeaponList, 0);
            }
            else
            {
                getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(currLevel);
                CreatDuleWeaponItem(getDuleWeaponList, 0);
            }
        }

        void _OnSelfLevelUpdated(UIEvent a_event)
        {

            _RefreshWeaponShow();

        }
        void _OnShopBuyGoodsSuccess(UIEvent a_event)
        {
            ShopDataManager.GetInstance().InitBaseWeaponData(currJob);
            _QueryShopData(5, () =>
            {
                ShowDuelWeapon();
            }
          );
        }

        void _GuildBattleStateChanged(UIEvent a_event)
        {
            _UpdateStatus();
        }

        void _OnMoneyRewardsStatusChanged(UIEvent a_event)
        {
            _UpdateStatus();
        }

        void _OnBudoInfoChanged()
        {
            _UpdateStatus();
        }

        void SendMatchStartReq()
        {
            WorldMatchStartReq req = new WorldMatchStartReq();
            req.type = (byte)PkType.Pk_Season_1v1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //WaitNetMessageManager.GetInstance().Wait<WorldMatchStartRes>(msgRet =>
            //{
            //});
        }

        [ProtocolHandle(typeof(WorldMatchStartRes))]
        private void _onMathcStartRes(WorldMatchStartRes msgRet)
        {
            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
        }

        void SendMatchCancelReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //WaitNetMessageManager.GetInstance().Wait<WorldMatchCancelRes>(msgRet =>
            //{
            //   
            //});
        }

        [ProtocolHandle(typeof(WorldMatchCancelRes))]
        private void _onMathcCancelRes(WorldMatchCancelRes msgRet)
        {
            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);
        }

        void AddGuideEffect()
        {
            GameObject obj = AssetLoader.instance.LoadResAsGameObject(fingerPath);
            if (obj == null)
            {
                return;
            }
            obj.name = "GuideEffect";

            Utility.AttachTo(obj, btBegin.gameObject);
        }

        void RemoveGuideEffect()
        {
            GameObject effect = Utility.FindGameObject(frame, EffectPath, false);
            if (effect != null)
            {
                GameObject.Destroy(effect);
                effect = null;
            }
        }

        void ResetGuide()
        {
            fTimeIntrval = 0.0f;
            bIsAddGuide = false;
        }

        void CloseGuide()
        {
            //RemoveGuideEffect();
            bIsAddGuide = true;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }
        int index = 1;
        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeIntrval += timeElapsed;

            if (fTimeIntrval >= 12.0f)
            {
                if (!bIsAddGuide && RoomData.SceneSubType == CitySceneTable.eSceneSubType.TRADITION && !NewbieGuideManager.GetInstance().IsGuiding())
                {
                    //AddGuideEffect();
                    bIsAddGuide = true;
                }
            }

            if (mItemRoot.activeInHierarchy)
            {
                fTime += timeElapsed;

                int count = 0;
                if (fTime >= 30.0f)
                {

                    {
                        var DuelWeapon = Utility.FindChild(mGoDuelWeapon, "EffUI_DuelWeapon");
                        DuelWeapon.CustomActive(true);
                        count = getDuleWeaponList.Count;
                        CreatDuleWeaponItem(getDuleWeaponList, count - (count - index));
                        index++;
                        if (index == count)
                        {
                            index = 0;
                        }

                        StartCoroutine(WaitHide(1.0f));

                        fTime = 0;
                    }

                }
            }

        }

        IEnumerator WaitHide(float waitTime)
        {


            yield return Yielders.GetWaitForSeconds(waitTime);

            var DuelWeapon = Utility.FindChild(mGoDuelWeapon, "EffUI_DuelWeapon");
            //等待之后执行的动作  
            DuelWeapon.CustomActive(false);


        }


        [UIControl("Detail/RedPoint")]
        Image DailyProveRedPoint;

        [UIControl("btBegin")]
        Button btBegin;

        [UIObject("talk")]
        GameObject TalkRoot;  

        [UIControl("ranklist")]
        Button ranklist;

        [UIEventHandle("Detail")]
        void _OnPKRankDetailClicked()
        {
            frameMgr.OpenFrame<PKSeasonDetailFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("btSeasonAttr")]
        void _OnPKSeasonAttrClicked()
        {
            frameMgr.OpenFrame<PKSeasonDetailFrame>(FrameLayer.Middle, ESeasonDetailType.Attr);
        }

        void _QueryShopData(int iShopID, UnityEngine.Events.UnityAction callback)
        {
            var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(iShopID);
            if (null != shopItem)
            {
                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(shopItem.ID);
                if (null != shopData)
                {
                    if (null != callback)
                    {
                        callback.Invoke();
                    }
                    return;
                }
                else
                {
                    ShopDataManager.GetInstance().OpenShop(shopItem.ID,
                        0, -1,
                        () =>
                        {
                            if (null != callback)
                            {
                                callback.Invoke();
                            }
                        },
                        null,
                        ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME,
                        0);
                }
            }
        }

        void ShowDuelWeapon()
        {
            var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(5);
            if (null != shopData)
            {
                var allDuelWeapons = ShopDataManager.GetInstance().GetDuelWeaponsDict().GetEnumerator();

                bool bHasBuy = false;
                int iBuyTopLevel = 0;


                while (allDuelWeapons.MoveNext())
                {
                    var shopItemTableList = allDuelWeapons.Current.Value as List<ShopItemTable>;

                    if (shopItemTableList == null)
                    {
                        continue;
                    }
                    int count = 0;
                    for (int i = 0; i < shopItemTableList.Count; i++)
                    {
                        // 判断有没有买过
                        for (int j = 0; j < shopData.Goods.Count; j++)
                        {
                            if (shopItemTableList[i].ID == shopData.Goods[j].ID && shopData.Goods[j].LimitBuyTimes != 0)
                            {
                                count++;
                                break;
                            }
                        }
                    }
                    //如果count小于每个等级的总数量，说明已经买过，把买过最高等级装备的等级记录下来iBuyTopLevel
                    if (count < shopItemTableList.Count)
                    {
                        bHasBuy = true;

                        if (allDuelWeapons.Current.Key > iBuyTopLevel)
                        {
                            iBuyTopLevel = allDuelWeapons.Current.Key;
                        }

                    }

                }
                //iBuyTopLevel + 10显示下个等级的装备
                iBuyTopLevel += 10;
                if (bHasBuy)
                {
                    if (iBuyTopLevel > duelTuiJianEquipMaxLevel)
                    {
                        mGoDuelWeapon.CustomActive(false);
                        return;
                    }

                    if (iBuyTopLevel > currLevel)
                    {
                        getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(iBuyTopLevel);
                        CreatDuleWeaponItem(getDuleWeaponList, 0);
                    }
                    else
                    {
                        getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(currLevel);
                        CreatDuleWeaponItem(getDuleWeaponList, 0);
                    }
                }
                else
                {
                    getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(currLevel);
                    CreatDuleWeaponItem(getDuleWeaponList, 0);
                }


            }
        }

        void TryOpenSkilltreeFrame()
        {
            var expTableData = TableManager.GetInstance().GetTableItem<ExpTable>(PlayerBaseData.GetInstance().Level);
            if (expTableData == null)
            {
                return;
            }
            uint sp = 0;
            if (SkillDataManager.GetInstance().CurPVPSKillPage == ESkillPage.Page1)
            {
                sp = PlayerBaseData.GetInstance().pvpSP;
            }
            else if (SkillDataManager.GetInstance().CurPVPSKillPage == ESkillPage.Page2)
            {
                sp = PlayerBaseData.GetInstance().pvpSP2;
            }
            if (sp >= expTableData.SpPvp)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(string.Format(TR.Value("skill_openwaitroom_tips")),
                  () =>
                  {
                      SkillFrameParam frameParam = new SkillFrameParam();
                      frameParam.frameType = SkillFrameType.Normal;
                      frameParam.tabTypeIndex = SkillFrameTabType.PVP;

                      ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle, frameParam);
                  }, () =>
                  {
                      return;
                  });
            }
        }

        void CreatDuleWeaponItem(List<ShopItemTable> itemList, int index)
        {
            if (itemList == null || index < 0 || index >= itemList.Count)
            {
                mGoDuelWeapon.CustomActive(false);
                return;
            }

            if (currLevel > 60)
            {
                mGoDuelWeapon.CustomActive(false);
                return;
            }

            {
                selfPkCoin = PlayerBaseData.GetInstance().uiPkCoin;

                //List<ShopItemTable> getDuleWeaponList = ShopDataManager.GetInstance()._ScreenCurrJobDuelWeapons(currJob, currLevel);


                ItemTable table = TableManager.GetInstance().GetTableItem<ItemTable>(itemList[index].ItemID);

                if (table == null)
                {
                    return;
                }

                ItemData itemdata = ItemDataManager.CreateItemDataFromTable(table.ID);
                ComItem ShowItem = mItemRoot.GetComponentInChildren<ComItem>();

                if (ShowItem == null)
                {
                    ShowItem = CreateComItem(mItemRoot.gameObject);
                    ShowItem.labLevel.fontSize = 30;
                    ShowItem.transform.SetAsFirstSibling();
                }

                ShowItem.Setup(itemdata, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });

                shopItemTable = TableManager.GetInstance().GetTableItem<ShopItemTable>(itemList[index].ID);

                if (shopItemTable == null)
                {
                    return;
                }

                mName.text = itemdata.GetColorName();
                mCostSlider.value = selfPkCoin / shopItemTable.CostNum;
                mCostText.text = selfPkCoin + "/" + shopItemTable.CostNum;

                var goGoumai = Utility.FindChild(mItemRoot, "EffUI_kegoumai");

                if (mCostSlider.value == 1)
                {
                    goGoumai.CustomActive(true);
                }
                else
                {
                    goGoumai.CustomActive(false);
                }
            }


        }



        #region ExtraUIBind
        private GameObject mGoDuelWeapon = null;
        private GameObject mItemRoot = null;
        private Text mName = null;
        private Slider mCostSlider = null;
        private Text mCostText = null;
        private Button mDuelShopBtn = null;
        private Button mBtGuildFighter = null;
        private Button mBt3v3 = null;
        private Button mFairDuel = null;
        private Button m2V2PointRaceBtn = null;
        private Button m3V3PointRaceBtn = null;

        private GameObject mCancelGo;

        private Text mHonorExpValueLabel;
        private CommonPKRank mCommonPKRank = null;
        protected override void _bindExUI()
        {
            mGoDuelWeapon = mBind.GetGameObject("GoDuelWeapon");
            mItemRoot = mBind.GetGameObject("ItemRoot");
            mName = mBind.GetCom<Text>("Name");
            mCostSlider = mBind.GetCom<Slider>("CostSlider");
            mCostText = mBind.GetCom<Text>("CostText");
            mDuelShopBtn = mBind.GetCom<Button>("DuelShopBtn");
            mDuelShopBtn.onClick.AddListener(_onDuelShopBtnButtonClick);
            mBtGuildFighter = mBind.GetCom<Button>("btGuildFighter");
            mBtGuildFighter.onClick.AddListener(_onBtGuildFighterButtonClick);
            mBt3v3 = mBind.GetCom<Button>("bt3v3");
            mBt3v3.onClick.AddListener(_onBt3v3ButtonClick);
            mFairDuel = mBind.GetCom<Button>("btFairDuel");
            mFairDuel.onClick.AddListener(_onBtFairDuelClick);
            mCancelGo = mBind.GetGameObject("Cancel");

            m2V2PointRaceBtn = mBind.GetCom<Button>("2V2PointRace");
            m2V2PointRaceBtn.SafeSetOnClickListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<JoinPk2v2CrossFrame>();
            });
            m3V3PointRaceBtn = mBind.GetCom<Button>("3V3PointRace");
            m3V3PointRaceBtn.SafeSetOnClickListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<JoinPK3v3CrossFrame>(FrameLayer.Middle);
            });         


            mHonorExpValueLabel = mBind.GetCom<Text>("honorExpValueLabel");
            mCommonPKRank = mBind.GetCom<CommonPKRank>("CommonPKRank");
        }

        protected override void _unbindExUI()
        {
            mGoDuelWeapon = null;
            mItemRoot = null;
            mName = null;
            mCostSlider = null;
            mCostText = null;
            mDuelShopBtn.onClick.RemoveListener(_onDuelShopBtnButtonClick);
            mDuelShopBtn = null;
            mBtGuildFighter.onClick.RemoveListener(_onBtGuildFighterButtonClick);
            mBtGuildFighter = null;
            mBt3v3.onClick.RemoveListener(_onBt3v3ButtonClick);
            mBt3v3 = null;

            mFairDuel.onClick.RemoveListener(_onBtFairDuelClick);
            mFairDuel = null;

            mCancelGo = null;

            m2V2PointRaceBtn = null;
            m3V3PointRaceBtn = null;

            mHonorExpValueLabel = null;
            mCommonPKRank = null;
        }
        #endregion
        #region Callback
        private void _onDuelShopBtnButtonClick()
        {
            //frameMgr.OpenFrame<ShopMainFrame>(FrameLayer.Middle, new ShopMainFrameData { iShopFrameID = 1, iShopID = 5 });
            //ShopFrame.OpenLinkFrame("5|0|14");
            //单独打开决斗商店

            ShopNewDataManager.GetInstance().OpenShopNewFrame(5);
        }

        private void _onBtGuildFighterButtonClick()
        {
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

                if (state >= EGuildBattleState.Preparing && state <= EGuildBattleState.Firing)
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                    {
                        GuildMainFrame.OpenLinkFrame("10");
                    }
                    else
                    {
                        GuildMainFrame.OpenLinkFrame("8");
                    }

                    return;
                }
            }
            else
            {
                frameMgr.OpenFrame<GuildListFrame>(FrameLayer.Middle);
            }
        }

        private void _onBt3v3ButtonClick()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3RoomListFrame>())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<Pk3v3RoomListFrame>(FrameLayer.Middle);

            GameStatisticManager.GetInstance().DoStartUIButton("PK3V3");
        }

        private void _onBtFairDuelClick()
        {
            if (bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<FairDuelEntranceFrame>(FrameLayer.Middle, RoomData);

        }
        /// <summary>
        /// 是否显示公平竞技场的入口按钮
        /// </summary>
        /// <param name="uiEvent"></param>
        private void _OnUpdateFairDuelEntryState(UIEvent uiEvent)
        {
            bool isShow = (bool)uiEvent.Param1;
            mFairDuel.CustomActive(isShow);
        }
        private void Update3V3PointRaceBtn(UIEvent uiEvent)
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                m3V3PointRaceBtn.CustomActive(false);
                return;
            }
            byte status = (byte)uiEvent.Param1;
            bool isFlag = false;
            if (status >= (byte)ScoreWarStatus.SWS_PREPARE && status < (byte)ScoreWarStatus.SWS_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }
            m3V3PointRaceBtn.CustomActive(isFlag);
        }
        private void Update2V2PointRaceBtn(UIEvent uiEvent)
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                m2V2PointRaceBtn.CustomActive(false);
                return;
            }
            byte status = (byte)uiEvent.Param1;
            bool isFlag = false;
            if (status >= (byte)ScoreWar2V2Status.SWS_2V2_PREPARE && status < (byte)ScoreWar2V2Status.SWS_2V2_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }
            m2V2PointRaceBtn.CustomActive(isFlag);
        }
        #endregion

        private void OnCountValueChangeChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            string countKey = (string)uiEvent.Param1;

            if (string.Equals(countKey, HonorSystemDataManager.GetInstance().PkHonorExpCounterStr) == true)
                UpdateHonorExpValueLabel();
        }

        private void UpdateHonorExpValueLabel()
        {
            if (mHonorExpValueLabel == null)
                return;

            var honorExpValueStr = HonorSystemUtility.GetThisWeekHonorExpStrInPk();
            mHonorExpValueLabel.text = honorExpValueStr;
        }
    }
}
