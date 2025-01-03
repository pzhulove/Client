using UnityEngine;
using System.Collections;
using GameClient;

namespace GameClient
{ 
    public interface IActivityConsumeData
    {
        long GetLeftCount();

        long GetSumCount();

        bool IsCanBuyCount();

        long GetLeftBuyCount();

        long GetHasUsedCount();

        long GetHasBuyCount();

        int GetCostItemID();

        int GetCostItemCount();

        byte GetCostItemType();
    }

    public class BaseActivityConsumeData
    {
        protected int                          mId;
        protected ProtoTable.DungeonTable      mDungeonTable;
        protected ProtoTable.DungeonTimesTable mDungeonTimesTable;
        protected ProtoTable.VipPrivilegeTable mVipTable;

        public BaseActivityConsumeData(int id)
        {
            mId           = id;
            mDungeonTable = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(id);

            if (null != mDungeonTable)
            {
				mDungeonTimesTable = TableManager.instance.GetTableItem<ProtoTable.DungeonTimesTable>((int)mDungeonTable.SubType);
            }

            if (null != mDungeonTimesTable)
            {
                mVipTable = TableManager.instance.GetTableItem<ProtoTable.VipPrivilegeTable>(mDungeonTimesTable.BuyTimesVipPrivilege);
            }
        }
    }

    public class NormalActivityConsumeData : BaseActivityConsumeData, IActivityConsumeData
    {
        public NormalActivityConsumeData(int id) : base(id)
        {
        }

        /// <summary>
        /// 基础次数
        /// </summary>
        protected long _getCommonSumCount()
        {
            long cnt = 0;

            if (null != mDungeonTimesTable)
            {
                cnt = mDungeonTimesTable.BaseTimes;
            }

            return cnt;
        }

        /// <summary>
        /// 基础购买次数次数
        /// </summary>
        protected long _getCommonBuySumCount()
        {
            long cnt = 0;
            if (null != mDungeonTimesTable)
            {
                cnt = mDungeonTimesTable.BaseBuyTimes;
            }

            return cnt;
        }

        /// <summary>
        /// VIP购买次数次数
        /// </summary>
        protected long _getVIPBuySumCount()
        {
            long cnt = 0;
            if (null != mVipTable)
            {
                float num = Utility.GetCurVipLevelPrivilegeData(mVipTable.Type);
                if (num <= 0.0f)
                {
                    cnt = 0;
                }
                else 
                {
                    cnt = (long)num;
                }
            }

            return cnt;
        }

        /// <summary>
        /// 已经使用的次数
        /// </summary>
        protected long _getUsedCounter()
        {
            long cnt = 0;

            if (null != mDungeonTimesTable)
            {
                cnt = CountDataManager.GetInstance().GetCount(mDungeonTimesTable.UsedTimesCounter);
            }

            return cnt;
        }

        /// <summary>
        /// 已经购买的次数
        /// </summary>
        protected long _getBuyCounter()
        {
            long cnt = 0;
            if (null != mDungeonTimesTable)
            {
                cnt = CountDataManager.GetInstance().GetCount(mDungeonTimesTable.BuyTimesCounter);
            }

            return cnt;
        }

        /// <summary>
        /// 可以购买的总次数
        /// </summary>
        protected long _getBuySum()
        {
            return _getVIPBuySumCount() + _getCommonBuySumCount();
        }

        /// <summary>
        /// 剩余可以购买次数
        /// </summary>
        protected long _getLeftBuyCount()
        {
            return _getBuySum() - _getBuyCounter();
        }

        protected long _getLeftCount()
        {
            long leftCnt = 0;
            leftCnt = _getSumCount() - _getUsedCounter();

            if (leftCnt < 0)
            {
                //Logger.LogErrorFormat("[次数消耗] 剩余数值非法 {0}", leftCnt);
                leftCnt = 0;
            }

            return leftCnt;
        }

        protected long _getSumCount()
        {
            long cnt = 0;
            cnt = _getCommonSumCount() + _getBuyCounter();

            if (cnt < 0)
            {
                Logger.LogErrorFormat("[次数消耗] 总次数数值非法 {0}", cnt);
                cnt = 0;
            }

            return cnt;
        }

#region IActivityConsumeData implementation
        public long GetLeftCount()
        {
            return _getLeftCount();
        }

        public long GetSumCount()
        {
            return _getSumCount();
        }

        public bool IsCanBuyCount()
        {
            return _getLeftBuyCount() > 0;
        }

        public long GetLeftBuyCount()
        {
            return _getBuySum() - _getBuyCounter();
        }

        public long GetHasUsedCount()
        {
            return _getLeftBuyCount();
        }

        public long GetHasBuyCount()
        {
            return _getBuyCounter();
        }

        public int GetCostItemID()
        {
            if (null != mDungeonTimesTable)
            {
                return mDungeonTimesTable.BuyTimesCostItemID;
            }

            return 0;
        }

        public int GetCostItemCount()
        {
            int costCount = 0;
            int buyTimes  = (int)_getLeftBuyCount();

            if (null != mDungeonTimesTable)
            {
                if (mDungeonTimesTable.BuyTimesCost.Count > 0)
                {
                    costCount = mDungeonTimesTable.BuyTimesCost[mDungeonTimesTable.BuyTimesCost.Count - 1];
                }

                if (buyTimes >= 0 && buyTimes < mDungeonTimesTable.BuyTimesCost.Count)
                {
                    costCount = mDungeonTimesTable.BuyTimesCost[buyTimes];
                }
            }

            return costCount;
        }

        public byte GetCostItemType()
        {
            if (null != mDungeonTable)
            {
                return (byte)mDungeonTable.SubType;
            }

            return 0;
        }
#endregion

    }

    public class DeadTowerActivityConsumeData : BaseActivityConsumeData, IActivityConsumeData
    {
        public DeadTowerActivityConsumeData(int id) : base(id)
        {

        }

        private int _getDeadTowerNormalLeftCount()
        {
            return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_RESET_REMAIN_TIMES);
        }

        private int _getDeadTowerVipCount()
        {
            float totalCount = Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.WARRIOR_TOWER_REBEGIN_NUM);
            if (totalCount <= 0.0f)
            {
                return 0;
            }
            else 
            {
                return (int)totalCount - CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_VIP_TOWER_PAY_TIMES);
            }
        }


#region IActivityConsumeData implementation
        public long GetLeftCount()
        {
            return _getDeadTowerNormalLeftCount() + _getDeadTowerVipCount();
        }

        public long GetSumCount()
        {
            return 0;
        }

        public bool IsCanBuyCount()
        {
            return _getDeadTowerVipCount() > 0;
        }

        public long GetLeftBuyCount()
        {
            return 0;
        }

        public long GetHasUsedCount()
        {
            return 0;
        }

        public long GetHasBuyCount()
        {
            return 0;
        }
        
        public int GetCostItemID()
        {
            return 0;
        }

        public int GetCostItemCount()
        {
            return 0;
        }

        public byte GetCostItemType()
        {
            return 0;
        }
#endregion
    }
    public class FinalTestActivityConsumeData : BaseActivityConsumeData, IActivityConsumeData
    {
        public FinalTestActivityConsumeData(int id) : base(id)
        {
        }     
        #region IActivityConsumeData implementation
        public long GetLeftCount()
        {
            return ActivityDataManager.GetInstance().GetUltimateChallengeLeftCount();
        }
        public long GetSumCount()
        {
            return ActivityDataManager.GetInstance().GetUltimateChallengeMaxCount();
        }
        public bool IsCanBuyCount()
        {
            return false;
        }
        public long GetLeftBuyCount()
        {
            return 0;
        }
        public long GetHasUsedCount()
        {
            return 0;
        }
        public long GetHasBuyCount()
        {
            return 0;
        }
        public int GetCostItemID()
        {
            return 0;
        }
        public int GetCostItemCount()
        {
            return 0;
        }
        public byte GetCostItemType()
        {
            return 0;
        }
        #endregion
    }
}
