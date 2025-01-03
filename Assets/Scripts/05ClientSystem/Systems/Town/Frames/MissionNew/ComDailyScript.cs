using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
    class ComDailyScriptTemplate<T> : MonoBehaviour where T : ClientFrame, new()
    {
        MissionManager.SingleMissionInfo value;
        T frame;

        List<ComItemNew> akComItems = new List<ComItemNew>();
        List<ComItemNew> akCachedItems = new List<ComItemNew>();

        public Text Name;
        public Text Desc;
        public GameObject awardParent;
        public UIGray gray;
        public Button award;
        public Button go;
        public Image complete;
        public Image acquired;
        public Text score;
        public Text VitalityValue;
        public Image Icon;

        void OnDestroy()
        {
            Name = null;
            Desc = null;
            awardParent = null;

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

        void _OnItemClicked(GameObject obj, IItemDataModel item)
        {
            ItemTipManager.GetInstance().ShowTip(item as ItemData);
        }

        void OnClickAward()
        {
            int mGoodLuckCharmNum = 0;
            int mGoodLuckCharmTatleNumber = 0;
            string mGoodLuckCharmName = "";
            if (RewardIsHaveGoogLuckCharm(ref mGoodLuckCharmNum,ref mGoodLuckCharmTatleNumber,ref mGoodLuckCharmName))
            {
                //如果领取的好运符数量加上已有的好运符数量大于最大数 弹框提示
                if ((mGoodLuckCharmNum + (int)PlayerBaseData.GetInstance().WeaponLeaseTicket) > mGoodLuckCharmTatleNumber)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("GoodLuckCharmDesc", mGoodLuckCharmNum, mGoodLuckCharmName, mGoodLuckCharmName),
                        ()=>
                        {
                            MissionManager.GetInstance().sendCmdSubmitTask((uint)value.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
                        });
                }
                else
                {
                    MissionManager.GetInstance().sendCmdSubmitTask((uint)value.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
                }

                return;
            }
           
            MissionManager.GetInstance().sendCmdSubmitTask((uint)value.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
        }

        /// <summary>
        /// 奖励道具是否有好运符
        /// </summary>
        /// <returns></returns>
        bool RewardIsHaveGoogLuckCharm(ref int iGoodLuckCharmNum, ref int iGoodLuckCharmTatleNumber,ref string sGoodLuckCharmName)
        {
            bool isFindGoodLuckCharm = false;//奖励道具是否有好运符
            var awards = MissionManager.GetInstance().GetMissionAwards(value.missionItem.ID);
            for (int i = 0; i < awards.Count; i++)
            {
                var mAwards = awards[i];
                var mItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mAwards.ID);
                if (mItemTable != null)
                {
                    if (mItemTable.Type != ItemTable.eType.INCOME && mItemTable.SubType != ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                    {
                        continue;
                    }

                    isFindGoodLuckCharm = true;
                    iGoodLuckCharmNum = mAwards.Num;
                    iGoodLuckCharmTatleNumber = mItemTable.MaxNum;
                    ItemData mData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemTable.ID);
                    if (mData != null)
                    {
                        sGoodLuckCharmName = mData.GetColorName();
                    }
                }
            }

            return isFindGoodLuckCharm;
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
            frame = clientFrame as T;
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
            //设置积分
            score.text = "x" + missionItem.MissionParam.ToString();
            //设置活力值
            VitalityValue.text = "x" + missionItem.VitalityValue;
            //设置任务图标
            if(null != Icon)
            {
                // Icon.sprite = AssetLoader.instance.LoadRes(missionItem.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref Icon, missionItem.Icon);
                Icon.SetNativeSize();
            }
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

                        ComItemNew comItem = null;
                        if (akCachedItems.Count > 0)
                        {
                            comItem = akCachedItems[0];
                            akCachedItems.RemoveAt(0);
                        }
                        else
                        {
                            comItem = frame.CreateComItemNew(awardParent);
                        }
                        if (comItem != null)
                        {
                            comItem.CustomActive(true);
                            comItem.Setup(ItemData, _OnItemClicked);
                            comItem.transform.SetSiblingIndex(i + 1);
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
                go.CustomActive(false);
                acquired.CustomActive(true);
                //设置完成图标
            }
            else
            {
                complete.CustomActive(false);
                acquired.CustomActive(false);

                bool bNeedShowGo = missionItem != null && missionItem.TaskFinishType == ProtoTable.MissionTable.eTaskFinishType.TFT_LINKS &&
                    !string.IsNullOrEmpty(missionItem.LinkInfo) && value.status == (int)Protocol.TaskStatus.TASK_UNFINISH;

                award.CustomActive(!bNeedShowGo);
                go.CustomActive(bNeedShowGo);

                //设置完成进度
                var contentProcess = Utility.ParseMissionProcess(missionItem.ID, true);
                if (contentProcess != null)
                {
                    if (contentProcess.iPreValue > contentProcess.iAftValue)
                    {
                        contentProcess.iPreValue = contentProcess.iAftValue;
                    }
                    bCanSendAcquire = value.status == (int)Protocol.TaskStatus.TASK_FINISHED;
                }
            }

            award.enabled = !bAcquired && bCanSendAcquire;
            gray.enabled = !award.enabled;
        }
    }
    class ComDailyScript : ComDailyScriptTemplate<MissionDailyFrame>
    {

    }
}