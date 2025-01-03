using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace GameClient
{
    public class FunctionFrame : ClientFrame
    {
        public enum FunctionItemType
        {
            FIT_MISSION = 0,
            FIT_TEAM,
            FIT_COUNT,
        }

        FunctionItemType eFunctionItemType;

        //[UIControl("ButtonArray/BtnMission",typeof(Image))]
        //Image btnMission;
        //[UIControl("ButtonArray/BtnTeam", typeof(Image))]
        //Image btnTeam;
        //Image[] btnArray;
        ScrollRect scrollRect;
        GameObject goArrowDown;
        GameObject goArrowUp;

        #region MissionTraceObject
        GameObject m_goMissionParent;
        GameObject m_goMissionPrefab;
        class MissionTraceObject : CachedObject
        {
            static string ms_empty = "empty";
            static string ms_created = "alive";
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            int iId;
            FunctionFrame THIS;

            LinkParse content;
            Text name;
            Button link;

            Text jumpHint;
            GameObject goJumpLink;

            MissionManager.SingleMissionInfo value;
            ProtoTable.MissionTable missionItem;

            ComEffect comEffect;
            ComItem comAward;
            Button onClickAward;
            GameObject goAwake;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                iId = (int)param[2];
                THIS = param[3] as FunctionFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    content = Utility.FindComponent<LinkParse>(goLocal, "ScrollView/ViewPort/Content");

                    name = Utility.FindComponent<Text>(goLocal, "Tittle");

                    jumpHint = Utility.FindComponent<Text>(goLocal,"JumpHint");
                    goJumpLink = Utility.FindChild(goLocal, "JumpLink");

                    link = goLocal.GetComponent<Button>();

                    goAwake = Utility.FindChild(goLocal, "Awake");

                    comEffect = goLocal.GetComponent<ComEffect>();

                    comAward = THIS.CreateComItem(Utility.FindChild(goLocal, "ItemParent"));
                    comAward.transform.SetAsFirstSibling();

                    onClickAward = Utility.FindComponent<Button>(goLocal, "ItemParent/OnClick");
                    onClickAward.onClick.RemoveAllListeners();
                    onClickAward.onClick.AddListener(() =>
                    {
                        if(null != comAward && comAward.gameObject.activeSelf)
                        {
                            ItemTipManager.GetInstance().ShowTip(comAward.ItemData, null);
                        }
                    });
                }

                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);

                goLocal.name = ms_created;
                Enable();
                _Update();
            }

            public bool IsTaskGuiding
            {
                get
                {
                    return NewbieGuideManager.GetInstance().IsGuidingControl();                   
                }
            }

            void OnNewbieGuideStart(UIEvent uiEvent)
            {
                _Update();
            }

            void OnNewbieGuideFinish(UIEvent uiEvent)
            {
                _Update();
            }

            public override void OnRecycle()
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
                if (goLocal != null)
                {
                    comEffect.Stop("mainEffect");
                    comEffect.Stop("attachEffect");
                    goLocal.CustomActive(false);
                }
                goLocal.name = ms_empty;
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }

            public override void OnDestroy()
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
                if(null != onClickAward)
                {
                    onClickAward.onClick.RemoveAllListeners();
                    onClickAward = null;
                }
                if(null != link)
                {
                    link.onClick.RemoveAllListeners();
                    link = null;
                }
            }

            public void SetAsFirstSibling()
            {
                if(goLocal != null)
                {
                    goLocal.transform.SetAsFirstSibling();
                }
            }

            public override void SetAsLastSibling()
            {
                if(goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            void _Update()
            {
                if(comAward == null)
                {
                    return;
                }

                comEffect.Stop("mainEffect");
                comEffect.Stop("attachEffect");
                comEffect.Stop("awakeEffect");
                comAward.CustomActive(false);
                comAward.Setup(null, null);
                onClickAward.gameObject.CustomActive(false);
                goAwake.CustomActive(false);

                if (iId != 0 && iId != 1 && iId != 2)
                {
                    jumpHint.CustomActive(false);
                    goJumpLink.CustomActive(false);
                    missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iId);
                    MissionManager.GetInstance().taskGroup.TryGetValue((uint)iId, out value);
                    if (value != null)
                    {
                        //设置任务名称
                        name.text = MissionManager.GetInstance().GetMissionName((uint)missionItem.ID) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status,value.missionItem.ID);
                        //设置任务内容
                        if (content != null)
                        {
                            content.SetText(Utility.ParseMissionText(iId, true));
                        }
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                            link.onClick.AddListener(() => { THIS.OnClickLink(iId); });
                        }

                        if(missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
                        {
                            comEffect.Play("mainEffect");
                            if(missionItem.MinPlayerLv <= 10 && !IsTaskGuiding)
                            {
                                comEffect.Play("attachEffect");
                            }
                        }

                        if(value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_TITLE)
                        {
                            int iFnalID = MissionManager.GetInstance().GetFinalTitleMission(value.missionItem.ID);
                            var awards = MissionManager.GetInstance().GetMissionAwards(iFnalID, PlayerBaseData.GetInstance().JobTableID);
                            if(null != awards && awards.Count > 0)
                            {
                                var current = awards[awards.Count - 1];
                                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(current.ID);
                                if(null != itemData)
                                {
                                    comAward.Setup(itemData,null);
                                    comAward.CustomActive(true);
                                    onClickAward.gameObject.CustomActive(true);
                                }
                            }
                        }

                        if(value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
                        {
                            comEffect.Play("awakeEffect");
                            goAwake.CustomActive(true);
                        }
                    }
                }
                else
                {
                    missionItem = null;
                    value = null;

                    if(iId == 1)
                    {
                        jumpHint.CustomActive(false);
                        goJumpLink.CustomActive(false);
                        //设置任务名称
                        name.text = TR.Value("mission_has_no_mission");
                        //设置任务内容
                        content.SetText(TR.Value("mission_finish_all"), false);
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                        }
                        return;
                    }

                    if(iId == 2)
                    {
                        jumpHint.CustomActive(false);
                        goJumpLink.CustomActive(false);

                        //设置任务名称
                        name.text = TR.Value("mission_has_no_main");
                        //设置任务内容
                        content.SetText(TR.Value("mission_finish_all_main"), false);
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                        }
                        return;
                    }

                    var nextMissionItem = MissionManager.GetInstance().GetNextMissionItem(PlayerBaseData.GetInstance().Level);
                    if(nextMissionItem == null)
                    {
                        jumpHint.CustomActive(false);
                        goJumpLink.CustomActive(false);
                        //设置任务名称
                        name.text = TR.Value("mission_has_no_main");
                        //设置任务内容
                        content.SetText(TR.Value("mission_finish_all_main"), false);
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                        }
                    }
                    else
                    {
                        jumpHint.CustomActive(true);
                        goJumpLink.CustomActive(true);
                        //设置任务名称
                        name.text = MissionManager.GetInstance().GetMissionName((uint)nextMissionItem.ID);
                        //设置任务内容
                        int iNextLv = MissionManager.GetInstance().GetNextLevelMission(PlayerBaseData.GetInstance().Level);
                        content.SetText("", false);
                        jumpHint.text = string.Format(TR.Value("mission_need_lv"), iNextLv);
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                            link.onClick.AddListener(() =>
                            {
                                ActiveManager.GetInstance().OnClickLinkInfo("<type=framename param=1 value=GameClient.DevelopGuidanceMainFrame>");
                            });
                        }
                    }
                }
            }
        }
        CachedObjectListManager<MissionTraceObject> m_akMissionTraceObjects = new CachedObjectListManager<MissionTraceObject>();
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionFrame/FunctionFrame";
        }

        protected override void _OnCloseFrame()
        {
            MissionManager.GetInstance().onAddNewMission -= OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= OnDeleteMission;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, FrameUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FunctionFrameUpdate, FrameUpdate);
            m_akMissionTraceObjects.DestroyAllObjects();

            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect = null;
        }

        protected override void _OnOpenFrame()
        {
            eFunctionItemType = FunctionItemType.FIT_MISSION;
            m_goMissionParent = Utility.FindChild(frame, "ScrollView/ViewPort/Content");
            m_goMissionPrefab = Utility.FindChild(frame, "ScrollView/ViewPort/Content/Prefab");
            m_goMissionPrefab.CustomActive(false);

            MissionManager.GetInstance().onAddNewMission += OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteMission;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, FrameUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FunctionFrameUpdate, FrameUpdate);

            scrollRect = Utility.FindComponent<ScrollRect>(frame, "ScrollView");
            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect.onValueChanged.AddListener(_OnValueChanged);
            //goArrowDown = Utility.FindChild(frame, "ArrowDown");
            //goArrowUp = Utility.FindChild(frame, "ArrowUp");

            _SetFunction();
        }

        void _OnValueChanged(Vector2 value)
        {
            if(m_akMissionTraceObjects.ActiveObjects.Count > 2)
            {
                if (value.y <= 0.0f)
                {
                    //goArrowDown.CustomActive(false);
                    //goArrowUp.CustomActive(true);
                }
                else
                {
                    //goArrowDown.CustomActive(true);
                    //goArrowUp.CustomActive(false);
                }
            }
            else
            {
                //goArrowDown.CustomActive(false);
                //goArrowUp.CustomActive(false);
            }
        }

        void FrameUpdate(UIEvent uiEvent)
        {
            _SetFunction();
        }

        public bool IsNormalMission(UInt32 iMissionID)
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
            if(missionItem == null)
            {
                return false;
            }

            if(missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH ||
                missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN ||
                missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE ||
                missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB ||
                missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_TITLE ||
                missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
            {
                return true;
            }

            return false;
        }

        void OnAddNewMission(UInt32 iTaskID)
        {
            if (IsNormalMission(iTaskID))
            {
                _SetFunction();
            }
        }

        void OnUpdateMission(UInt32 iTaskID)
        {
            if (IsNormalMission(iTaskID))
            {
                _SetFunction();
            }
        }

        void OnDeleteMission(UInt32 iTaskID)
        {
            if (IsNormalMission(iTaskID))
            {
                _SetFunction();
            }
        }

        private void _SetSelectedButton()
        {
//             for (int i = 0; i < btnArray.Length; ++i)
//             {
//                 if (btnArray[i] != null)
//                 {
//                     if ((int)eFunctionItemType == i)
//                     {
// 						btnArray[i].sprite = Utility.createSprite("UIPacked/p-Interface.png:Interface_taskItem");
//                     }
//                     else
//                     {
// 						btnArray[i].sprite = Utility.createSprite("UIPacked/p-Interface.png:Interface_taskItembg");
//                     }
//                 }
//             }
        }

        void OnClickLink(Int32 iTaskID)
        {
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
				//Logger.LogErrorFormat("OnClickLink task:{0}", iTaskID);
                MissionManager.GetInstance().AutoTraceTask(iTaskID);
            }
        }

        private void _SetFunction()
        {
            //获取任务列表
            m_akMissionTraceObjects.RecycleAllObject();
            var traceTaskList = MissionManager.GetInstance().GetTraceTaskList();
            if (MissionManager.GetInstance().ListCnt > 0)
            {
                for(int i = 0; i < MissionManager.GetInstance().ListCnt; ++i)
                {
                    int iId = traceTaskList[i];
                    var current = m_akMissionTraceObjects.Create(new object[] {m_goMissionParent,m_goMissionPrefab, iId,this });
                    if(current != null)
                    {
                        if (iId != 0 && iId != 1 && iId != 2)
                        {
                            current.SetAsLastSibling();
                        }
                        else
                        {
                            current.SetAsFirstSibling();
                        }
                    }
                }
            }
            _OnValueChanged(scrollRect.normalizedPosition);

            var clientSystemTown = ClientSystem.GetTargetSystem<ClientSystemTown>();
            if (clientSystemTown != null && clientSystemTown.MainPlayer != null && MissionManager.GetInstance().ListCnt > 0)
            {
                clientSystemTown.MainPlayer.CreateMissionLink((UInt32)traceTaskList[0]);
            }
        }
    }
}