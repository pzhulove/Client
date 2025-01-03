using System;
using System.Collections.Generic;
using Scripts.UI;
namespace GameClient
{
    //Roll道具结果界面
    public class DungeonRollResultFrame : ClientFrame
    {
        ComUIListScript mRollItems = null;
        List<BattleDataManager.ResultRollItem> mResultItemList = null;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Reward/RollItemResultFrame";
        }
        protected override void _bindExUI()
        {
            mRollItems = mBind.GetCom<ComUIListScript>("RollItemList");
        }
        protected override void _unbindExUI()
        {
            mRollItems = null;
        }
        protected override void _OnCloseFrame()
        {
           if(GameClient.ItemTipManager.GetInstance() != null)
            GameClient.ItemTipManager.GetInstance().CloseAll();
        }
        private int SortItemData(BattleDataManager.ResultRollItem a, BattleDataManager.ResultRollItem b)
        {
            if (a == null || b == null || a.item == null || b.item == null) return 0;
            if (a.item.Quality > b.item.Quality) return 1;
            if (a.item.Quality == b.item.Quality)
            {
                if (a.item.TableID < b.item.TableID)
                {
                    return 1;
                }
                else if (a.item.TableID > b.item.TableID)
                {
                    return -1;
                }
                return 0;
            }
            return -1;
        }
        protected override void _OnOpenFrame()
        {
            var resultItemList = userData as List<BattleDataManager.ResultRollItem>;
            if (resultItemList == null) return;
            //如果有部分玩家谦让，不显示，只显示roll点玩家信息，由上至下的顺序显示
            //如果全部玩家谦让显示所有玩家信息
            //这里重新组织需要显示的玩家数据给界面使用
            mResultItemList = new List<BattleDataManager.ResultRollItem>();
            for (int i = 0; i < resultItemList.Count;i++)
            {
                var curItem = resultItemList[i];
               
                if (curItem == null) continue;
                var newItem = new BattleDataManager.ResultRollItem
                {
                    itemData = curItem.itemData,
                    item = curItem.item
                };
                mResultItemList.Add(newItem);
                bool isAllHum = true;
                for(int j = 0;j < curItem.playerInfoes.Count;j++)
                {
                    if(curItem.playerInfoes[j].opType != (byte)Protocol.RollOpTypeEnum.RIE_MODEST)
                    {
                        isAllHum = false;
                        newItem.playerInfoes.Add(curItem.playerInfoes[j]);
                    }
                }
                if(isAllHum)
                {
                    newItem.playerInfoes.AddRange(curItem.playerInfoes);
                }
            }
            if (mResultItemList != null)
            {
                mResultItemList.Sort(SortItemData);
            }
            _InitUI();
        }
        private void OnItemData(ComUIListElementScript item)
        {
            if (mResultItemList == null) return;
            if (item.m_index >= 0 && item.m_index < mResultItemList.Count)
            {
                ComRollItemResult comUI = item.GetComponent<ComRollItemResult>();
                if (comUI != null)
                {
                    var curData = mResultItemList[(mResultItemList.Count - item.m_index) - 1];
                    if (curData == null) return;
                    comUI.Init(curData.item, curData.playerInfoes);
                }
            }
        }
        void _InitUI()
        {
            if (mRollItems == null) return;
            mRollItems.Initialize();
            mRollItems.onItemVisiable = OnItemData;
            mRollItems.SetElementAmount(mResultItemList.Count);
        }
    }
}
