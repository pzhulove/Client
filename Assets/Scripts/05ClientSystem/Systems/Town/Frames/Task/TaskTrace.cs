using System;
using System.Text.RegularExpressions;

namespace GameClient
{
    public delegate void OnReached();

    namespace TaskTrace
    {
        class DungenTrace
        {
            public Int32 iDungenID = 0;
            public Int32 iTaskID = 0;
            public UnityEngine.Events.UnityAction onSucceed = null;
            public UnityEngine.Events.UnityAction onFailed = null;

            public void OnMoveStateChanged(BeTownPlayerMain.EMoveState ePre, BeTownPlayerMain.EMoveState eCur)
            {
            }

            public void OnMoveSuccess()
            {
                GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;

                if (clientSystem != null)
                {
                    BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                    BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                    BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                }

                OnTriggerDungen();

                if (onSucceed != null)
                {
                    onSucceed.Invoke();
                    onSucceed = null;
                }
                onFailed = null;
            }

            public void OnAutoMoveFail()
            {
                GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
                if (clientSystem != null && clientSystem.MainPlayer != null)
                {
                    BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                    BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                    BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                }

                if (onFailed != null)
                {
                    onFailed.Invoke();
                    onFailed = null;
                }
                onSucceed = null;
            }

            void OnTriggerDungen()
            {
                ProtoTable.DungeonTable dungeonItem = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonTable>(iDungenID);
                if(dungeonItem != null)
                {
                    ChapterSelectFrame.SetDungeonID(iDungenID);
                }
            }
        }
        class NpcTrace
        {
            public Int32 iNpcID = 0;
            public Int32 iTaskID = 0;
            public bool bNeedDialog = false;
            public OnReached onReached = null;
            public UnityEngine.Events.UnityAction onSucceed = null;
            public UnityEngine.Events.UnityAction onFailed = null;

            public void OnMoveStateChanged(BeTownPlayerMain.EMoveState ePre, BeTownPlayerMain.EMoveState eCur)
            {
            }

            public void OnMoveSuccess()
            {
                GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
                if (clientSystem != null && clientSystem.MainPlayer != null)
                {
                    BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                    BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                    BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                }

                if(onReached != null)
                {
                    onReached.Invoke();
                    onReached = null;
                }
                else
                {
                    OnTriggerNpc();
                }

                if(onSucceed != null)
                {
                    onSucceed.Invoke();
                    onSucceed = null;
                }

                onFailed = null;
            }

            public void OnAutoMoveFail()
            {
                GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
                if (clientSystem != null && clientSystem.MainPlayer != null)
                {
                    BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(this.OnMoveSuccess);
                    BeTownPlayerMain.OnAutoMoveFail.RemoveListener(this.OnAutoMoveFail);
                    BeTownPlayerMain.OnMoveStateChanged.RemoveListener(this.OnMoveStateChanged);
                }

                if(onFailed != null)
                {
                    onFailed.Invoke();
                    onFailed = null;
                }
                onSucceed = null;
                onReached = null;
            }

            void OnTriggerNpc()
            {
                Logger.LogWarning("I Have Enter The Npc Zone!");
                ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
                if(missionItem == null)
                {
                    Logger.LogErrorFormat("MissionID is wrong MissionID = {0}", iTaskID);
                    return;
                }

                MissionManager.SingleMissionInfo singleInfo = null;
                if (!MissionManager.GetInstance().taskGroup.TryGetValue((uint)missionItem.ID, out singleInfo))
                {
                    return;
                }

                if (singleInfo.status == (int)Protocol.TaskStatus.TASK_INIT)
                {
                    if (missionItem.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_NPC && iNpcID == missionItem.MissionTakeNpc)
                    {
                        ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.BefTaskDlgID);
                        if (bNeedDialog && talkItem != null)
                        {
                            TaskDialogFrame.OnDialogOver onDialogOver = null;
                            ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if(clientSystem != null)
                            {
                                clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_Start);
                                onDialogOver = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                                {
                                    clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_End);
                                });
                            }
                            MissionManager.GetInstance().CloseAllDialog();
                            MissionManager.GetInstance().CreateDialogFrame(missionItem.BefTaskDlgID, iTaskID, onDialogOver);
                        }
                        else
                        {
                            MissionManager.GetInstance().sendCmdAcceptTask((uint)iTaskID, (Protocol.TaskSubmitType)missionItem.AcceptType, (uint)missionItem.MissionTakeNpc);
                        }
                    }
                    return;
                }

                if (singleInfo.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
                {
                    if(missionItem.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_ACCESS_SHOP)
                    {
                        var npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
                        if(npcItem != null && npcItem.Function == ProtoTable.NpcTable.eFunction.shopping)
                        {
                            if (npcItem.FunctionIntParam.Count > 0)
                            {
                                var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(npcItem.FunctionIntParam[0]);
                                if(shopItem != null)
                                {
                                    int itemID = 0;
                                    Regex regex = new Regex(@"<a href=item>\[(\d+)\]</a>", RegexOptions.Singleline);
                                    foreach(Match match in regex.Matches(missionItem.TaskFinishText))
                                    {
                                        if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                                        {
                                            if (!int.TryParse(match.Groups[1].Value, out itemID))
                                            {
                                                itemID = 0;
                                            }
                                            break;
                                        }
                                    }
                                    ShopDataManager.GetInstance().OpenShop(shopItem.ID,itemID);
                                }
                            }
                        }
                        return;
                    }

                    ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.BefTaskDlgID);
                    if (bNeedDialog && talkItem != null)
                    {
                        TaskDialogFrame.OnDialogOver onDialogOver = null;
                        ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (clientSystem != null)
                        {
                            clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_Start);
                            onDialogOver = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                            {
                                clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_End);
                            });
                        }
                        MissionManager.GetInstance().CloseAllDialog();
                        MissionManager.GetInstance().CreateDialogFrame(missionItem.BefTaskDlgID, iTaskID, onDialogOver);
                    }
                    /*
                    else
                    {
                        MissionManager.GetInstance().sendCmdAcceptTask((uint)iTaskID, (Protocol.TaskSubmitType)missionItem.AcceptType, (uint)missionItem.MissionTakeNpc);
                    }
                    */
                }

                if (singleInfo.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    if (missionItem.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC && iNpcID == missionItem.MissionFinishNpc)
                    {
                        ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(missionItem.AftTaskDlgID);
                        if (bNeedDialog && talkItem != null)
                        {
                            TaskDialogFrame.OnDialogOver onDialogOver = null;
                            ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                            if (clientSystem != null)
                            {
                                clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_Start);
                                onDialogOver = new TaskDialogFrame.OnDialogOver().AddListener(() =>
                                {
                                    clientSystem.PlayNpcSound(iNpcID, NpcVoiceComponent.SoundEffectType.SET_End);
                                });
                            }
                            MissionManager.GetInstance().CloseAllDialog();
                            MissionManager.GetInstance().CreateDialogFrame(missionItem.AftTaskDlgID, iTaskID, onDialogOver);
                        }
                        else
                        {
                            MissionManager.GetInstance().OpenAwardFrame((uint)iTaskID);
                        }
                    }
                    return;
                }
            }
        }
    }
}