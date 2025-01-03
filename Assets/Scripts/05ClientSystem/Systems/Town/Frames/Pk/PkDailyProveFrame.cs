using System;
using System.Collections.Generic;
///////删除linq
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    public class PkDailyProveFrame : ClientFrame
    {
        const int DailyProveNum = 2;
        ComItem[] ComItems = new ComItem[DailyProveNum];
        List<MissionManager.SingleMissionInfo> DailyProveList = new List<MissionManager.SingleMissionInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkDailyProveFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.DailyProve);
        }

        void ClearData()
        {
            MissionManager.GetInstance().onUpdateMission -= OnUpdateDailyProve;
            DailyProveList.Clear();

            for (int i = 0; i < ComItems.Length; ++i)
            {
                ComItems[i] = null;
            }
        }

        protected void BindUIEvent()
        {
        }

        protected void UnBindUIEvent()
        {
        }

        [UIEventHandle("middle/title/btClose")]
        void OnClose()
        {          
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("middle/greenback/Daily{0}/btReceive", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 1, DailyProveNum)]
        void OnReceiveAwards(int iIndex)
        {
            if (iIndex < 0 || iIndex >= DailyProveList.Count)
            {
                return;
            }

            MissionManager.GetInstance().sendCmdSubmitTask(DailyProveList[iIndex].taskID, TaskSubmitType.TASK_SUBMIT_UI, 0);
        }

        void InitInterface()
        {
            MissionManager.GetInstance().onUpdateMission += OnUpdateDailyProve;

            for(int i = 0; i < btReceive.Length; i++)
            {
                ComItem ShowItem = CreateComItem(pos[i].gameObject);
                ComItems[i] = ShowItem;

                btReceive[i].gameObject.AddComponent<UIGray>();
                btReceive[i].gameObject.GetComponent<UIGray>().enabled = true;
                btReceive[i].interactable = false;
            }

            var missionList = MissionManager.GetInstance().taskGroup.Values.ToList();

            for (int i = 0; i < missionList.Count; ++i)
            {
                MissionTable missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)missionList[i].taskID);

                if (missionItem == null || missionItem.TaskType != MissionTable.eTaskType.TT_DIALY || missionItem.SubType != MissionTable.eSubType.Daily_Prove)
                {
                    continue;
                }

                DailyProveList.Add(missionList[i]);
            }

            UpdateDailyProveInterface();
        }

        void UpdateDailyProveInterface()
        {
            for(int i = 0; i < DailyProveList.Count && i < DailyProveNum; i++)
            {
                MissionTable missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)DailyProveList[i].taskID);

                if (missionItem == null)
                {
                    continue;
                }

                Description[i].text = Utility.ParseMissionText((int)DailyProveList[i].taskID, true);

                var awards = missionItem.Award.Split(new char[] { ',' });

                if (awards.Length > 0)
                {
                    var award = awards[0].Split(new char[] { '_' });
                    if (award.Length == 2)
                    {
                        ItemData data = ItemDataManager.CreateItemDataFromTable(int.Parse(award[0]));
                        if (data != null)
                        {
                            ComItems[i].Setup(data, null);
                        }

                        num[i].text = award[1];
                    }
                }

                if (DailyProveList[i].status >= (int)TaskStatus.TASK_OVER)
                {
                    //finish[i].gameObject.SetActive(true);
                    btReceive[i].gameObject.SetActive(false);

                    Description[i].text += "<color=#01BC47FF>  (已完成)</color>";
                }
                else
                {
                    //finish[i].gameObject.SetActive(false);
                    btReceive[i].gameObject.SetActive(true);

                    if (DailyProveList[i].status == (int)TaskStatus.TASK_FINISHED)
                    {
                        btReceive[i].gameObject.GetComponent<UIGray>().enabled = false;
                        btReceive[i].interactable = true;
                    }
                    else
                    {
                        btReceive[i].gameObject.GetComponent<UIGray>().enabled = true;
                        btReceive[i].interactable = false;
                    }
                }
            }        
        }

        public void OnUpdateDailyProve(UInt32 iMissionID)
        {
            if (!Utility.IsDailyProve(iMissionID))
            {
                return;
            }

            UpdateDailyProveData(iMissionID);
            UpdateDailyProveInterface();
        }

        void UpdateDailyProveData(UInt32 iMissionID)
        {
            MissionManager.SingleMissionInfo kSingleMissionInfo = null;

            if (!MissionManager.GetInstance().taskGroup.TryGetValue(iMissionID, out kSingleMissionInfo))
            {
                return;
            }

            for(int i = 0; i < DailyProveList.Count; i++)
            {
                if(DailyProveList[i].taskID != iMissionID)
                {
                    continue;
                }

                DailyProveList[i].status = kSingleMissionInfo.status;
                DailyProveList[i].taskContents = kSingleMissionInfo.taskContents;

                break;
            }
        }

        [UIControl("middle/greenback/Daily{0}/Description", typeof(Text), 1)]
        protected Text[] Description = new Text[DailyProveNum];

//         [UIControl("middle/greenback/Daily{0}/finish", typeof(Text), 1)]
//         protected Text[] finish = new Text[DailyProveNum];

        [UIControl("middle/greenback/Daily{0}/award/pos", typeof(RectTransform), 1)]
        protected RectTransform[] pos = new RectTransform[DailyProveNum];

        [UIControl("middle/greenback/Daily{0}/award/num", typeof(Text), 1)]
        protected Text[] num = new Text[DailyProveNum];

        [UIControl("middle/greenback/Daily{0}/btReceive", typeof(Button), 1)]
        protected Button[] btReceive = new Button[DailyProveNum];
    }
}
