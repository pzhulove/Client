using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComAchievementSubItem : MonoBehaviour
    {
        public Image icon;
        public Text acPoint;
        public Text itemName;
        public Text itemDesc;
        public string processFmt;
        public string finishTimeFmt;
        public string unlockHintFmt;
        public Text process;
        public Text fnishTime;
        public Slider slider;
        public GameObject[] parents = new GameObject[0];
        public StateController state;
        public Image pointIcon = null;

        ComItem[] comItems = null;

        ProtoTable.AchievementGroupSubItemTable value = null;

        void Awake()
        {

        }

        void OnDestroy()
        {
            if(null != comItems)
            {
                for(int i = 0; i < comItems.Length; ++i)
                {
                    ComItemManager.Destroy(comItems[i]);
                }
                comItems = null;
            }
        }

        public void OnClickLink()
        {
            var subItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSubItemTable>(value.ID);
            if(null != subItem)
            {
                var FuncUnlockData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(value.FunctionID);
                int unLockLevel = null == FuncUnlockData ? 0 : FuncUnlockData.FinishLevel;
                bool isUnlock = Utility.IsFunctionCanUnlock((ProtoTable.FunctionUnLock.eFuncType)value.FunctionID);
                if(!isUnlock)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(unlockHintFmt, unLockLevel),ProtoTable.CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    return;
                }
                ActiveManager.GetInstance().OnClickLinkInfo(subItem.LinkInfo);
            }
        }

        public void OnClickShare()
        {
            if(null != this.value)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnShareAchievementItem, this.value);
            }
        }

        public void OnClickAcquired()
        {
            if (null != value)
            {
                var missionValue = MissionManager.GetInstance().GetMission((uint)value.ID);
                if(null != missionValue)
                {
                    MissionManager.GetInstance().sendCmdSubmitTask((uint)missionValue.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
                }
            }
        }

        public void OnItemVisible(ProtoTable.AchievementGroupSubItemTable value)
        {
            if (null == comItems)
            {
                comItems = new ComItem[parents.Length];
                for (int i = 0; i < parents.Length; ++i)
                {
                    comItems[i] = ComItemManager.Create(parents[i]);
                }
            }

            this.value = value;
            if(null != value)
            {                
                icon.SafeSetImage(value.Icon, true);
                pointIcon.SafeSetImage(value.PointIcon,true);

                var missionValue = MissionManager.GetInstance().GetMission((uint)value.ID);
                var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(value.ID);

                if (null != acPoint && null != missionItem)
                {
                    acPoint.text = missionItem.IntParam0.ToString();
                }

                if (null != itemName)
                {
                    if(null != missionItem)
                    {
                        itemName.text = missionItem.TaskName;
                    }
                }

                if(null != itemDesc)
                {
                    itemDesc.text = Utility.ParseMissionText(value.ID, true);
                }


                //设置完成进度
                var contentProcess = Utility.ParseMissionProcess(value.ID, true);
                if (null != contentProcess)
                {
                    if (null != process)
                    {
                        process.text = string.Format(processFmt, contentProcess.iPreValue, contentProcess.iAftValue);
                    }
                    if (null != slider)
                    {
                        slider.value = contentProcess.fAmount;
                    }
                }

                //设置奖励
                var datas = MissionManager.GetInstance().GetMissionAwards(value.ID);
                int count = null == datas ? 0 : datas.Count;
                for(int i = 0; i < comItems.Length && i < parents.Length; ++i)
                {
                    parents[i].CustomActive(i < count);

                    if(i < count)
                    {
                        var awardData = datas[i];
                        if(null != awardData)
                        {
                            var itemData = ItemDataManager.CreateItemDataFromTable(awardData.ID);
                            itemData.Count = awardData.Num;
                            comItems[i].Setup(itemData, (go, item) => 
                            {
                                ItemTipManager.GetInstance().CloseAll();
                                ItemTipManager.GetInstance().ShowTip(item);
                            });
                        }
                        else
                        {
                            comItems[i].Setup(null, null);
                        }
                    }
                    else
                    {
                        comItems[i].Setup(null, null);
                    }
                }

                int status = (int)Protocol.TaskStatus.TASK_INIT;
                if(null != missionValue)
                {
                    status = missionValue.status;
                }

                //设置完成时间
                if(null != fnishTime)
                {
                    if(!string.IsNullOrEmpty(finishTimeFmt))
                    {
                        var dateTime = Function.ConvertIntDateTime(missionValue.finTime);
                        fnishTime.text = dateTime.ToString(finishTimeFmt, System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    }
                }

                //设置状态
                if(null != state)
                {
                    if (status == (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        state.Key = "finished";
                    }
                    else if(status == (int)Protocol.TaskStatus.TASK_OVER)
                    {
                        state.Key = "share";
                    }
                    else if(status == (int)Protocol.TaskStatus.TASK_UNFINISH)
                    {
                        if(!string.IsNullOrEmpty(value.LinkInfo))
                        {
                            state.Key = "go";
                        }
                        else
                        {
                            if (null != contentProcess && contentProcess.iPreValue > 0)
                            {
                                state.Key = "running";
                            }
                            else
                            {
                                state.Key = "unstart";
                            }
                        }
                    }
                    else if (status == (int)Protocol.TaskStatus.TASK_INIT)
                    {
                        if (!string.IsNullOrEmpty(value.LinkInfo))
                        {
                            state.Key = "go";
                        }
                        else
                        {
                            state.Key = "unstart";
                        }
                    }
                    else
                    {
                        state.Key = "unstart";
                    }
                }
            }
        }
    }
}