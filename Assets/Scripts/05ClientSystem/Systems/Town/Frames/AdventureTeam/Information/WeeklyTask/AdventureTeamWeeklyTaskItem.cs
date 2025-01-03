using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{
    public class AdventureTeamWeeklyTaskModule<T> : MonoBehaviour where T:ClientFrame,new() 
    {
        MissionManager.SingleMissionInfo value;
        T frame;

        List<ComItem> akComItems = new List<ComItem>();
        List<ComItem> akCachedItems = new List<ComItem>();

        public Text Desc;
        public GameObject awardParent;
        public UIGray gray;
        public Button award;
        public Button go;
        public Image complete;
        public Image acquired;
        public Image Icon;
        public Text Difficult;

        void _RecycleComItems()
        {
            for (int i = 0; i < akComItems.Count; ++i)
            {
                akComItems[i].Setup(null, null);
                akComItems[i].CustomActive(false);
                akCachedItems.Add(akComItems[i]);
            }
            akComItems.Clear();
        }

        void _OnItemClicked(GameObject obj,ItemData item)
        {
            if(item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        void OnClickAward()
        {
            if (AdventureTeamDataManager.GetInstance()._GetFinishedWeeklyTaskNum() < AdventureTeamDataManager.GetInstance().ADTMissionFinishMaxNum) 
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(
                    TR.Value("adventure_team_weekly_task_submit_tip"),
                    FinishTask);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("adventure_team_weekly_task_cant_submit", AdventureTeamDataManager.GetInstance().ADTMissionFinishMaxNum));
            }
        }

        void FinishTask()
        {
            MissionManager.GetInstance().FinishAccountTaskReq(value.missionItem.ID);
        }

        void OnClickGo()
        {
            if(value.missionItem != null)
            {
                ActiveManager.GetInstance().OnClickLinkInfo(value.missionItem.LinkInfo);
            }
        }

        public void Init(MissionManager.SingleMissionInfo data, ClientFrame clientFrame)
        {
            value = data;
            frame = clientFrame as T;
            var missionItem = value.missionItem;

            if(missionItem == null)
            {
                return;
            }
            if(award != null)
            {
                award.onClick.AddListener(OnClickAward);
            }
            if(go!= null)
            {
                go.onClick.AddListener(OnClickGo);
            }
            
            gameObject.name = missionItem.ID.ToString();

            _RecycleComItems();

            //任务描述
            if (Desc != null)
            {
                Desc.text = Utility.ParseMissionText(missionItem.ID, true);
            }
            //图标
            if(Icon != null&& !string.IsNullOrEmpty(missionItem.Icon))
            {
                ETCImageLoader.LoadSprite(ref Icon, missionItem.Icon);
                Icon.SetNativeSize();
            }
            //难度
            if(Difficult != null)
            {
                int i = 0;
                int.TryParse(missionItem.MissionParam, out i);
                Difficult.text = ((AdventureTeamTaskDifficult)i).ToString();
            }
            var awards = MissionManager.GetInstance().GetMissionAwards(missionItem.ID);
            for (int i = 0; i < awards.Count; ++i) 
            {
                var award = awards[i];
                ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ItemTable>(award.ID);
                if (itemInfo != null)
                {
                    var ItemData = ItemDataManager.CreateItemDataFromTable(award.ID);
                    if(ItemData != null)
                    {
                        ItemData.Count = award.Num;

                        ComItem comItem = null;
                        if(akCachedItems.Count > 0)
                        {
                            comItem = akCachedItems[0];
                            akCachedItems.RemoveAt(0);
                        }
                        else
                        {
                            comItem = frame.CreateComItem(awardParent);
                        }
                        if(comItem != null)
                        {
                            comItem.CustomActive(true);
                            comItem.Setup(ItemData, _OnItemClicked);
                            comItem.transform.SetSiblingIndex(i + 1);
                            akComItems.Add(comItem);
                        }
                    }
                }
            }
            //状态
            bool bAcquired = value.status == (int)Protocol.TaskStatus.TASK_OVER;
            bool bCanSendAcquire = false;
            if (bAcquired)
            {
                complete.CustomActive(true);
                award.CustomActive(false);
                go.CustomActive(false);
                acquired.CustomActive(true);
            }
            else
            {
                complete.CustomActive(false);
                acquired.CustomActive(false);

                bool bNeedShowGO = missionItem != null && missionItem.TaskFinishType == MissionTable.eTaskFinishType.TFT_LINKS &&
                    !string.IsNullOrEmpty(missionItem.LinkInfo) && value.status == (int)Protocol.TaskStatus.TASK_UNFINISH;

                award.CustomActive(!bNeedShowGO);
                go.CustomActive(bNeedShowGO);

                bCanSendAcquire = value.status == (int)Protocol.TaskStatus.TASK_FINISHED;
            }

            award.enabled = !bAcquired && bCanSendAcquire;

            bool isAccquireNumMax =
                AdventureTeamDataManager.GetInstance()._GetFinishedWeeklyTaskNum() >= AdventureTeamDataManager.GetInstance().ADTMissionFinishMaxNum;
            gray.enabled = isAccquireNumMax;
        }
    }

    

    class AdventureTeamWeeklyTaskItem : AdventureTeamWeeklyTaskModule<AdventureTeamInformationFrame>
    {

    }
}