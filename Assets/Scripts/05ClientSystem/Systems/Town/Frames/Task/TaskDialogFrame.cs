using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using System.Reflection;
using ProtoTable;

namespace GameClient
{
    public class TaskDialogFrame : ClientFrame
    {
        private Int32 dlgId = 0;
        private int initialDlgId = 0;  //表示连续对话时都是同一个预置物比如talktop，该值记录的是这一段连续对话的第一个dlgid
        private GameObject goNpcAnimationParent = null;
        private GameObject goAnimation = null;
        private Animation comAnimation = null;
        private ProtoTable.TalkTable talkItem = null;
        private UnityEngine.UI.Text npcName = null;
        private UnityEngine.UI.Text talkContent = null;
        private int iCurTaskId = 0;
        private UnityEngine.UI.Text taskName = null;
        private UISpecialFrameCreate comSpecialFrameCreate = null;
        private GeObjectRenderer objRender = null;

        private List<string> aniNames = new List<string>() {"stand" };
        private Vector3 playerCameraPosDiff = new Vector3(2.85f, 5.65f, 0);
        private const int playerNpcId = 2056;  //npcid为2056时表示玩家

        public class OnDialogOver : UnityEngine.Events.UnityEvent
        {
            public new OnDialogOver AddListener(UnityEngine.Events.UnityAction callback)
            {
                base.AddListener(callback);
                return this;
            }
        }
        private OnDialogOver onDialogOver;

        float RaycastInterval
        {
            get
            {
                float fInterval = 0.0f;
                float.TryParse(TR.Value("task_dlg_jump_interval"),out fInterval);
                return fInterval;
            }
        }
        bool bInteractable = false;

        public string GetObjectName()
        {
            return talkItem.ObjectName;
        }

        private void OnReset()
        {
            dlgId = 0;
            talkItem = null;
            talkContent = null;
            iCurTaskId = 0;
            if(comSpecialFrameCreate != null)
            {
                comSpecialFrameCreate.OnClose();
            }

            bInteractable = false;
            InvokeMethod.RemoveInvokeCall(this);
            InvokeMethod.Invoke(this, RaycastInterval, () =>
            {
                bInteractable = true;
            });
        }

        public override string GetPrefabPath()
        {
            dlgId = MissionManager.GetInstance().AddKeyDlg2Frame(this);
            initialDlgId = dlgId;
            talkItem = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(dlgId);
            if(talkItem != null)
            {
                //this.frame.name = talkItem.ObjectName;
                return talkItem.ObjectName;
            }
            else
            {
                MissionManager.GetInstance().RemoveDlgFrame(this.dlgId);
                Logger.LogError(string.Format("dlgId = {0} can not be found in table TalkTable!", dlgId));
                this.dlgId = 0;
                initialDlgId = 0;
            }

            return "";
        }

        protected override void _bindExUI()
        {
            base._bindExUI();

            if (mBind == null)
            {
                return;
            }

            objRender = mBind.GetCom<GeObjectRenderer>("objRender");
        }

        protected override void _unbindExUI()
        {
            base._unbindExUI();

            objRender = null;
        }

        protected override void _OnOpenFrame()
        {
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.TaskDialogFrameOpen);

            bInteractable = false;
            InvokeMethod.Invoke(this, RaycastInterval, () =>
             {
                 bInteractable = true;
             });

            if (NewbieGuideManager.GetInstance().IsGuiding() && NewbieGuideManager.GetInstance().GetCurGuideType() != NewbieGuideComType.TALK_DIALOG)
            {
                //SetVisible(false);
            }
            else
            {
                SetVisible(true);
            }

            BindUIEvent();
            AttachUIObject();
            //SetDialogUIData();        //必定触发taskidchanged 里面会触发该函数，无需重复触发

            onDialogOver = new OnDialogOver();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Dlg2TaskId, OnTaskIdChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DlgCallBack, OnAddCallBack);
        }

        private void  OnTaskIdChanged(UIEvent uiEvent)
        {
            this.iCurTaskId = uiEvent.EventParams.taskData.taskID;
            SetDialogUIData();
        }

        private void OnAddCallBack(UIEvent uiEvent)
        {
            UIEventDialogCallBack callbackInfo = uiEvent as UIEventDialogCallBack;
            if(callbackInfo != null)
            {
                onDialogOver = callbackInfo.callback;
            }
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();

            InvokeMethod.RemoveInvokeCall(this);

            MissionManager.GetInstance().RemoveDlgFrame(initialDlgId);      //这里用initialDlgId,因为add的时候也是add的她
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Dlg2TaskId, OnTaskIdChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DlgCallBack, OnAddCallBack);
            if (goAnimation != null)
            {
                GameObject.Destroy(goAnimation);
                goAnimation = null;
            }
            comAnimation = null;
            if(onDialogOver != null)
            {
                //onDialogOver.RemoveAllListeners();
                onDialogOver = null;
            }
            if(comSpecialFrameCreate != null)
            {
                comSpecialFrameCreate.OnClose();
                comSpecialFrameCreate = null;
            }
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, OnNewbieGuideFinish);
        }

        void OnNewbieGuideStart(UIEvent uiEvent)
        {
            if (NewbieGuideManager.GetInstance().IsGuiding() && NewbieGuideManager.GetInstance().GetCurGuideType() != NewbieGuideComType.TALK_DIALOG)
            {
                SetVisible(false);
            }
        }

        void OnNewbieGuideFinish(UIEvent uiEvent)
        {
            SetVisible(true);
        }

        private void AttachUIObject()
        {
            npcName = frame.transform.Find("BackGround").transform.Find("NpcName").GetComponent<UnityEngine.UI.Text>();
            talkContent = frame.transform.Find("BackGround").transform.Find("Talk").GetComponent<UnityEngine.UI.Text>();
            goNpcAnimationParent = Utility.FindChild(frame, "NpcAnimation");
            taskName = Utility.FindComponent<UnityEngine.UI.Text> (frame, "TaskName");
            comSpecialFrameCreate = Utility.FindComponent<UISpecialFrameCreate>(frame, "SpecialFrameRoot");
        }

        private void SetDialogUIData()
        {
            ProtoTable.NpcTable npcItem = TableManager.instance.GetTableItem<ProtoTable.NpcTable>(talkItem.NpcID);
            //设置NPCBodyICON
            if(npcItem != null && talkItem.NpcID != playerNpcId)
            {
                _ShowModule(npcItem.NpcBody);
            }

            if (goAnimation != null)
            {
                GameObject.Destroy(goAnimation);
                goAnimation = null;
            }
            comAnimation = null;

            //if (imgNpcIcon.sprite == null)
            //{
            //    imgNpcIcon.gameObject.CustomActive(false);
            //    goAnimation = AssetLoader.instance.LoadResAsGameObject(npcItem.NpcBody);
            //    if (goAnimation != null)
            //    {
            //        Utility.AttachTo(goAnimation, goNpcAnimationParent);

            //        if (goAnimation.transform.childCount > 0)
            //        {
            //            comAnimation = goAnimation.transform.GetChild(0).GetComponent<Animation>();
            //            if (comAnimation != null && !comAnimation.isPlaying)
            //            {
            //                comAnimation.Play();
            //            }
            //        }
            //    }
            //}

            if (taskName != null)
            {
                taskName.gameObject.CustomActive(false);
            }

            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iCurTaskId);

            var nextTalkItem = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(talkItem.NextID);
            if(nextTalkItem == null)
            {
                if (taskName != null)
                {
                    if (missionItem != null)
                    {
                        taskName.text = missionItem.TaskName;
                        taskName.gameObject.CustomActive(true);
                    }
                }
            }

            if(talkItem != null)
            {
                if (comSpecialFrameCreate != null && missionItem != null)
                {
                    comSpecialFrameCreate.Param0 = missionItem.ID.ToString();
                    comSpecialFrameCreate.ClsName = talkItem.AttachClassName;
                }
            }

            //设置对话NPCName
            if(npcName != null && npcItem != null)
            {
                npcName.text = npcItem.NpcName.Replace("[UserName]", PlayerBaseData.GetInstance().Name);
            }
            //设置对话内容
            if(talkContent != null)
            {
                talkContent.text = talkItem.TalkText.Replace("[UserName]", PlayerBaseData.GetInstance().Name);
            }
            //重新设置半身像为特殊对话
            //2056 表示的是玩家自己
            if(talkItem != null && talkItem.NpcID == playerNpcId)
            {
                var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
                if (jobItem != null)
                {
                    _ShowModule(jobItem.JobImage, true);
                }
            }
            //更新安扭状态
            Int32 iNextStepCount = _CheckNextStepCount(talkItem.NextID);
            if (iNextStepCount > 0)
            {
                frame.transform.Find("BtnNext").gameObject.SetActive(true);
                frame.transform.Find("BtnComplete").gameObject.SetActive(false);
                frame.transform.Find("BtnStepOver").gameObject.SetActive(iNextStepCount > 1);
            }
            else
            {
                frame.transform.Find("BtnNext").gameObject.SetActive(false);
                GameObject goBtnComplete = frame.transform.Find("BtnComplete").gameObject;
                goBtnComplete.SetActive(true);

                bool bAcceptTask = talkItem.TakeFinish == 0;
                if(bAcceptTask)
                {
                    Utility.SetChildTextContent(goBtnComplete.transform, "Text", "接受");
                }
                else
                {
                    Utility.SetChildTextContent(goBtnComplete.transform, "Text", "完成");
                }

                frame.transform.Find("BtnStepOver").gameObject.SetActive(false);
            }
        }

        [UIEventHandle("BtnNext")]
        void OnClickNext()
        {
            if(!bInteractable)
            {
                //Logger.LogErrorFormat("can not be click bInteractable = {0}", bInteractable);
                return;
            }

            if(talkItem != null)
            {
                if (talkItem.ID == talkItem.NextID)
                {
                    GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, talkItem.NextID, GameStatisticManager.DialogOperateType.DOT_NEXT, GameStatisticManager.DialogFinishType.DFT_FINISH);
                    OnStepOver(talkItem);
                    frameMgr.CloseFrame(this);
                    Logger.LogError("talkItem.ID == talkItem.NextID ?");
                    return;
                }

                var nextTalkItem = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(talkItem.NextID);
                if (nextTalkItem != null)
                {
                    TaskDialogFrame dialogFrame = MissionManager.GetInstance().GetDlgFrameByName(nextTalkItem.ObjectName);

                    if (dialogFrame != null)
                    {
                        GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, talkItem.NextID, GameStatisticManager.DialogOperateType.DOT_NEXT, GameStatisticManager.DialogFinishType.DFT_RESTAR);
                        dialogFrame.OnRestart(nextTalkItem, iCurTaskId);
                    }
                    else
                    {
                        GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, talkItem.NextID, GameStatisticManager.DialogOperateType.DOT_NEXT, GameStatisticManager.DialogFinishType.DFT_NEWCREATE);
                        MissionManager.GetInstance().CreateDialogFrame(talkItem.NextID, iCurTaskId, onDialogOver);
                    }
                }
                else
                {
                    GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, talkItem.NextID, GameStatisticManager.DialogOperateType.DOT_NEXT, GameStatisticManager.DialogFinishType.DFT_FINISH);
                    OnStepOver(talkItem);

                    GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.FinishTalkDialog, dlgId);
                    frameMgr.CloseFrame(this);
                }
            }
            else
            {
                Logger.LogError("talkItem == null ? how this happened?");
            }
        }

        public void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if(comAnimation != null && !comAnimation.isPlaying)
            {
                comAnimation.Play();
            }
        }

        private void OnStepOver(ProtoTable.TalkTable item)
        {
            if (onDialogOver != null)
            {
                onDialogOver.Invoke();
                onDialogOver.RemoveAllListeners();
                onDialogOver = null;
            }

            if(!string.IsNullOrEmpty(item.FrameClassName))
            {
                Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                string parserClassName = string.Format("GameClient.{0}", item.FrameClassName);
                Type type = assembly.GetType(parserClassName);
                var method = type.GetMethod("Open", BindingFlags.Static | BindingFlags.Public);
                if(method != null)
                {
                    method.Invoke(null,null);
                }
            }

            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iCurTaskId);
            if(missionItem == null)
            {
                return;
            }

            if(iCurTaskId != item.MissionID)
            {
                Logger.LogWarning("iCurTaskId != item.MissionID!");
            }

            Logger.LogWarning(string.Format("对话类型为TakeFinish = {0}",item.TakeFinish));
            bool bAcceptTask = item.TakeFinish == 0;
            if(bAcceptTask)
            {
                MissionManager.GetInstance().OnExecuteAcceptTask(iCurTaskId,false);
            }
            else
            {
                Logger.LogWarningFormat("OpenAwardFrame iCurTaskId = {0}", iCurTaskId);
                if (Utility.IsNormalMission((uint)iCurTaskId))
                {
                    var missionValue = MissionManager.GetInstance().GetMission((uint)iCurTaskId);
                    if (null != missionValue)
                    {
                        if(missionValue.status == (int)TaskStatus.TASK_FINISHED)
                        {
                            MissionManager.GetInstance().sendCmdSubmitTask((uint)iCurTaskId, (TaskSubmitType)missionItem.FinishType, (uint)missionItem.MissionFinishNpc);
                        }
                        else if (missionValue.status == (int)TaskStatus.TASK_INIT)
                        {
                            // 这里做一个容错机制,在某些情况下，任务状态错了的时候，再重新发一下任务接取
                            // 比如玩家任务满20个的时候,后来又触发了下一个自动接取的任务，但是由于个数限制导致自动接取失败，玩家又无法手动接取，就造成了永远接取不了的情况
                            MissionManager.GetInstance().OnExecuteAcceptTask(iCurTaskId, false);
                        }
                    }
                }
                //MissionManager.GetInstance().OpenAwardFrame((uint)iCurTaskId);

                //GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.MissionRewardFrameClose, iCurTaskId);
            }
        }

        [UIEventHandle("BtnStepOver")]
        void OnClickStepOver()
        {
            if (!bInteractable)
            {
                //Logger.LogErrorFormat("can not be click bInteractable = {0}", bInteractable);
                return;
            }
            /*
            Int32 iNextStepCount = _CheckNextStepCount(talkItem.NextID);
            if(iNextStepCount > 0)
            {
                _GoNextNStep(iNextStepCount);
            }
            else*/
            {
                GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, 0, GameStatisticManager.DialogOperateType.DOT_JUMPOVER, GameStatisticManager.DialogFinishType.DFT_FINISH);
                OnStepOver(talkItem);
                frameMgr.CloseFrame(this);
            }
        }

        [UIEventHandle("BtnComplete")]
        void OnClickComplete()
        {
            GameStatisticManager.GetInstance().DoStatDialog(talkItem.ID, 0, GameStatisticManager.DialogOperateType.DOT_COMPLETE, GameStatisticManager.DialogFinishType.DFT_FINISH);
            OnClickNext();
        }

        private Int32 _CheckNextStepCount(Int32 iNextTalkID)
        {
            Int32 iCount = 0;
            ProtoTable.TalkTable nextTalkItem = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(iNextTalkID);
            while(nextTalkItem != null)
            {
                ++iCount;
                nextTalkItem = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(nextTalkItem.NextID);
                if(iCount > 100000)
                {
                    Logger.LogErrorFormat("TalkTable is Error dialogid list existed death loop which first id is {0},that is to say a->next is b b->next is c then c->next is a death loop!", iNextTalkID);
                    iCount = 1;
                    break;
                }
            }
            return iCount;
        }

        private void _GoNextNStep(Int32 iGoTimes)
        {
            ProtoTable.TalkTable talkFinishPre = talkItem;
            ProtoTable.TalkTable talkCurStep = talkItem;
            while (iGoTimes > 0)
            {
                if (talkItem != null)
                {
                    if(talkItem.ID == talkItem.NextID)
                    {
                        Logger.LogError("talkItem.ID == talkItem.NextID ?");
                        break;
                    }
                    talkFinishPre = talkCurStep;
                    talkCurStep = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(talkCurStep.NextID);
                }
                else
                {
                    break;
                }
                iGoTimes--;
            }

            if(talkCurStep.ObjectName != talkFinishPre.ObjectName)
            {
                TaskDialogFrame talkPreDlgFrame = MissionManager.GetInstance().GetDlgFrameByName(talkFinishPre.ObjectName);
                if(talkPreDlgFrame != null)
                {
                    talkPreDlgFrame.OnRestart(talkFinishPre,iCurTaskId);
                }
                else
                {
                    MissionManager.GetInstance().CreateDialogFrame(talkFinishPre.ID, iCurTaskId,onDialogOver);
                }
            }

            TaskDialogFrame dialogFrame = MissionManager.GetInstance().GetDlgFrameByName(talkCurStep.ObjectName);
            if (dialogFrame != null)
            {
                dialogFrame.OnRestart(talkCurStep, iCurTaskId);
            }
            else
            {
                MissionManager.GetInstance().CreateDialogFrame(talkCurStep.ID, iCurTaskId,onDialogOver);
            }
        }

        void _ShowModule(string path, bool isPlayer = false)
        {
            if (objRender != null)
            {
                objRender.gameObject.CustomActive(true);
                if (!objRender.IsSpinePathEquals(path))
                {
                    objRender.ClearObject();
                }

                try
                {
                    if (isPlayer)
                    {
                        objRender.SetCameraPos(objRender.m_CameraPos + playerCameraPosDiff);
                    }

                    if (!objRender.IsSpinePathEquals(path))
                    {
                        objRender.LoadObject(path, 28);
                        objRender.AddAni(aniNames);
                    }
                    //setSpineAnimatin();
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("create spineModule failed: {0}", e.ToString());
                }
            }
        }

        void _HideModule()
        {
            if (objRender != null)
            {
                objRender.gameObject.CustomActive(false);
            }
        }

        public void OnRestart(ProtoTable.TalkTable talkItem,int iCurTaskId)
        {
            OnReset();
            this.iCurTaskId = iCurTaskId;
            dlgId = talkItem.ID;        //这里不要去设置initialdlgid
            this.talkItem = talkItem;
            AttachUIObject();
            SetDialogUIData();
        }
    }
}