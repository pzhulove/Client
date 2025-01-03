using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class ExpGetBackActive : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList;
        private List<ActiveData> mDataList = new List<ActiveData>();
        private int mFatiguePerCount = 25;

        public enum EActiveType
        {
            Reward,//奖励找回
            Fatigue,//疲劳找回
        }

        public class ActiveData
        {
            public string Icon;
            public List<int> NormalRewardIds = new List<int>();
            public List<int> NormalRewardNums = new List<int>();
            public int NormalCostItemId;
            public int NormalCostNum;
            public int PerfectCostItemId;
            public int PerfectCostNum;
            public List<int> PerfectRewardItemIds = new List<int>();
            public List<int> PerfectRewardNums = new List<int>();
            public int Lv;
            public int Vip;
            public int Count;//可找回次数
            public int MaxCount;//最多可找回
            public int NormalHasGotBack;//普通已找回
            public int PerfectHasGotBack;//完美已找回
            public ActiveManager.ActivityData OriginData; //原始数据
            public EActiveType ActiveType;
        }
        

        // Start is called before the first frame update
        void Start()
        {
            mComUIList.Initialize();
            mComUIList.onItemVisiable = _OnItemVisiable;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;

            _RefreshData();
        }

        private void _OnItemVisiable(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < mDataList.Count)
            {
                var script = item.GetComponent<ExpGetBackItem>();
                if (script != null)
                {
                    script.Init(mDataList[item.m_index], _OnNormalClick, _OnPerfectClick);
                }
            }
        }

        private void _OnNormalClick(ActiveData data)
        {
            if (data != null)
            {
                GetBackFrameParam param = new GetBackFrameParam()
                {
                    ActiveData = data,
                    IsPerfect = false
                };
                if (data.ActiveType == EActiveType.Fatigue)
                {
                    if (PlayerBaseData.GetInstance().IsLevelFull)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_fatigue_get_back_full"));
                    }
                    else
                    {
                        ClientSystemManager.GetInstance().OpenFrame<FatigueGetBackFrame>(FrameLayer.Middle, param);
                    }
                }
                else if (data.ActiveType == EActiveType.Reward)
                {
                    ClientSystemManager.GetInstance().OpenFrame<RewardGetBackFrame>(FrameLayer.Middle, param);
                }
            }
        }

        private void _OnPerfectClick(ActiveData data)
        {
            if (data != null)
            {
                GetBackFrameParam param = new GetBackFrameParam()
                {
                    ActiveData = data,
                    IsPerfect = true
                };
                if (data.ActiveType == EActiveType.Fatigue)
                {
                    if (PlayerBaseData.GetInstance().IsLevelFull)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_fatigue_get_back_full"));
                    }
                    else
                    {
                        ClientSystemManager.GetInstance().OpenFrame<FatigueGetBackFrame>(FrameLayer.Middle, param);
                    }
                }
                else if (data.ActiveType == EActiveType.Reward)
                {
                    ClientSystemManager.GetInstance().OpenFrame<RewardGetBackFrame>(FrameLayer.Middle, param);
                }
            }
        }

        private void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            _RefreshData();
        }

        private void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        private void _RefreshData()
        {
            var rewardActive = ActiveManager.GetInstance().GetActiveData(8200);
            var fatigueActive = ActiveManager.GetInstance().GetActiveData(8100);
            mDataList.Clear();

            if (fatigueActive != null && fatigueActive.akChildItems != null)
            {
                for (int i = 0; i < fatigueActive.akChildItems.Count; ++i)
                {
                    _AddFatigueActivity(fatigueActive.akChildItems[i]);
                }
            }

            if (rewardActive != null && rewardActive.akChildItems != null)
            {
                for (int i = 0; i < rewardActive.akChildItems.Count; ++i)
                {
                    _AddRewardActivity(rewardActive.akChildItems[i]);
                }
            }

            mComUIList.SetElementAmount(mDataList.Count);
        }

        private void _AddFatigueActivity(ActiveManager.ActivityData data)
        {
            var activeData = new ActiveData();
            activeData.OriginData = data;
            activeData.ActiveType = EActiveType.Fatigue;
            mDataList.Add(activeData);

            ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(data.ID);
            if (activeItem != null)
            {
                activeData.Icon = activeItem.InitDesc;
            }

            int rewardeItemId = 600000003;
            var rewardData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.FATIGUE_GET_BACK_REWARD_ID);

            if (rewardData != null && rewardData.IntParamsLength > 0)
            {
                rewardeItemId = rewardData.IntParamsArray(0);
            }
            activeData.NormalRewardIds.Add(rewardeItemId);
            activeData.PerfectRewardItemIds.Add(rewardeItemId);
            if (data.akActivityValues != null)
            {
                for (int i = 0; i < data.akActivityValues.Count; ++i)
                {
                    var msg = data.akActivityValues[i];
                    if (msg.key == "lf")
                    {
                        int.TryParse(msg.value, out activeData.Count);
                    }
                    else if (msg.key == "mf")
                    {
                        int.TryParse(msg.value, out activeData.MaxCount);
                    }
                    else if (msg.key == "le")
                    {
                        int num;
                        int.TryParse(msg.value, out num);
                        activeData.NormalRewardNums.Add(num);
                    }
                    else if (msg.key == "lmi")
                    {
                        int.TryParse(msg.value, out activeData.NormalCostItemId);
                    }
                    else if (msg.key == "he")
                    {
                        int num;
                        int.TryParse(msg.value, out num);
                        activeData.PerfectRewardNums.Add(num);
                    }
                    else if (msg.key == "hmi")
                    {
                        int.TryParse(msg.value, out activeData.PerfectCostItemId);
                    }
                    else if (msg.key == "hgf")
                    {
                        int.TryParse(msg.value, out activeData.PerfectHasGotBack);
                    }
                    else if (msg.key == "lgf")
                    {
                        int.TryParse(msg.value, out activeData.NormalHasGotBack);
                    }
                    else if (msg.key == "vip")
                    {
                        int.TryParse(msg.value, out activeData.Vip);
                    }
                    else if (msg.key == "lv")
                    {
                        int.TryParse(msg.value, out activeData.Lv);
                    }
                }
            }
        }

        private void _AddRewardActivity(ActiveManager.ActivityData data)
        {
            var activeData = new ActiveData();
            int normalCostId = 600000001;
            int perfectCostId = 600000008;

            activeData.OriginData = data;
            activeData.ActiveType = EActiveType.Reward;
            activeData.NormalCostItemId = normalCostId;
            activeData.PerfectCostItemId = perfectCostId;
            mDataList.Add(activeData);

            var normalData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.REWARD_GET_BACK_NORAML_COST_ID);
            var perfectData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.REWARD_GET_BACK_PERFECT_COST_ID);

            ProtoTable.ActiveTable activeItem = TableManager.GetInstance().GetTableItem<ProtoTable.ActiveTable>(data.ID);
            if (activeItem != null)
            {
                activeData.Icon = activeItem.InitDesc;
            }

            if (normalData != null && normalData.IntParamsLength > 0)
            {
                normalCostId = normalData.IntParamsArray(0);
            }

            if (perfectData != null && perfectData.IntParamsLength > 0)
            {
                perfectCostId = perfectData.IntParamsArray(0);
            }

            var step = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_FATIGUE_MAKEUP_STEP);
            if (step != null)
            {
                mFatiguePerCount = step.Value;
            }

            if (data.akActivityValues != null)
            {
                int lv = 0;
                for (int i = 0; i < data.akActivityValues.Count; ++i)
                {
                    var msg = data.akActivityValues[i];
                    if (msg.key.Contains("ni"))
                    {
                        int id;
                        int.TryParse(msg.value, out id);
                        activeData.NormalRewardIds.Add(id);
                    }
                    else if (msg.key.Contains("nnum"))
                    {
                        int num;
                        int.TryParse(msg.value, out num);
                        activeData.NormalRewardNums.Add(num);
                    }
                    if (msg.key.Contains("pi"))
                    {
                        int id;
                        int.TryParse(msg.value, out id);
                        activeData.PerfectRewardItemIds.Add(id);
                    }
                    else if (msg.key.Contains("pnum"))
                    {
                        int num;
                        int.TryParse(msg.value, out num);
                        activeData.PerfectRewardNums.Add(num);
                    }
                    else if (msg.key == "ncs")
                    {
                        int.TryParse(msg.value, out activeData.NormalCostNum);
                    }
                    else if (msg.key == "pcs")
                    {
                        int.TryParse(msg.value, out activeData.PerfectCostNum);
                    }
                    else if (msg.key == "ct")
                    {
                        int.TryParse(msg.value, out activeData.Count);
                    }
                }
                //activeData.NormalRewardNums
            }

        }
    }
}
