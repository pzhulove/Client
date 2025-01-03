using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventurerPassCardBuyLevelFrameView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mListScript;
        [SerializeField] private Slider mLvSlider;
        [SerializeField] private Text mTextAwardCount;
        [SerializeField] private Text mTextBuyLvCount;
        [SerializeField] private Text mTextCostCount;
        private int mAddLv = 1;
        private int mCurLv = 0;
        private List<AdventurerPassCardDataManager.ItemInfo> mAwardList;
        public void OnInit()
        {
            if (null != mListScript)
            {
                mListScript.Initialize();
                mListScript.onItemVisiable = _OnShowItem;
                mListScript.OnItemUpdate = _OnShowItem;
            }
            mCurLv = (int)AdventurerPassCardDataManager.GetInstance().CardLv;
            var maxBuyLv = AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv() - mCurLv;
            if (maxBuyLv <= 0)
            {
                Logger.LogErrorFormat("冒险通行证已经到达满级 无法再购买等级");
                return;
            }
            mLvSlider.minValue = 1;
            mLvSlider.maxValue = maxBuyLv;
            mAddLv = 1;
            mLvSlider.value = mAddLv;
            SetBuyLv();
        }

        public void OnUninit()
        {
            if (null != mListScript)
            {
                mListScript.UnInitialize();
            }
        }

        private void SetBuyLv()
        {
            mAwardList = AdventurerPassCardDataManager.GetInstance().GetSeasonLvRangeAwardItems(mCurLv + 1, mCurLv + mAddLv);
            mAwardList.Sort(_SortList);
            mTextAwardCount.SafeSetText(TR.Value("adventurer_pass_card_buy_lv_award_count_tip", mCurLv + mAddLv, mAwardList.Count));
            mTextBuyLvCount.SafeSetText(TR.Value("adventurer_pass_card_buy_lv_count_tip", mAddLv, mCurLv + mAddLv));
            var table = TableManager.GetInstance().GetTableItem<AdventurePassBuyLevelTable>(mAddLv);
            if (null == table)
            {
                Logger.LogErrorFormat("冒险通行证购买等级表中没有id={0}的数据", mAddLv);
                return;
            }
            mTextCostCount.SafeSetText(TR.Value("adventurer_pass_card_buy_lv_cost_tip", table.PointCost));
            mListScript.SetElementAmount(mAwardList.Count);
        }
        //排序
        private int _SortList(AdventurerPassCardDataManager.ItemInfo l, AdventurerPassCardDataManager.ItemInfo r)
        {
            if (null == r && null == l)
                return 0;
            if (null == r)
                return -1;
            if (null == l)
                return 1;
            var tableItemL = TableManager.GetInstance().GetTableItem<ItemTable>(l.itemID);
            var tableItemR = TableManager.GetInstance().GetTableItem<ItemTable>(r.itemID);
            if (null == tableItemR && null == tableItemL)
                return 0;
            if (null == tableItemR)
                return -1;
            if (null == tableItemL)
                return 1;
            if (tableItemL.Color > tableItemR.Color)
                return -1;
            if (tableItemL.Color < tableItemR.Color)
                return 1;
            if (l.itemNum > r.itemNum)
                return -1;
            if (l.itemNum < r.itemNum)
                return 1;
            if (l.itemID > r.itemID)
                return -1;
            if (l.itemID < r.itemID)
                return 1;
            return 0;
        }

        //显示道具
        private void _OnShowItem(ComUIListElementScript item)
        {
            if (null == item || null == mAwardList || mAwardList.Count <= item.m_index)
                return;
            var script = item.GetComponent<ShowAwardItem>();
            if (null == script)
                return;
            var model = ItemDataManager.GetInstance().CreateItem(mAwardList[item.m_index].itemID, mAwardList[item.m_index].itemNum);
            script.OnInit(model);
        }

        public void OnClickAdd()
        {
            mLvSlider.value += 1;
        }

        public void OnClickRecduce()
        {
            mLvSlider.value -= 1;
        }

        public void OnValueChanged(float value)
        {
            mAddLv = (int)value;
            SetBuyLv();
        }

        public void OnClickBuyLv()
        {
            int moneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            var table = TableManager.GetInstance().GetTableItem<AdventurePassBuyLevelTable>(mAddLv);
            if (null == table)
                return;
            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
            {
                nMoneyID = moneyID,
                nCount = table.PointCost,
            },
            () =>
            {
                LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.AdventurerPassCardBuyLevel,
               TR.Value("adventurer_pass_card_buy_lv_tip", table.PointCost, mCurLv + mAddLv, mAwardList.Count),
               () =>
               {
                   AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassBuyLvReq((uint)mAddLv);
               },
               () =>
               {
               },
               TR.Value("adventurer_pass_card_buy_lv_tip_ok"),
               TR.Value("adventurer_pass_card_buy_lv_tip_cancel"));
            },
            "common_money_cost",
            null);
            return;
        }

        public void OnClickCancel()
        {
            ClientSystemManager.GetInstance().CloseFrame<AdventurerPassCardBuyLevelFrame>();
        }
    }
}
