using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using Network;

namespace GameClient
{
    class BudoArenaFrameData
    {
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;
    }
    class BudoArenaFrame : ClientFrame
    {
        public static void Open(BudoArenaFrameData data = null)
        {
            ClientSystemManager.GetInstance().CloseFrame<BudoArenaFrame>();
            ClientSystemManager.GetInstance().OpenFrame<BudoArenaFrame>(FrameLayer.Bottom, data);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Budo/BudoArenaFrame";
        }

        BudoArenaFrameData m_kData = null;

        Button btnToTraditional;
        Button btnMatch;
        Button btnStopMatch;
        MatchCounterDown comCounterDown;
        ComTalk comTalk = null;

        GameObject goSeek = null;
        UIAudioProxy comAudio = null;
        GameObject _LoadSeekCom(GameObject goParent,string path = "UIFlatten/Prefabs/Pk/Seek")
        {
            if(goParent == null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            GameObject goSeek = AssetLoader.instance.LoadRes(path, typeof(GameObject)).obj as GameObject;
            Utility.AttachTo(goSeek, goParent);
            goSeek.CustomActive(true);
            comAudio = goSeek.GetComponent<UIAudioProxy>();
            if(null != comAudio)
            {
                comAudio.TriggerAudio(8888);
            }

            comCounterDown = Utility.FindComponent<MatchCounterDown>(goSeek, "CountDown");
            comCounterDown.enabled = false;

            return goSeek;
        }

        protected override void _OnOpenFrame()
        {
            m_kData = userData as BudoArenaFrameData;
            btnToTraditional = Utility.FindComponent<Button>(frame, "BtnSwitchToFight");
            if(btnToTraditional != null)
            {
                btnToTraditional.onClick.AddListener(_OnClickSwithToTraditional);
            }
            goMask = Utility.FindChild(frame, "FrontPage");
            goMask.CustomActive(false);
            goTimer = Utility.FindChild(frame, "MatchInfo");
            goTimer.CustomActive(false);
            goSeek = _LoadSeekCom(Utility.FindChild(frame, "MatchInfo"));

            bIsMatch = false;

            btnMatch = Utility.FindComponent<Button>(frame, "BtnStartMatch");
            btnMatch.onClick.AddListener(_OnClickMatch);
            btnMatch.CustomActive(true);
            btnStopMatch = Utility.FindComponent<Button>(frame, "BtnStopMatch");
            btnStopMatch.onClick.AddListener(_OnClickStopMatch);
            btnStopMatch.CustomActive(false);
            bIsWaiting = false;
            _InitStars();

            _UpdateBudoInfo();

            BudoManager.GetInstance().onBudoInfoChanged += _UpdateBudoInfo;
            BudoManager.GetInstance().onBudoInfoChanged += OnBudoInfoChanged;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActiveUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnMatchOk);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchFailed, _OnMatchStop);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnMatchCancelOk);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelFailed, _OnMatchCancelFailed);

            comTalk = ComTalk.Create(Utility.FindChild(frame, "TalkParent"));
        }

        void _OnActiveUpdate(UIEvent uiEvent)
        {
            uint nID = (uint)uiEvent.Param1;
            if (nID == BudoManager.ActiveID)
            {
                if(ActiveManager.GetInstance().allActivities.ContainsKey((int)nID))
                {
                    var activity = ActiveManager.GetInstance().allActivities[(int)nID];
                    if(activity != null && activity.state == 0)
                    {
                        if(bIsMatch)
                        {
                            WorldMatchCancelReq req = new WorldMatchCancelReq();
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                        }
                    }
                }
            }
        }

        void _OnMatchOk(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = true;
            goMask.CustomActive(true);
        }

        void _OnMatchStop(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = false;
            goMask.CustomActive(false);

            btnMatch.CustomActive(true);
            btnStopMatch.CustomActive(false);

            goTimer.CustomActive(false);
            comCounterDown.enabled = false;
        }

        void _OnMatchCancelOk(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = false;
            goMask.CustomActive(false);

            btnMatch.CustomActive(true);
            btnStopMatch.CustomActive(false);

            goTimer.CustomActive(false);
            comCounterDown.enabled = false;
        }

        void _OnMatchCancelFailed(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = true;
            goMask.CustomActive(true);

            btnMatch.CustomActive(false);
            btnStopMatch.CustomActive(true);
        }

        [UIControl("ResultIntimeShow/Name", typeof(Text))]
        Text m_kJarName;
        [UIControl("ResultIntimeShow/Content/HintWin", typeof(Text))]
        Text m_kBudoWinTimes;
        [UIControl("ResultIntimeShow/FriendlyHint")]
        Text m_kBudoFriendlyHint;
        [UIObject("ResultIntimeShow/Content/Results")]
        GameObject goBudoParent;
        List<GameObject> m_akRoots = new List<GameObject>();
        List<GameObject> m_akStarBacks = new List<GameObject>();
        List<GameObject> m_akStarFronts = new List<GameObject>();
        Button btnAward;
        UIGray grayAward;
        ComItem comJarItem;
        GameObject goMask;
        GameObject goTimer;
        bool bIsWaiting = false;
        bool bIsMatch = false;

        void _OnClickAward()
        {
            BudoManager.GetInstance().SendSceneWudaoRewardReq();
        }
        void _InitStars()
        {
            comJarItem = CreateComItem(Utility.FindChild(frame, "ResultIntimeShow/Content/ItemParent"));
            comJarItem.Setup(null, null);
            btnAward = Utility.FindComponent<Button>(frame, "ResultIntimeShow/BtnTaskAward");
            btnAward.onClick.AddListener(_OnClickAward);
            grayAward = btnAward.gameObject.GetComponent<UIGray>();
            for (int i = 0; i < goBudoParent.transform.childCount; ++i)
            {
                var child = goBudoParent.transform.GetChild(i).gameObject;
                m_akRoots.Add(child);
                m_akStarBacks.Add(child.transform.Find("bg").gameObject);
                m_akStarFronts.Add(child.transform.Find("child").gameObject);
            }
        }
        void _UnInitStars()
        {
            m_akRoots.Clear();
            m_akStarBacks.Clear();
            m_akStarFronts.Clear();
            if(btnAward != null)
            {
                btnAward.onClick.RemoveAllListeners();
                btnAward = null;
            }
        }

        void _PreViewItem(GameObject obj, ItemData item)
        {
            if(item != null)
            {
                BudoManager.GetInstance().OpenBudoPreviewFrame((int)item.TableID);
            }
        }

        void OnBudoInfoChanged()
        {
            //if(BudoManager.GetInstance().CanAcqured)
            //{
            //    var pkData = PlayerBaseData.GetInstance().PkEndData;
            //    BudoResultFrameData data = new BudoResultFrameData();
            //    data.bNeedAnimation = true;
            //    data.bDebug = false;
            //    data.bNeedOpenBudoInfo = false;
            //    data.eResult = (PKResult)pkData.result;
            //    BudoResultFrame.Open(data);
            //}
        }

        void _UpdateBudoInfo()
        {
            var itemData = BudoManager.GetInstance().GetJarDataByTimes();
            if(itemData != null)
            {
                m_kJarName.text = itemData.GetColorName();
                comJarItem.Setup(itemData, _PreViewItem);
            }
            else
            {
                comJarItem.Setup(null, null);
            }
            m_kBudoWinTimes.text = string.Format(TR.Value("budo_win_times"),BudoManager.GetInstance().WinTimes);

            for(int i = 0; i < m_akStarBacks.Count; ++i)
            {
                m_akRoots[i].CustomActive(i < BudoManager.GetInstance().MaxLoseTimes);
                m_akStarBacks[i].CustomActive(i < BudoManager.GetInstance().MaxLoseTimes);
                m_akStarFronts[i].CustomActive(i < BudoManager.GetInstance().LoseTimes);
            }

            m_kBudoFriendlyHint.text = string.Format(TR.Value("budo_result_hint"), BudoManager.GetInstance().MaxWinTimes, BudoManager.GetInstance().MaxLoseTimes);
            bool bCanAward = BudoManager.GetInstance().CanAcqured;
            btnAward.enabled = bCanAward;
            grayAward.enabled = !bCanAward;
            btnAward.CustomActive(false);
        }

        [UIEventHandle("btMainMenu")]
        void OnClickMainMenu()
        {
            //BudoManager.GetInstance().SendReturnToTownRelation();
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("btReturnToTown")]
        void OnClickReturnToTown()
        {
            if (bIsMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if (m_kData == null)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town!!!");
                return;
            }

            ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = m_kData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = m_kData.TargetTownSceneID,
                targetDoorID = 0,
            },true));

            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("btMenu")]
        void OnBtnMebuClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
        }

        void _OnClickMatch()
        {
            if(bIsMatch)
            {
                return;
            }
            if (!BudoManager.GetInstance().CanMatch)
            {
                Logger.LogErrorFormat("CanNotMatch!!!!");
                return;
            }
            if(TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }
            if (bIsWaiting)
            {
                return;
            }
            bIsWaiting = true;
            bIsMatch = true;

            _BeginMatch();
            goTimer.CustomActive(true);
            comCounterDown.enabled = true;
            BudoManager.GetInstance().SendMatchParty();
        }

        void _OnClickStopMatch()
        {
            if(!bIsMatch)
            {
                return;
            }
            if (bIsWaiting)
            {
                return;
            }
            bIsWaiting = true;
            bIsMatch = false;

            btnMatch.CustomActive(true);
            btnStopMatch.CustomActive(false);
            goMask.CustomActive(true);

            WorldMatchCancelReq req = new WorldMatchCancelReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait<WorldMatchCancelRes>(msgRet =>
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
            });
        }

        void _BeginMatch()
        {
            btnMatch.CustomActive(false);
            btnStopMatch.CustomActive(true);
            goMask.CustomActive(true);
        }

        protected override void _OnCloseFrame()
        {
            if(bIsMatch)
            {
                //send
                bIsMatch = false;
            }

            if(btnMatch != null)
            {
                btnMatch.onClick.RemoveAllListeners();
                btnMatch = null;
            }
            if(btnStopMatch != null)
            {
                btnStopMatch.onClick.RemoveAllListeners();
                btnStopMatch = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnMatchOk);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchFailed, _OnMatchStop);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnMatchCancelOk);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelFailed, _OnMatchCancelFailed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActiveUpdate);

            if (btnToTraditional != null)
            {
                btnToTraditional.onClick.RemoveAllListeners();
                btnToTraditional = null;
            }
            m_kData = null;
            comJarItem.Setup(null, null);
            comJarItem = null;

            if(comTalk != null)
            {
                ComTalk.Recycle();
                comTalk = null;
            }

            BudoManager.GetInstance().onBudoInfoChanged -= _UpdateBudoInfo;
            BudoManager.GetInstance().onBudoInfoChanged -= OnBudoInfoChanged;

            _UnInitStars();
			
			UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMission);

			//ComTalk.Recycle();
        }

        void _OnClickSwithToTraditional()
        {
            if (bIsMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }
            Utility.SwitchToPkWaitingRoom(PkRoomType.TraditionPk);
            Close();
        }
    }
}