using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComFunctionItem : MonoBehaviour
    {
        public LinkParse content;
        public Text Name;
        public Text jumpHint;
        public GameObject goJumpLink;
        public Button link;
        public GameObject goAwake;
        public ComEffect comEffect;
        public GameObject goItemParent;
        ComItem comAward;
        public Image bg;
        public GameObject goFinishAnimation;

        [SerializeField]
        private bool willOpenDailyTodo = true;

        [SerializeField]
        Image taskType = null;

        static string ms_empty = "empty";
        static string ms_created = "alive";

        int iId = -1;
        MissionManager.SingleMissionInfo value;
        ProtoTable.MissionTable missionItem;
        public bool IsExist()
        {
            if (iId < 10 && iId >= 0)
            {
                return true;
            }

            if (value == null || value.missionItem == null)
            {
                return false;
            }

            if (!MissionManager.GetInstance().taskGroup.ContainsKey((uint)value.missionItem.ID))
            {
                return false;
            }

            var curValue = MissionManager.GetInstance().taskGroup[(uint)value.missionItem.ID];
            if (curValue == null)
            {
                return false;
            }

            return true;
        }
        public void PlayAnimation()
        {
            if (null != goFinishAnimation)
            {
                goFinishAnimation.CustomActive(true);
            }
        }
        public void StopAnimation()
        {
            if (null != goFinishAnimation)
            {
                goFinishAnimation.CustomActive(false);
            }
        }
        bool m_bSelected = false;
        bool bSelected
        {
            get
            {
                return m_bSelected;
            }
            set
            {
                m_bSelected = value;
                _UpdateBG();
            }
        }
        public bool IsTaskGuiding
        {
            get
            {
                return NewbieGuideManager.GetInstance().IsGuidingControl();
            }
        }

        public static string ms_selected_texture = "task_trace_selected_texture";
        public static string ms_normal_texture = "task_trace_normal_texture";

        void _UpdateBG()
        {
            if (null != bg)
            {
                bg.sprite = AssetLoader.instance.LoadRes(bSelected ? TR.Value(ms_selected_texture) : TR.Value(ms_normal_texture), typeof(Sprite)).obj as Sprite;
            }
        }

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TraceBegin, OnTraceBegin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TraceEnd, OnTraceEnd);
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TraceBegin, OnTraceBegin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TraceEnd, OnTraceEnd);
            if (null != link)
            {
                link.onClick.RemoveAllListeners();
                link = null;
            }
            if (null == comAward)
            {
                ComItemManager.Destroy(comAward);
                comAward = null;
            }
        }

        void OnNewbieGuideStart(UIEvent uiEvent)
        {
            if(this.iId != -1)
            {
                OnItemVisible(this.iId);
            }
        }

        void OnNewbieGuideFinish(UIEvent uiEvent)
        {
            if (this.iId != -1)
            {
                OnItemVisible(this.iId);
            }
        }

        void OnTraceBegin(UIEvent uiEvent)
        {
            int traceId = (int)uiEvent.Param1;
            MissionManager.GetInstance().FunctionTraceID = traceId;
            if (iId == traceId)
            {
                bSelected = true;
            }
        }

        void OnTraceEnd(UIEvent uiEvent)
        {
            int traceId = (int)uiEvent.Param1;
            if (MissionManager.GetInstance().FunctionTraceID == traceId)
            {
                MissionManager.GetInstance().FunctionTraceID = -1;
            }
            if (iId == traceId)
            {
                bSelected = false;
            }
        }

        void OnClickLink(int iTaskID, UnityEngine.Events.UnityAction onBegin, UnityEngine.Events.UnityAction onEnd)
        {
            if (ClientSystemManager.GetInstance().CurrentSystem is ClientSystemTown)
            {
                MissionManager.SingleMissionInfo missioninfo = null;

                // if (MissionManager.GetInstance().IsChangeJobMainMission(iTaskID, ref missioninfo))
                // {
                //     if (!Utility.CheckCanChangeJob())
                //     {
                //         return;
                //     }
                // }

                //Logger.LogErrorFormat("OnClickLink task:{0}", iTaskID);
                if (null != onBegin)
                {
                    onBegin();
                }

                MissionManager.GetInstance().AutoTraceTask(iTaskID, onEnd, onEnd);
            }
        }

        string GetTaskTypeImgPath(int missionTableID)
        {
            var table = TableManager.GetInstance().GetTableItem<MissionTable>(missionTableID);
            if(table == null)
            {
                return "";
            }

            switch(table.TaskType)
            {
                case MissionTable.eTaskType.TT_MAIN:
                    return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_01";
                case MissionTable.eTaskType.TT_BRANCH:
                    return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_02";                
                case MissionTable.eTaskType.TT_CYCLE:
                    return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_03";
                case MissionTable.eTaskType.TT_TITLE:
                    return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_04";
                case MissionTable.eTaskType.TT_CHANGEJOB:
                case MissionTable.eTaskType.TT_AWAKEN:
                    return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_05";
            }

            return "UI/Image/NewPacked/MainUI01.png:MainUI01_Task_Leixing_02";
        }

        public void OnItemVisible(int iId)
        {
            this.iId = iId;
            if(null == comAward)
            {
                comAward = ComItemManager.Create(goItemParent);
            }
            gameObject.name = ms_created;

            _UpdateBG();

            if(null != comEffect)
            {
                comEffect.Stop("mainEffect");
                comEffect.Stop("attachEffect");
                comEffect.Stop("awakeEffect");
                comEffect.Stop("JinGuangEffect");
            }

            if(null != comAward)
            {
                comAward.CustomActive(false);
                comAward.Setup(null, null);
            }
            goAwake.CustomActive(false);

            int traceId = iId;
            if (iId != 0 && iId != 1 && iId != 2)
            {
                jumpHint.CustomActive(false);
                goJumpLink.CustomActive(false);
                missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iId);
                MissionManager.GetInstance().taskGroup.TryGetValue((uint)iId, out value);
                if (value != null)
                {
                    taskType.SafeSetImage(GetTaskTypeImgPath(iId));


                    //设置任务名称
                    Name.text = MissionManager.GetInstance().GetMissionName((uint)missionItem.ID) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status, value.missionItem.ID);
                    //设置任务内容
                    if (content != null)
                    {
                        if (missionItem.SubType == MissionTable.eSubType.SummerNpc)
                        {
                            //需要解析，有点繁琐
                            content.SetText(Utility.ParseMissionText(iId, true));
                        }
                        else
                        {
                            content.SetText(Utility.ParseMissionText(iId, true));
                        }
                    }
                    //设置点击链接
                    if (link != null)
                    {
                        link.onClick.RemoveAllListeners();
                        link.onClick.AddListener(() =>
                        {
                            OnClickLink(iId,
                                () =>
                                {
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TraceBegin, traceId);
                                },
                                () =>
                                {
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TraceEnd, traceId);
                                }
                                );
                        });
                    }

                    if (missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
                    {
                        if (PlayerBaseData.GetInstance().Level <= 15)
                        {
                            comEffect.Play("JinGuangEffect");
                        }
                        else
                        {
                            comEffect.Play("mainEffect");
                        }
                        if (missionItem.MinPlayerLv <= 10 && !IsTaskGuiding)
                        {
                            comEffect.Play("attachEffect");
                        }
                    }

                    if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_TITLE)
                    {
                        int iFnalID = MissionManager.GetInstance().GetFinalTitleMission(value.missionItem.ID);
                        var awards = MissionManager.GetInstance().GetMissionAwards(iFnalID, PlayerBaseData.GetInstance().JobTableID);
                        if (null != awards && awards.Count > 0)
                        {
                            var current = awards[awards.Count - 1];
                            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(current.ID);
                            if (null != itemData)
                            {
                                comAward.Setup(itemData, (GameObject go,ItemData item)=> { ItemTipManager.GetInstance().ShowTip(item); });
                                comAward.CustomActive(true);
                            }
                        }
                    }

                    if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
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

                if (iId == 1)
                {
                    jumpHint.CustomActive(false);
                    goJumpLink.CustomActive(false);
                    //设置任务名称
                    Name.text = TR.Value("mission_has_no_mission");
                    //设置任务内容
                    content.SetText(TR.Value("mission_finish_all"), false);
                    //设置点击链接
                    if (link != null)
                    {
                        link.onClick.RemoveAllListeners();
                    }
                    return;
                }

                if (iId == 2)
                {
                    jumpHint.CustomActive(false);
                    goJumpLink.CustomActive(false);

                    //设置任务名称
                    Name.text = TR.Value("mission_has_no_main");
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
                if (nextMissionItem == null)
                {
                    jumpHint.CustomActive(false);
                    goJumpLink.CustomActive(false);
                    //设置任务名称
                    Name.text = TR.Value("mission_has_no_main");
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
                    if (PlayerBaseData.GetInstance().Level <= 15)
                    {
                        comEffect.Play("JinGuangEffect");
                    }

                    jumpHint.CustomActive(true);
                    goJumpLink.CustomActive(true);
                    //设置任务名称
                    Name.text = MissionManager.GetInstance().GetMissionName((uint)nextMissionItem.ID);
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
                            if (willOpenDailyTodo && DailyTodoDataManager.GetInstance().BFuncOpen)
                            {
                                ActiveManager.GetInstance().OnClickLinkInfo(DailyTodoFrame.OPEN_LINK_INFO);
                            }
                            else
                            {
                                ActiveManager.GetInstance().OnClickLinkInfo("<type=framename param=1 value=GameClient.DevelopGuidanceMainFrame>");
                            }
                        });
                    }
                }
            }
        }
    }
}