using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class ComAchievementScript : MonoBehaviour
    {
        MissionManager.SingleMissionInfo value;
        MissionFrameNew frame;

        List<ComItem> akComItems = new List<ComItem>();
        List<ComItem> akCachedItems = new List<ComItem>();

        public Text Name;
        public Text Desc;
        public GameObject awardParent;

        public Image progressImage;
        public Slider slider;
        public Text progressText;
        public GameObject progress;

        public UIGray gray;
        public Button award;
        public Button go;
        public Image complete;

        void OnDestroy()
        {
            Name = null;
            Desc = null;
            awardParent = null;

            progressImage = null;
            if(slider != null)
            {
                slider.onValueChanged.RemoveAllListeners();
                slider = null;
            }

            progressText = null;
            progress = null;

            gray = null;
            if(award != null)
            {
                award.onClick.RemoveAllListeners();
                award = null;
            }

            if(go != null)
            {
                go.onClick.RemoveAllListeners();
                go = null;
            }

            complete = null;
        }

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

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }

        void OnClickAward()
        {
            MissionManager.GetInstance().sendCmdSubmitTask((uint)value.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
        }

        void OnClickGo()
        {
            if (value.missionItem != null)
            {
                ActiveManager.GetInstance().OnClickLinkInfo(value.missionItem.LinkInfo);
            }
        }

        public void OnVisible(MissionManager.SingleMissionInfo data, ClientFrame clientFrame)
        {
            value = data;
            frame = clientFrame as MissionFrameNew;
            var missionItem = value.missionItem;

            award.onClick.RemoveAllListeners();
            award.onClick.AddListener(OnClickAward);

            go.onClick.RemoveAllListeners();
            go.onClick.AddListener(OnClickGo);

            gameObject.name = missionItem.ID.ToString();
            //设置任务名称
            Name.text = missionItem.TaskName;
            //设置任务描述
            Desc.text = Utility.ParseMissionText(missionItem.ID, true);
            //设置奖励
            _RecycleComItems();
            var awards = MissionManager.GetInstance().GetMissionAwards(missionItem.ID);
            for (int i = 0; i < awards.Count; ++i)
            {
                var awardR = awards[i];
                ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(awardR.ID);
                if (itemInfo != null)
                {
                    var ItemData = GameClient.ItemDataManager.CreateItemDataFromTable(awardR.ID);
                    if (ItemData != null)
                    {
                        ItemData.Count = awardR.Num;

                        ComItem comItem = null;
                        if (akCachedItems.Count > 0)
                        {
                            comItem = akCachedItems[0];
                            akCachedItems.RemoveAt(0);
                        }
                        else
                        {
                            comItem = frame.CreateComItem(awardParent);
                        }
                        if (comItem != null)
                        {
                            comItem.CustomActive(true);
                            comItem.Setup(ItemData, _OnItemClicked);
                            akComItems.Add(comItem);
                        }
                    }
                }
            }

            //设置进度与完成状态
            bool bAcquired = value.status == (int)Protocol.TaskStatus.TASK_OVER;
            bool bCanSendAcquire = false;
            if (bAcquired)
            {
                complete.CustomActive(true);
                award.CustomActive(false);
                progress.CustomActive(false);
                go.CustomActive(false);
                //设置完成图标
            }
            else
            {
                complete.CustomActive(false);
                bool bNeedShowGo = missionItem != null && missionItem.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_LINKS &&
                    !string.IsNullOrEmpty(missionItem.LinkInfo) && value.status == (int)Protocol.TaskStatus.TASK_UNFINISH;
                award.CustomActive(!bNeedShowGo);
                go.CustomActive(bNeedShowGo);
                progress.CustomActive(true);

                //设置完成进度
                var contentProcess = Utility.ParseMissionProcess(missionItem.ID, true);
                if (contentProcess != null)
                {
                    if (contentProcess.iPreValue > contentProcess.iAftValue)
                    {
                        contentProcess.iPreValue = contentProcess.iAftValue;
                    }
                    slider.value = contentProcess.fAmount;
                    progressText.text = contentProcess.iPreValue + "/" + contentProcess.iAftValue;
                    bCanSendAcquire = value.status == (int)Protocol.TaskStatus.TASK_FINISHED;
                }
            }

            award.enabled = !bAcquired && bCanSendAcquire;
            gray.enabled = !award.enabled;
        }
    }
}