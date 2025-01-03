using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using ProtoTable;

namespace GameClient
{
    public class CostItemManager : DataManager<CostItemManager>
    {
        public class CostInfo
        {
            public int nMoneyID = 0;
            public int nCount = 0;
            public bool bMainCost = true;

            public bool IsValid()
            {
                if (
                    nMoneyID > 0 &&
                    TableManager.GetInstance().GetTableItem<ItemTable>(nMoneyID) != null &&
                    nCount >= 0
                    )
                {
                    return true;
                }
                return false;
            }
        }

        bool m_bNotify = true;
        public bool isNotify { get { return m_bNotify; } set { m_bNotify = value; } }

        public override void Initialize()
        {
            isNotify = true;
        }

        public override void Clear()
        {
            isNotify = true;
        }

        /// <summary>
        /// 消耗多种道具，并提供默认处理流程
        /// </summary>
        /// <param name="a_arrCostInfos"></param>
        /// <param name="a_delDefaultCanCost"></param>
        /// <param name="a_strDefaultCanCostDesc"></param>
        public void TryCostMoneiesDefault(List<CostInfo> a_arrCostInfos, Action a_delDefaultCanCost, Action a_cancel = null, string a_strDefaultCanCostDesc = "common_money_cost")
        {
            List<CostInfo> arrMoneyCostInfos = new List<CostInfo>();
            int nMoneyTypeCount = 0;
            for (int j = 0; j < a_arrCostInfos.Count; ++j)
            {
                CostInfo costInfo = a_arrCostInfos[j];
                if (costInfo.IsValid() == false)
                {
                    continue;
                }
                List<CostInfo> arrRealCostInfos = _CalculateRealCostInfos(costInfo);

                int nTotalOwnedCount = 0;
                for (int i = 0; i < arrRealCostInfos.Count; ++i)
                {
                    nTotalOwnedCount += arrRealCostInfos[i].nCount;
                }
                if (nTotalOwnedCount < costInfo.nCount)
                {
                    CommonCannotCostMoneyHandle(costInfo, arrRealCostInfos.ToArray());
                    return;
                }
                else
                {
                    if (_IsIncomeType(costInfo.nMoneyID))
                    {
                        arrMoneyCostInfos.AddRange(arrRealCostInfos);
                        nMoneyTypeCount++;
                    }
                }
            }

            // 如果用到了替代货币，则弹出提示框，否则，直接执行操作
            if (arrMoneyCostInfos.Count > nMoneyTypeCount && isNotify)
            {
                CostInfo[] arrTempInfos = arrMoneyCostInfos.ToArray();

                CostItemNotifyData frameData = new CostItemNotifyData();
                frameData.strContent = TR.Value(a_strDefaultCanCostDesc, _GetMainCostMoneiesDesc(arrTempInfos), _GetEqualCostMoneiesDesc(arrTempInfos));
                frameData.delOnOkCallback = () =>
                {
                    if (a_delDefaultCanCost != null)
                    {
                        for(int i = 0;i < a_arrCostInfos.Count;i++)
                        {
                            int nMoneyID = a_arrCostInfos[i].nMoneyID;
                            if (nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT) || nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT))
                            {
                                if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                                {
                                    return;
                                }
                            }
                        }
                        a_delDefaultCanCost.Invoke();
                    }
                };

                frameData.delOnCancelCallback = () =>
                {
                    if (a_cancel != null)
                    {
                        a_cancel.Invoke();
                    }
                };

                ClientSystemManager.GetInstance().OpenFrame<CostItemNotifyFrame>(FrameLayer.Middle, frameData);
            }
            else
            {
                if (a_delDefaultCanCost != null)
                {
                    for (int i = 0; i < a_arrCostInfos.Count; i++)
                    {
                        int nMoneyID = a_arrCostInfos[i].nMoneyID;
                        if (nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT) || nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT))
                        {
                            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                            {
                                return;
                            }
                        }
                    }
                    a_delDefaultCanCost.Invoke();
                }
            }
        }

        /// <summary>
        /// 消耗一种道具，并提供默认处理流程
        /// </summary>
        /// <param name="a_costInfo"></param>
        /// <param name="a_delDefaultCanCost"></param>
        /// <param name="a_strDefaultCanCostDesc"></param>
        public void TryCostMoneyDefault(CostInfo a_costInfo, Action a_delDefaultCanCost, string a_strDefaultCanCostDesc = "common_money_cost", Action a_cancel = null)
        {
            if (a_costInfo.IsValid() == false)
            {
                return;
            }

            List<CostInfo> arrRealCostInfos = _CalculateRealCostInfos(a_costInfo);

            int nTotalOwnedCount = 0;
            for (int i = 0; i < arrRealCostInfos.Count; ++i)
            {
                nTotalOwnedCount += arrRealCostInfos[i].nCount;
            }
            if (nTotalOwnedCount < a_costInfo.nCount)
            {
                if (a_cancel != null)
                {
                    a_cancel.Invoke();
                }

                CommonCannotCostMoneyHandle(a_costInfo, arrRealCostInfos.ToArray());
            }
            else
            {
                // 如果是货币，且用到了替代货币，则弹出提示框，否则，直接执行操作
                if ((_IsIncomeType(a_costInfo.nMoneyID) || _IsTicketType(a_costInfo.nMoneyID)) && arrRealCostInfos.Count > 1 && isNotify) 
                {
                    CostInfo[] arrTempInfos = arrRealCostInfos.ToArray();
                    CostItemNotifyData frameData = new CostItemNotifyData();
                    frameData.strContent = TR.Value(a_strDefaultCanCostDesc, _GetMainCostMoneiesDesc(arrTempInfos), _GetEqualCostMoneiesDesc(arrTempInfos));
                    frameData.delOnOkCallback = () =>
                    {
                        if (a_delDefaultCanCost != null)
                        {
                            if (a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT) || a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT))
                            {
                                if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                                {
                                    return;
                                }
                            }
                            a_delDefaultCanCost.Invoke();
                        }
                    };
                    frameData.delOnCancelCallback = () =>
                    {
                        if (a_cancel != null)
                        {
                            a_cancel.Invoke();
                        }
                    };
                    ClientSystemManager.GetInstance().OpenFrame<CostItemNotifyFrame>(FrameLayer.Middle, frameData);
                }
                else
                {
                    if (a_delDefaultCanCost != null)
                    {
                        if (a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT) || a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT))
                        {
                            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                            {
                                return;
                            }
                        }
                        a_delDefaultCanCost.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 消耗一种道具，提供可以消耗，和不可以消耗的回调
        /// </summary>
        /// <param name="a_costInfo"></param>
        /// <param name="a_delCanCost"></param>
        /// <param name="a_delCanotCost"></param>
        public void TryCostMoney(CostInfo a_costInfo, Action<CostInfo, CostInfo[]> a_delCanCost, Action<CostInfo, CostInfo[]> a_delCanotCost)
        {
            if (a_costInfo.IsValid() == false)
            {
                return;
            }

            List<CostInfo> arrRealCostInfos = _CalculateRealCostInfos(a_costInfo);

            int nTotalOwnedCount = 0;
            for (int i = 0; i < arrRealCostInfos.Count; ++i)
            {
                nTotalOwnedCount += arrRealCostInfos[i].nCount;
            }
            if (nTotalOwnedCount < a_costInfo.nCount)
            {
                if (a_delCanotCost != null)
                {
                    a_delCanotCost.Invoke(a_costInfo, arrRealCostInfos.ToArray());
                }
            }
            else
            {
                if (a_delCanCost != null)
                {
                    if(a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT) || a_costInfo.nMoneyID == ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT))
                    {
                        if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                        {
                            return;
                        }
                    }
                    a_delCanCost.Invoke(a_costInfo, arrRealCostInfos.ToArray());
                }
            }
        }

        /// <summary>
        /// 通用的不可消耗道具的处理函数
        /// </summary>
        /// <param name="a_costInfo"></param>
        /// <param name="a_arrRealCostInfos"></param>
        public void CommonCannotCostMoneyHandle(CostInfo a_costInfo, CostInfo[] a_arrRealCostInfos)
        {
            if (a_arrRealCostInfos == null || a_costInfo == null)
            {
                return;
            }

            // 货币（道具）不足，如果存在点券，则直接弹出购买提示，否则，弹出获取途径
            bool bHasPoint = false;
            int nPointID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            for (int i = 0; i < a_arrRealCostInfos.Length; ++i)
            {
                if (a_arrRealCostInfos[i].nMoneyID == nPointID)
                {
                    SystemNotifyManager.SystemNotify(1207, () =>
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<VipFrame>())
                        {
                            VipFrame frame = ClientSystemManager.GetInstance().GetFrame(typeof(VipFrame)) as VipFrame;
                            frame.SwitchPage(VipTabType.PAY);
                        }
                        else
                        {
                            ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
                        }
                    });
                    bHasPoint = true;
                    break;
                }
            }

            if (bHasPoint == false)
            {
                if (a_arrRealCostInfos.Length > 0)
                {
                    ItemComeLink.OnLink(a_costInfo.nMoneyID, a_costInfo.nCount, true);
                }
            }
        }

        /// <summary>
        /// 格式化消耗道具的信息
        /// </summary>
        /// <param name="a_infos"></param>
        /// <returns></returns>
        public string GetCostMoneiesDesc(params CostInfo[] a_infos)
        {
            string strMoneies = string.Empty;
            if (a_infos == null)
            {
                return strMoneies;
            }

            for (int k = 0; k < a_infos.Length; ++k)
            {
                if (a_infos[k].nCount > 0)
                {
                   
                    ItemData data = ItemDataManager.GetInstance().GetCommonItemTableDataByID(a_infos[k].nMoneyID);
                    if (data != null)
                    {
                        if (string.IsNullOrEmpty(strMoneies) == false)
                        {
                            strMoneies += '、';
                        }
                        strMoneies += TR.Value("common_money_format",data.GetColorName(),a_infos[k].nCount);
                    }
                }
            }

            return strMoneies;
        }

        /// <summary>
        /// 格式化主消耗道具的信息
        /// </summary>
        /// <param name="a_infos"></param>
        /// <returns></returns>
        string _GetMainCostMoneiesDesc(params CostInfo[] a_infos)
        {
            string strMainMoneiesDesc = string.Empty;
            if (a_infos == null)
            {
                return strMainMoneiesDesc;
            }

            for (int k = 0; k < a_infos.Length; ++k)
            {
                if (a_infos[k].bMainCost)
                {
                    if (string.IsNullOrEmpty(strMainMoneiesDesc) == false)
                    {
                        strMainMoneiesDesc += '、';
                    }
                    strMainMoneiesDesc += TR.Value("common_money_format",
                        ItemDataManager.GetInstance().GetCommonItemTableDataByID(a_infos[k].nMoneyID).GetColorName(),
                        a_infos[k].nCount);
                }
            }

            return strMainMoneiesDesc;
        }

        /// <summary>
        /// 格式化等价消耗道具的信息
        /// </summary>
        /// <param name="a_infos"></param>
        /// <returns></returns>
        string _GetEqualCostMoneiesDesc(params CostInfo[] a_infos)
        {
            string strEqualMoneiesDesc = string.Empty;
            if (a_infos == null)
            {
                return strEqualMoneiesDesc;
            }

            for (int k = 0; k < a_infos.Length; ++k)
            {
                if (a_infos[k].bMainCost == false)
                {
                    if (a_infos[k].nCount > 0)
                    {
                        if (string.IsNullOrEmpty(strEqualMoneiesDesc) == false)
                        {
                            strEqualMoneiesDesc += '、';
                        }
                        strEqualMoneiesDesc += TR.Value("common_money_format",
                            ItemDataManager.GetInstance().GetCommonItemTableDataByID(a_infos[k].nMoneyID).GetColorName(),
                            a_infos[k].nCount);
                    }
                }
            }

            return strEqualMoneiesDesc;
        }

        List<CostInfo> _CalculateRealCostInfos(CostInfo a_costInfo)
        {
            List<int> ids = new List<int>();
            EqualItemTable equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(a_costInfo.nMoneyID);
            if (equalTable != null)
            {
                ids.AddRange(equalTable.EqualItemIDs);
            }
            ids.Add(a_costInfo.nMoneyID);
            if (_IsIncomeType(a_costInfo.nMoneyID))
            {
                int id = a_costInfo.nMoneyID;
                ids.RemoveAt(ids.Count - 1);
                ids.Insert(0, id);
            }
            int nTotalOwnedCount = 0;
            List<CostInfo> arrCostInfos = new List<CostInfo>();
            for (int i = 0; i < ids.Count; ++i)
            {
                CostInfo info = new CostInfo();
                info.nMoneyID = ids[i];
                if (i == 0)
                {
                    info.bMainCost = true;
                }
                else
                {
                    info.bMainCost = false;
                }
                arrCostInfos.Add(info);

                int nOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(info.nMoneyID, false);
                if (nTotalOwnedCount + nOwnedCount >= a_costInfo.nCount)
                {
                    info.nCount = a_costInfo.nCount - nTotalOwnedCount;
                    nTotalOwnedCount = a_costInfo.nCount;
                    break;
                }
                else
                {
                    info.nCount = nOwnedCount;
                    nTotalOwnedCount += nOwnedCount;
                }
            }

            return arrCostInfos;
        }

        bool _IsIncomeType(int a_nItemID)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(a_nItemID);
            if (itemData != null)
            {
                return itemData.Type == ItemTable.eType.INCOME;
            }
            return false;
        }
        bool _IsTicketType(int a_nItemID)
        {
            if(a_nItemID == 200000004 || a_nItemID == 200000003)
            {
                return true;
            }
            return false;
        }

        public enum eCostEnoughType
        {
            /// <summary>
            /// 非法类型
            /// <summary>
            Invalid,

            /// <summary>
            /// 不够
            /// <summary>
            NotEnough,

            /// <summary>
            /// 使用替代道具之后，足够
            /// <summary>
            UseEqualItemEnough,

            /// <summary>
            /// 使用原始道具，足够
            /// <summary>
            UseOriginItemEnough,
        }

        /// <summary>
        /// 判断是否可以消耗对应道具
        /// </summary>
        /// <param name="a_costInfo"></param>
        /// <param name="a_delCanCost"></param>
        /// <param name="a_delCanotCost"></param>
        public eCostEnoughType GetCostEnoughType(CostInfo a_costInfo)
        {
            if (null == a_costInfo || a_costInfo.IsValid() == false)
            {
                return eCostEnoughType.Invalid;
            }

            List<CostInfo> arrRealCostInfos = _CalculateRealCostInfos(a_costInfo);

            int nTotalOwnedCount = 0;
            for (int i = 0; i < arrRealCostInfos.Count; ++i)
            {
                nTotalOwnedCount += arrRealCostInfos[i].nCount;
            }
            if (nTotalOwnedCount < a_costInfo.nCount)
            {
                return eCostEnoughType.NotEnough;
            }
            else
            {
                if (arrRealCostInfos.Count > 1)
                {
                    return eCostEnoughType.UseEqualItemEnough;
                }
                else
                {
                    return eCostEnoughType.UseOriginItemEnough;
                }
            }

        }

        public bool IsEnough2Cost(CostInfo info)
        {
            eCostEnoughType type = GetCostEnoughType(info);

            return eCostEnoughType.Invalid   != type
                && eCostEnoughType.NotEnough != type;
        }

        public bool IsEnough2Cost(List<CostInfo> infos)
        {
            for (int i = 0; i < infos.Count; ++i)
            {
                if (!IsEnough2Cost(infos[i]))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
