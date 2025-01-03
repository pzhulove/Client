using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    public class GetBackFrameParam
    {
        public ExpGetBackActive.ActiveData ActiveData;
        public bool IsPerfect;
    }

    public class FatigueGetBackFrame : ClientFrame
    {
        private FatigueGetBackFrameView mView;
        private GetBackFrameParam mParam;
        private List<ProtoTable.FatigueMakeUpPrice> mPriceItems = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Activities/FatigueGetBackFrame";
        }

        protected override void _OnOpenFrame()
        {
            var data = userData as GetBackFrameParam;
            if (data == null || frame == null)
            {
                Close();
            }
            else
            {
                if (null == mPriceItems)
                {
                    Dictionary<int, object> fatiguePriceTable = TableManager.GetInstance().GetTable<ProtoTable.FatigueMakeUpPrice>();
                    if (null != fatiguePriceTable)
                    {
                        mPriceItems = new List<ProtoTable.FatigueMakeUpPrice>(fatiguePriceTable.Count);
                        var enumerator = fatiguePriceTable.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            mPriceItems.Add(enumerator.Current.Value as ProtoTable.FatigueMakeUpPrice);
                        }
                    }
                }
                mParam = data;
                mView = frame.GetComponent<FatigueGetBackFrameView>();
                if (mView != null && mParam != null && mParam.ActiveData != null)
                {
                    int step = 0;
                    var stepData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_FATIGUE_MAKEUP_STEP);
                    if (stepData != null)
                    {
                        step = stepData.Value;
                    }
                    int costId = mParam.IsPerfect ? mParam.ActiveData.PerfectCostItemId : mParam.ActiveData.NormalCostItemId;
                    int exp = _GetExpPerValue(mParam.ActiveData.Lv, mParam.IsPerfect);
                    mView.Init(mParam.ActiveData.MaxCount, mParam.ActiveData.Vip, mParam.ActiveData.MaxCount, mParam.ActiveData.Count, exp, costId, step, _OnConfirmClick, _OnCloseClick, _GetTotalMoneyCost);
                }
                else
                {
                    _OnCloseClick();
                }
            }
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private void _OnConfirmClick(int fatigue)
        {
            int leftFagure = mParam.ActiveData.Count;
            int getValue = IntMath.Min(leftFagure, fatigue);
            int iTotalMoney = _GetTotalMoneyCost(getValue);

            int costId = mParam.IsPerfect ? mParam.ActiveData.PerfectCostItemId : mParam.ActiveData.NormalCostItemId;
            int iValue = mParam.IsPerfect ? ((1 << 31) | (1 << 16)) : ((1 << 31) | (0 << 16));

            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = costId, nCount = iTotalMoney },
                () =>
                {
                    iValue |= fatigue;
                    ActiveManager.GetInstance().SendSubmitActivity(mParam.ActiveData.OriginData.ID, (uint)iValue);
                    frameMgr.CloseFrame(this);
#if UNITY_EDITOR_WIN
                    Logger.LogErrorFormat("try getValue = {0} cost = {1} iValue = {2}",
                        getValue,
                        iTotalMoney,
                        iValue);
#endif
                });
        }

        int _GetTotalMoneyCost(int curPl)
        {
            int iPrePl = mParam.IsPerfect ? mParam.ActiveData.PerfectHasGotBack : mParam.ActiveData.NormalHasGotBack;
            int sum = 0;

            var begin = _FindZone(iPrePl);
            var end = _FindZone(iPrePl + curPl);
            if (-1 != begin && -1 != end)
            {
                int tempPl = iPrePl;
                while(begin != end)
                {
                    var priceItem = mPriceItems[begin];
                    int price = mParam.IsPerfect ? priceItem.HiPrice : priceItem.LowPrice;
                    sum += (priceItem.FatigueSection[1] - tempPl) * price;
                    tempPl = priceItem.FatigueSection[1];
                    begin += 1;
                }

                if(true)
                {
                    var priceItem = mPriceItems[begin];
                    int price = mParam.IsPerfect ? priceItem.HiPrice : priceItem.LowPrice;
                    sum += ((iPrePl + curPl) - tempPl) * price;
                }
            }
            else
            {
                Logger.LogErrorFormat("iPrePl = {0} iPrePl + curPl = {1}", iPrePl, iPrePl + curPl);
            }

            return sum;
        }

        int _GetExpPerValue(int lv, bool isPerfect)
        {
            int iQueryValue = 0;
            var fagureItem = TableManager.GetInstance().GetTableItem<ProtoTable.FatigueMakeUp>(lv);
            if (null != fagureItem)
            {
                if (!isPerfect)
                    iQueryValue = fagureItem.LowEXP;
                else
                    iQueryValue = fagureItem.HiEXP;
            }
            return iQueryValue;
        }

        int _GetTotalMoneyCost(int curPl, bool isPerfect, int iPrePl)
        {
            int sum = 0;

            var begin = _FindZone(iPrePl);
            var end = _FindZone(iPrePl + curPl);
            if (-1 != begin && -1 != end)
            {
                int tempPl = iPrePl;
                while (begin != end)
                {
                    var priceItem = mPriceItems[begin];
                    int price = isPerfect ? priceItem.HiPrice : priceItem.LowPrice;
                    sum += (priceItem.FatigueSection[1] - tempPl) * price;
                    tempPl = priceItem.FatigueSection[1];
                    begin += 1;
                }

                if (true)
                {
                    var priceItem = mPriceItems[begin];
                    int price = isPerfect ? priceItem.HiPrice : priceItem.LowPrice;
                    sum += ((iPrePl + curPl) - tempPl) * price;
                }
            }
            else
            {
                Logger.LogErrorFormat("iPrePl = {0} iPrePl + curPl = {1}", iPrePl, iPrePl + curPl);
            }

            return sum;
        }

        int _FindZone(int pl)
        {
            int findIndex = -1;
            for (int i = 0; i < mPriceItems.Count; ++i)
            {
                var priceItem = mPriceItems[i];
                if (pl >= (priceItem.FatigueSection[0]) && pl <= priceItem.FatigueSection[1])
                {
                    findIndex = i;
                    break;
                }
            }
            return findIndex;
        }
    }
}
